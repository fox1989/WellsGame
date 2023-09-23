using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;





public class Main : MonoLogicNode
{

    public Transform gameLayerRoot;
    public Transform uiLayerRoot;







    private void Awake()
    {
        InitEventCenter();
        InitMGame();
        InitLGame();
        InitUICtrl();

        Events_Custom customEvent = new Events_Custom(Events.Event_MainInited);
        EventCenter.Instance.SendEvent(customEvent);
    }

    void InitEventCenter()
    {
        EventCenter eventCenter = new EventCenter();
        Attach(eventCenter);
    }


    private void InitMGame()
    {
        string mGameName = "pref_MWellsGame";
        GameObject prefGameGo = Resources.Load(mGameName) as GameObject;

        if (prefGameGo == null)
        {
            Debug.LogError("MGame prefab is null Name:" + mGameName);
            return;
        }

        GameObject mGameGo = Instantiate(prefGameGo, gameLayerRoot);
        mGameGo.transform.localPosition = Vector3.zero;
        mGameGo.SetActive(true);
        MWellsGame mGame = mGameGo.GetComponent<MWellsGame>();
        if (mGame == null)
        {
            Debug.LogError("MGame script not find null please and script by prefab Name:" + mGameName);
            return;
        }
        Attach(mGame);
    }

    void InitLGame()
    {
        LWellsGame lGame = new LWellsGame();
        Attach(lGame);
    }


    void InitUICtrl()
    {
        string uiCtrlName = "pref_UICtrl";
        GameObject prefUICtrl = Resources.Load(uiCtrlName) as GameObject;

        if (prefUICtrl == null)
        {
            Debug.LogError("UICtrl prefab is null Name:" + uiCtrlName);
            return;
        }

        GameObject go = Instantiate(prefUICtrl, uiLayerRoot);
        go.transform.localPosition = Vector3.zero;
        go.SetActive(true);
        UICtrl uiCtrl = go.GetComponent<UICtrl>();
        if (uiCtrl == null)
        {
            Debug.LogError("UICtrl script not find null please and script by prefab Name:" + uiCtrlName);
            return;
        }
        Attach(uiCtrl);
    }


    private void Update()
    {
        UpdateLogic(Time.deltaTime);
    }


}

