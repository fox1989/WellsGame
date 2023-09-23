using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICtrl : MonoLogicNode
{
    List<GameObject> allUiPlanes = new List<GameObject>();

    public GameObject gameOverPanel;
    public Text gameOverText;

    public GameObject playingPanel;
    public Text tipText;


    public GameObject selectPiecePanel;
    public GameObject quitPanel;



    public override void OnAttach(ILogicNode parent)
    {
        EventCenter.Instance.AddListen((int)Events.Event_GameResult, OnGameOver);
        EventCenter.Instance.AddListen((int)Events.Event_Begin, OnBegin);
        EventCenter.Instance.AddListen((int)Events.Event_ChangePlayerType, OnChangePlayer);
        EventCenter.Instance.AddListen((int)Events.Event_ShowSelectPiecePanel, OnShowSelectPiece);
    }



    public override void OnDetach(ILogicNode parent)
    {
        EventCenter.Instance.RemoveListen((int)Events.Event_GameResult, OnGameOver);
        EventCenter.Instance.RemoveListen((int)Events.Event_Begin, OnBegin);
        EventCenter.Instance.RemoveListen((int)Events.Event_ChangePlayerType, OnChangePlayer);
        EventCenter.Instance.RemoveListen((int)Events.Event_ShowSelectPiecePanel, OnShowSelectPiece);

    }

    private void Awake()
    {
        allUiPlanes.Add(gameOverPanel);
        allUiPlanes.Add(playingPanel);
        allUiPlanes.Add(selectPiecePanel);
    }




    void CloseAll()
    {

        for (int i = 0; i < allUiPlanes.Count; i++)
        {
            allUiPlanes[i].SetActive(false);
        }
    }

    #region GameOver

    private void OnGameOver(BaseEvent e)
    {
        Events_GameResult evt = e as Events_GameResult;

        if (evt == null)
            return;
        CloseAll();


        if (gameOverText)
        {
            string showTex = "";
            switch (evt.winPlayer)
            {
                case PlayerType.Player:
                    showTex = "恭喜你获胜了！";
                    break;
                case PlayerType.None:
                    showTex = "你和电脑平分秋色~";
                    break;
                case PlayerType.Ai:
                    showTex = "惜败电脑，下次加油~";
                    break;
            }


            gameOverText.text = showTex;
        }
        gameOverPanel.SetActive(true);
    }



    public void OnClick_ReBegin()
    {
        Events_Custom evt = new Events_Custom(Events.Event_ReBegin);
        EventCenter.Instance.SendEvent(evt);
    }


    public void OnClick_OnQuit()
    {
        if (quitPanel)
        {
            quitPanel.SetActive(true);
        }

    }


    #endregion


    private void OnBegin(BaseEvent e)
    {
        CloseAll();

        Debug.Log("OnBegin");

        if (playingPanel)
        {
            playingPanel.SetActive(true);
        }
    }


    private void OnChangePlayer(BaseEvent e)
    {
        Events_ChangePlayerType evt = e as Events_ChangePlayerType;
        if (evt == null) return;

        if (tipText)
        {
            string showText = "";

            switch (evt.currPlayerType)
            {
                case PlayerType.Player:
                    showText = "请在对应格子上画...";
                    break;
                case PlayerType.Ai:
                    showText = "等待对手中...";
                    break;

            }
            tipText.text = showText;
        }
    }




    private void OnShowSelectPiece(BaseEvent e)
    {
        CloseAll();

        if (selectPiecePanel)
        {
            selectPiecePanel.SetActive(true);
        }
    }


    public void OnClick_SelectFork()
    {
        Events_SelectPieceType evt = new Events_SelectPieceType();
        evt.pieceType = PieceType.Fork;
        EventCenter.Instance.SendEvent(evt);

    }


    public void OnClick_SlelectCircle()
    {
        Events_SelectPieceType evt = new Events_SelectPieceType();
        evt.pieceType = PieceType.Circle;
        EventCenter.Instance.SendEvent(evt);
    }



    public void OnClick_Cancel()
    {
        if (quitPanel)
        {
            quitPanel.SetActive(false);
        }
    }


    public void OnClick_Quit()
    {
        Application.Quit();//暴力退出吧，正常流程要把每一个node Detach 之后回到主界面，没有就算了
    }


}
