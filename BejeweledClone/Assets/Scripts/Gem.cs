﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Gem : MonoBehaviour, IBeginDragHandler, IDropHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    public Sprite[] Sprites;

    public static Gem DraggedGem;
    public static Gem DropOnGem;

    public int Line, Column;

    private Image gemImage;

    private int gemId;

    void Start()
    {
        gemImage = GetComponentInChildren<Image>();

        gemId = Random.Range(0, Sprites.Length);
        gemImage.sprite = Sprites[gemId];
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

            BoardManager.GetInstance().SwitchGems();
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
