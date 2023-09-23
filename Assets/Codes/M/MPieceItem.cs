using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class MPieceItem : MonoBehaviour
{

    public Image image;
    public Vector2Int coord;

    public PieceType pieceType = PieceType.None;

    public Sprite forkIcon;
    public Sprite circleIcon;

    public void Init(Vector2Int coord)
    {
        this.coord = coord;
        gameObject.name = coord.ToString();
    }


    public void SetSize(int size)
    {
        image.rectTransform.sizeDelta = new Vector2(size, size);
    }



    public void OnClick()
    {
        Events_PrintPiece events_PrintPiece = new Events_PrintPiece();

        events_PrintPiece.coord = coord;
        events_PrintPiece.playerType = PlayerType.Player;
        EventCenter.Instance.SendEvent(events_PrintPiece);
        Debug.Log("click :" + coord);
    }


    public void SetPieceType(PieceType pieceType)
    {
        this.pieceType = pieceType;
        Sprite icon = null;
        switch (pieceType)
        {
            case PieceType.None:
                break;
            case PieceType.Circle: icon = circleIcon; break;
            case PieceType.Fork: icon = forkIcon; break;
        }

        image.sprite = icon;
    }




}
