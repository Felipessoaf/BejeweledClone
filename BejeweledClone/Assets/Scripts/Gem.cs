﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Gem : MonoBehaviour, IBeginDragHandler, IDropHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("References")]
    public Sprite[] Sprites;
    public GameObject SelectionBorder;

    public static Gem DraggedGem;
    public static Gem DropOnGem;

    public int Line, Column;
    public int GemId { get; private set; }

    private Image gemImage;

    public void GenerateId()
    {
        if(gemImage == null)
        {
            gemImage = GetComponentInChildren<Image>();
        }

        GemId = Random.Range(0, Sprites.Length);
        gemImage.sprite = Sprites[GemId];
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (BoardManager.GetInstance().CanMove)
        {
            //print("OnBeginDrag: " + gameObject.name);
            DraggedGem = this;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (BoardManager.GetInstance().CanMove)
        {
            //print("OnDrop: " + gameObject.name);
            DropOnGem = this;

            BoardManager.GetInstance().CheckMove();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (BoardManager.GetInstance().CanMove)
        {
            if(DraggedGem == null)
            {
                DraggedGem = this;
                SelectionBorder.SetActive(true);
            }
            else
            {
                DraggedGem.SelectionBorder.SetActive(false);
                DropOnGem = this;

                BoardManager.GetInstance().CheckMove();
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //print("OnEndDrag: " + gameObject.name);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //print("OnDrag: " + gameObject.name);
    }
}
