using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LWellsGame : LogicNode
{
    /// <summary>
    /// 棋盘的大小
    /// </summary>
    private const int boardSize = 3;
    /// <summary>
    /// 连续几个算赢
    /// </summary>
    private const int continuousToWin = 3;

    private int[,] checkBoard = new int[boardSize, boardSize];


    public enum PieceType
    {
        None = 0,
        Fork = 1,
        Circle = 2,
    }


    public override void OnAttach(ILogicNode parent)
    {
        EventCenter.Instance.AddListen((int)Events.Event_PrintPiece, OnPrintPiece);
    }


    public override void OnDetach(ILogicNode parent)
    {
        EventCenter.Instance.RemoveListen((int)Events.Event_PrintPiece, OnPrintPiece);
    }



    private void Init()
    {
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                checkBoard[x, y] = (int)PieceType.None;
            }
        }
    }

    List<Vector2Int[]> dirs = new List<Vector2Int[]>()
    {
        new Vector2Int[2] {new Vector2Int(1,0),new Vector2Int(-1,0)},
        new Vector2Int[2] {new Vector2Int(0,1),new Vector2Int(0,-1)},
        new Vector2Int[2] {new Vector2Int(1,1),new Vector2Int(-1,-1)},
        new Vector2Int[2] {new Vector2Int(1,-1),new Vector2Int(-1,1)},

    };

    /// <summary>
    /// 判断是否有人赢了
    /// </summary>
    /// <param name="pieceType"></param>
    /// <param name="winList"></param>
    /// <returns></returns>
    bool CheckWin(out PieceType pieceType, out List<Vector2Int> winList)
    {
        HashSet<Vector2Int> finded = new HashSet<Vector2Int>();

        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                Vector2Int coord = new Vector2Int(x, y);
                pieceType = (PieceType)checkBoard[coord.x, coord.y];
                if (pieceType != (int)PieceType.None && FindContinuous(coord, continuousToWin, out winList, finded))
                {
                    return true;
                }
                finded.Add(coord);
            }
        }
        pieceType = PieceType.None;
        winList = new List<Vector2Int>();
        return false;
    }

    /// <summary>
    /// 查找连续
    /// </summary>
    /// <param name="coord"></param>
    /// <param name="winCount"></param>
    /// <param name="winList"></param>
    /// <param name="hashCoord"></param>
    /// <returns></returns>
    bool FindContinuous(Vector2Int coord, int winCount, out List<Vector2Int> winList, HashSet<Vector2Int> hashCoord)
    {
        winList = new List<Vector2Int>();
        int pieceType = checkBoard[coord.x, coord.y];
        foreach (var dir in dirs)
        {
            winList.Clear();
            int count = 0;
            int step = 1;
            while (true)
            {
                Vector2Int nextCoord = coord + dir[0] * step;
                step++;
                if (CheckInBoard(nextCoord) && !hashCoord.Contains(nextCoord))//找过了就不找了吧
                {
                    int nextPieceType = checkBoard[nextCoord.x, nextCoord.y];
                    if (nextPieceType == pieceType)
                    {
                        winList.Add(nextCoord);
                        count++;
                        continue;
                    }
                }
                break;
            }

            step = 1;

            while (true)
            {
                Vector2Int nextCoord = coord + dir[1] * step;
                step++;
                if (CheckInBoard(nextCoord) && !hashCoord.Contains(nextCoord))
                {
                    int nextPieceType = checkBoard[nextCoord.x, nextCoord.y];
                    if (nextPieceType == pieceType)
                    {
                        winList.Add(nextCoord);
                        count++;
                        continue;
                    }
                }
                break;
            }

            if (count >= winCount)
            {
                return true;
            }

        }

        return false;

    }



    bool CheckInBoard(Vector2Int coord)
    {
        return coord.x >= 0 && coord.y >= 0 && coord.x < boardSize && coord.y < boardSize;
    }


    void OnPrintPiece(BaseEvent inEvent)
    {
        Events_PrintPiece events_PrintPiece = new Events_PrintPiece();
        if (events_PrintPiece == null) { return; }
        PrintPiece(events_PrintPiece.coord, events_PrintPiece.pieceType);
    }


    public void PrintPiece(Vector2Int coord, PieceType pieceType)
    {
        if (!CheckInBoard(coord))
        {
            Debug.LogError("coord is out board coord:" + coord.ToString());
            return;
        }
        int tPieceType = checkBoard[coord.x, coord.y];

        if (tPieceType != (int)PieceType.None)
        {
            Debug.LogError("curr coord is have  Piece");
            return;
        }

        checkBoard[coord.x, coord.y] = (int)pieceType;

        Events_SyncPiece syncPiece= new Events_SyncPiece();
        syncPiece.coord = coord;
        syncPiece.pieceType = pieceType;
        EventCenter.Instance.SendEvent(syncPiece);


       bool haveWin= CheckWin(out PieceType winPiece, out List<Vector2Int> winList);


        if (haveWin)
        {
            Events_GameResult gameResult= new Events_GameResult();
            gameResult.winList = winList;
            gameResult.winPieceType = winPiece;
            EventCenter.Instance.SendEvent(gameResult);
        }
    }

}
