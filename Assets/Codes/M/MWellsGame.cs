using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class MWellsGame : MonoLogicNode
{


    public GameObject prefPiece;

    public int pieceSize = 100;

    public int gapSize = 5;


    private Dictionary<Vector2Int, MPieceItem> allPieceDic = new Dictionary<Vector2Int, MPieceItem>();


    public override void OnAttach(ILogicNode parent)
    {
        EventCenter.Instance.AddListen((int)Events.Event_InitedLGame, OnInitedLGame);
        EventCenter.Instance.AddListen((int)Events.Event_SyncPiece, OnSyncPiece);
        EventCenter.Instance.AddListen((int)Events.Event_GameResult, OnGameOver);
    }

    private void OnSyncPiece(BaseEvent inEvent)
    {
        Events_SyncPiece events_SyncPiece = (Events_SyncPiece)inEvent;

        if (events_SyncPiece == null)
        {
            return;
        }

        if (allPieceDic.TryGetValue(events_SyncPiece.coord, out MPieceItem mPieceItem))
        {
            mPieceItem.SetPieceType(events_SyncPiece.pieceType);
            mPieceItem.SetColor(events_SyncPiece.color);
        }
    }

    public override void OnDetach(ILogicNode parent)
    {
        EventCenter.Instance.RemoveListen((int)Events.Event_InitedLGame, OnInitedLGame);
        EventCenter.Instance.RemoveListen((int)Events.Event_SyncPiece, OnSyncPiece);
        EventCenter.Instance.RemoveListen((int)Events.Event_GameResult, OnGameOver);

    }

    private void OnGameOver(BaseEvent e)
    {
        Events_GameResult evt = e as Events_GameResult;

        if (evt == null) return;

        if (evt.winList == null)
        {
            return;
        }
        foreach (var coord in evt.winList)
        {
            if (allPieceDic.TryGetValue(coord, out MPieceItem mPieceItem))
            {
                mPieceItem.SetColor(Color.red);
            }
        }


    }

    void OnInitedLGame(BaseEvent inEvent)
    {
        Events_Custom custom = inEvent as Events_Custom;
        if (custom == null)
        {
            Debug.LogError("MWellsGame.OnInitedLGame custom is null");
            return;
        }

        int boardSize = custom.intParam1;


        if (prefPiece == null)
        {
            Debug.LogError("MWellsGame.OnInitedLGame prefPiece is null");
        }
        float tPieceSize = pieceSize + gapSize;
        float origin = -(boardSize - 1) * tPieceSize * 0.5f;

        Vector3 originPos = new Vector3(origin, origin, 0);
        allPieceDic.Clear();

        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                GameObject go = Instantiate(prefPiece, transform);
                go.SetActive(true);
                MPieceItem mPieceItem = go.GetComponent<MPieceItem>();
                Vector2Int coord = new Vector2Int(x, y);
                mPieceItem.Init(coord);
                mPieceItem.SetSize(pieceSize);
                Vector3 currPos = originPos + new Vector3(tPieceSize * x, tPieceSize * y, 0);
                mPieceItem.transform.localPosition = currPos;
                allPieceDic.Add(coord, mPieceItem);
            }
        }

        Events_Custom events_Custom = new Events_Custom(Events.Event_InitedMGame);
        EventCenter.Instance.SendEvent(events_Custom);
    }


}
