using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static LWellsGame;

public enum Events
{
    None,
    Event_PrintPiece,
    Event_SyncPiece,
    Event_GameResult,

    Event_MainInited,

    Event_InitedLGame,
    Event_InitedMGame,

    Event_ChangePlayerType,
    Event_SelectPieceType,



    Event_Begin,
    Event_ReBegin,
    Event_ShowSelectPiecePanel,
}

public class Events_Custom : BaseEvent
{
    public Events_Custom(Events inEvent)
    {
        eventKey = (int)inEvent;

    }

    private int eventKey = 0;

    public override int EventKey
    {
        get
        {
            return eventKey;
        }
    }

    public int intParam1;

    public string strParam1;

    public float floatParma1;

}


public class Events_PrintPiece : BaseEvent
{
    public override int EventKey => (int)Events.Event_PrintPiece;

    public PlayerType playerType;

    public Vector2Int coord;

}


public class Events_SyncPiece : BaseEvent
{
    public override int EventKey => (int)Events.Event_SyncPiece;

    public PieceType pieceType;

    public Vector2Int coord;

    public Color color = Color.white;
}


public class Events_GameResult : BaseEvent
{
    public override int EventKey => (int)Events.Event_GameResult;

    public PlayerType winPlayer;

    public List<Vector2Int> winList;
}

public class Events_ChangePlayerType : BaseEvent
{
    public override int EventKey => (int)Events.Event_ChangePlayerType;

    public PlayerType currPlayerType;

}

public class Events_SelectPieceType : BaseEvent
{
    public override int EventKey => (int)Events.Event_SelectPieceType;

    public PieceType pieceType;

}





