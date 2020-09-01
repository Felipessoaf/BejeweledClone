using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Gem : MonoBehaviour, IBeginDragHandler, IDropHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    #region Variables
    [Header("References")]
    public Sprite[] Sprites;
    public GameObject SelectionBorder;

    public GameObject ExplosionParticle;

    public static Gem DraggedGem;
    public static Gem DropOnGem;

    public int Line, Column;
    public int GemId { get; private set; }

    private Image gemImage;
    #endregion

    public void GenerateId()
    {
        if(gemImage == null)
        {
            gemImage = GetComponentInChildren<Image>();
        }

        GemId = Random.Range(0, Sprites.Length);
        gemImage.sprite = Sprites[GemId];
        gameObject.SetActive(true);
    }

    #region MainHandlers
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (BoardManager.GetInstance().CanMove)
        {
            DraggedGem = this;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (BoardManager.GetInstance().CanMove)
        {
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
    #endregion

    public void DestroyGem()
    {
        Instantiate(ExplosionParticle, transform.position, ExplosionParticle.transform.rotation, transform.parent);
        gameObject.SetActive(false);
    }

    #region OtherHandlers
    public void OnEndDrag(PointerEventData eventData)
    {
        //print("OnEndDrag: " + gameObject.name);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //print("OnDrag: " + gameObject.name);
    }
    #endregion
}
