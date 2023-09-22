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
}
public class Events_PrintPiece : BaseEvent
{
    public override int EventKey => (int)Events.Event_PrintPiece;

    public PieceType pieceType;

    public Vector2Int coord;

}


public class Events_SyncPiece : BaseEvent
{
    public override int EventKey => (int)Events.Event_SyncPiece;

    public PieceType pieceType;

    public Vector2Int coord;

    /// <summary>
    /// 未来支持 增删改吧，现在就只有增加
    /// </summary>
    public int actionType = 0;
}


public class Events_GameResult : BaseEvent
{
    public override int EventKey => (int)Events.Event_GameResult;

    public PieceType winPieceType;

    public List<Vector2Int> winList;
}



