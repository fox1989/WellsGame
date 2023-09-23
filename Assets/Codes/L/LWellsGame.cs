using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

public enum PieceType
{
    None = 0,
    Fork = 1,
    Circle = 2,
}

public enum PlayerType
{
    None,
    /// <summary>
    /// 玩家
    /// </summary>
    Player,
    /// <summary>
    /// Ai
    /// </summary>
    Ai,

    Max
}


public class LWellsGame : LogicNode
{

    /// <summary>
    /// 当前是谁的回合
    /// </summary>
    public PlayerType currPlayType = PlayerType.None;

    public PieceType playerPieceType = PieceType.None;

    public PieceType AiPieceType = PieceType.None;

    /// <summary>
    /// 棋盘的大小
    /// </summary>
    private const int boardSize = 3;
    /// <summary>
    /// 连续几个算赢
    /// </summary>
    private const int continuousToWin = 3;

    private int[,] checkBoard = new int[boardSize, boardSize];


    public override void OnAttach(ILogicNode parent)
    {
        EventCenter.Instance.AddListen((int)Events.Event_PrintPiece, OnPrintPiece);
        EventCenter.Instance.AddListen((int)Events.Event_MainInited, OnInit);
        EventCenter.Instance.AddListen((int)Events.Event_InitedMGame, OnMGameInited);
        EventCenter.Instance.AddListen((int)Events.Event_GameResult, OnGameOver);
        EventCenter.Instance.AddListen((int)Events.Event_ReBegin, OnReBegin);
        EventCenter.Instance.AddListen((int)Events.Event_SelectPieceType, OnSelectPieceType);

    }

    public override void OnDetach(ILogicNode parent)
    {
        EventCenter.Instance.RemoveListen((int)Events.Event_PrintPiece, OnPrintPiece);
        EventCenter.Instance.RemoveListen((int)Events.Event_MainInited, OnInit);
        EventCenter.Instance.RemoveListen((int)Events.Event_InitedMGame, OnMGameInited);
        EventCenter.Instance.RemoveListen((int)Events.Event_GameResult, OnGameOver);
        EventCenter.Instance.RemoveListen((int)Events.Event_ReBegin, OnReBegin);
        EventCenter.Instance.RemoveListen((int)Events.Event_SelectPieceType, OnSelectPieceType);

    }


    private void OnGameOver(BaseEvent e)
    {
        currPlayType = PlayerType.None;
    }


    /// <summary>
    /// 重新开始
    /// </summary>
    /// <param name="e"></param>
    private void OnReBegin(BaseEvent e)
    {
        ///刷新，同步M层
        OnMGameInited(null);

    }




    void OnInit(BaseEvent inEvent)
    {
        ResetBoard();
        Events_Custom events_Custom = new Events_Custom(Events.Event_InitedLGame);
        events_Custom.intParam1 = boardSize;
        EventCenter.Instance.SendEvent(events_Custom);
    }


    void OnMGameInited(BaseEvent inEvent)
    {
        ResetBoard();

        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                PieceType pieceType = (PieceType)checkBoard[x, y];
                Events_SyncPiece events_SyncPiece = new Events_SyncPiece();
                events_SyncPiece.pieceType = pieceType;
                events_SyncPiece.coord = new Vector2Int(x, y);
                EventCenter.Instance.SendEvent(events_SyncPiece);
            }
        }


        Events_Custom evt = new Events_Custom(Events.Event_ShowSelectPiecePanel);
        EventCenter.Instance.SendEvent(evt);
    }



    void ResetBoard()
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
    /// /// <param name="gapCount">可以隔一个</param>
    /// <returns></returns>
    bool FindContinuous(Vector2Int coord, int winCount, out List<Vector2Int> winList, HashSet<Vector2Int> hashCoord)
    {
        winList = new List<Vector2Int>();
        int pieceType = checkBoard[coord.x, coord.y];
        foreach (var dir in dirs)
        {
            winList.Clear();
            winList.Add(coord);
            int count = 1;// 自己已经算了一个了
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
        Events_PrintPiece events_PrintPiece = inEvent as Events_PrintPiece;
        if (events_PrintPiece == null) { return; }
        if (events_PrintPiece.playerType != currPlayType) return;

        PieceType pieceType = PieceType.None;

        switch (events_PrintPiece.playerType)
        {
            case PlayerType.None:
                break;
            case PlayerType.Player:
                pieceType = playerPieceType; break;
            case PlayerType.Ai: pieceType = AiPieceType; break;

        }

        PrintPiece(events_PrintPiece.coord, pieceType);


    }

    /// <summary>
    /// 画一个格子
    /// </summary>
    /// <param name="coord"></param>
    /// <param name="pieceType"></param>
    void PrintPiece(Vector2Int coord, PieceType pieceType)
    {
        if (!CheckInBoard(coord))
        {
            Debug.LogError("coord is out board coord:" + coord.ToString());
            return;
        }
        int tPieceType = checkBoard[coord.x, coord.y];

        if (tPieceType != (int)PieceType.None)
        {
            Debug.LogError("curr coord is have  Piece corrd:" + coord.ToString());
            return;
        }

        checkBoard[coord.x, coord.y] = (int)pieceType;

        Events_SyncPiece syncPiece = new Events_SyncPiece();
        syncPiece.coord = coord;
        syncPiece.pieceType = pieceType;
        EventCenter.Instance.SendEvent(syncPiece);


        bool haveWin = CheckWin(out PieceType winPiece, out List<Vector2Int> winList);


        if (haveWin)
        {

            Debug.Log("haveWin type:" + winPiece.ToString());

            Events_GameResult gameResult = new Events_GameResult();
            gameResult.winList = winList;
            gameResult.winPlayer = winPiece == playerPieceType ? PlayerType.Player : PlayerType.Ai;
            EventCenter.Instance.SendEvent(gameResult);
            return;
        }

        bool isTie = true;
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                if (checkBoard[x, y] == (int)PieceType.None)
                {
                    isTie = false;
                    break;
                }
            }
            if (!isTie) break;
        }

        if (isTie)
        {
            Events_GameResult gameResult = new Events_GameResult();
            gameResult.winList = null;
            gameResult.winPlayer = PlayerType.None;
            EventCenter.Instance.SendEvent(gameResult);
        }

        ChangeCurrPlayer();


    }

    /// <summary>
    /// 回合转换
    /// </summary>
    void ChangeCurrPlayer()
    {
        currPlayType = currPlayType == PlayerType.Player ? PlayerType.Ai : PlayerType.Player;

        SendChangePlayer();
    }




    void SetPlayerPieceType(PieceType pieceType)
    {
        playerPieceType = pieceType;
        AiPieceType = pieceType == PieceType.Circle ? PieceType.Fork : PieceType.Circle;
    }


    void SendChangePlayer()
    {
        Events_ChangePlayerType evt = new Events_ChangePlayerType();
        evt.currPlayerType = currPlayType;
        EventCenter.Instance.SendEvent(evt);
    }



    public void OnSelectPieceType(BaseEvent inEvent)
    {
        Events_SelectPieceType evt = inEvent as Events_SelectPieceType;
        if (evt == null) return;
        SetPlayerPieceType(evt.pieceType);
        RandomFirst();

        Events_Custom custom = new Events_Custom(Events.Event_Begin);
        EventCenter.Instance.SendEvent(custom);


    }

    /// <summary>
    /// 随机一个先手
    /// </summary>
    void RandomFirst()
    {
        currPlayType = (PlayerType)UnityEngine.Random.Range((int)PlayerType.Player, (int)PlayerType.Max);
        SendChangePlayer();
    }


    public override void OnUpdateLogic(float deltaTime)
    {
        AiWaiting(deltaTime);
    }




    #region AI

    const float aiWaitingTime = 1.5f;
    float curAiWaitingTime = 0;

    void AiWaiting(float deltaTime)
    {
        if (currPlayType != PlayerType.Ai)
        {
            return;
        }

        curAiWaitingTime += deltaTime;

        if (curAiWaitingTime > aiWaitingTime)
        {
            AiPlaying();
            curAiWaitingTime = 0;
        }


    }


    void AiPlaying()
    {
        HashSet<Vector2Int> finded = new HashSet<Vector2Int>();
        PieceType pieceType = playerPieceType;
        bool findPlayCoord = false;
        List<Vector2Int> coords = new List<Vector2Int>();
        int maxIndex = 0;
        int maxContinuous = 0;
        List<Vector2Int> randomList = new List<Vector2Int>();
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                Vector2Int coord = new Vector2Int(x, y);
                PieceType tPieceType = (PieceType)checkBoard[coord.x, coord.y];
                if (pieceType == tPieceType)
                {
                    AiFindContinuous(coord, out List<Vector2Int> reList, out int max, out int tMaxIndex, finded);
                    finded.Add(coord);

                    if (max > maxContinuous)
                    {
                        coords = reList;
                        maxContinuous = max;
                        maxIndex = tMaxIndex;
                    }
                    break;
                }
                else if (tPieceType == PieceType.None)
                {
                    randomList.Add(coord);
                }

            }
        }

        if (coords.Count > 0 && coords.Count > maxIndex && maxContinuous >= continuousToWin - 1)//防守
        {
            Vector2Int playCoord = coords[maxIndex];
            AiPrint(playCoord);
        }
        else if (randomList.Count > 0)//进攻，这是个弱智电脑 ，就让它随机下吧
        {
            int randomIndex = UnityEngine.Random.Range(0, randomList.Count);
            Vector2Int tCoord = randomList[randomIndex];
            AiPrint(tCoord);
        }
        else
        {
            //理论上这里平局了吧
            Events_GameResult gameResult = new Events_GameResult();
            gameResult.winList = null;
            gameResult.winPlayer = PlayerType.None;
            EventCenter.Instance.SendEvent(gameResult);
        }
    }


    void AiPrint(Vector2Int coord)
    {
        PrintPiece(coord, AiPieceType);
    }


    void AiFindContinuous(Vector2Int coord, out List<Vector2Int> reList, out int maxContinuous, out int maxIndex, HashSet<Vector2Int> hashCoord)
    {

        int gapCount = 2;
        reList = new List<Vector2Int>();
        maxContinuous = 0;
        maxIndex = 0;

        foreach (var dir in dirs)
        {
            List<Vector2Int> tCoordList = new List<Vector2Int>();

            int currGapcount = 0;//跳的范围
            int step = 1;
            tCoordList.Add(coord);
            while (true)
            {
                Vector2Int nextCoord = coord + dir[0] * step;
                step++;
                if (CheckInBoard(nextCoord) && !hashCoord.Contains(nextCoord))//找过了就不找了吧
                {
                    int nextPieceType = checkBoard[nextCoord.x, nextCoord.y];
                    if (nextPieceType == (int)PieceType.None)
                    {
                        currGapcount++;
                    }
                    tCoordList.Add(nextCoord);

                    if (currGapcount >= gapCount)
                    { break; }

                    continue;
                }
                break;
            }

            step = 1;
            currGapcount = 0;
            while (true)
            {
                Vector2Int nextCoord = coord + dir[1] * step;
                step++;
                if (CheckInBoard(nextCoord) && !hashCoord.Contains(nextCoord))
                {
                    int nextPieceType = checkBoard[nextCoord.x, nextCoord.y];
                    if (nextPieceType == (int)PieceType.None)
                    {
                        currGapcount++;
                    }
                    tCoordList.Add(nextCoord);

                    if (currGapcount >= gapCount)
                    { break; }

                    continue;
                }
                break;
            }

            tCoordList.Sort(delegate (Vector2Int a, Vector2Int b)
            {
                return a.sqrMagnitude.CompareTo(b.sqrMagnitude);
            });


            int currCompareType = checkBoard[coord.x, coord.y];
            int tMax = 0;
            int tMaxIndex = -1;
            FindMax(tCoordList, currCompareType, ref tMax, ref tMaxIndex);
            if (tMax > maxContinuous && tMaxIndex >= 0)
            {
                maxContinuous = tMax;
                reList = tCoordList;
                maxIndex = tMaxIndex;
            }
        }
    }


    void FindMax(List<Vector2Int> tCoordList, int currCompareType, ref int tMax, ref int maxIndex)
    {
        if (tCoordList.Count <= 0)
            return;
        int tCount = 0;
        for (int i = 0; i < tCoordList.Count; i++)
        {
            Vector2Int tcoord = tCoordList[i];
            int tPieceType = checkBoard[tcoord.x, tcoord.y];
            if (currCompareType == tPieceType)
            {
                tCount++;
            }
            else
            {
                if (tPieceType == (int)PieceType.None)
                {
                    maxIndex = i;
                }

                tMax = Math.Max(tMax, tCount);
                tCount = 0;
            }
        }
        tMax = Math.Max(tMax, tCount);

    }






    #endregion







}
