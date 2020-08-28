using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gem : MonoBehaviour
{
    [Header("References")]
    public Sprite[] Sprites;

    private Image gemImage;

    private int gemId;

    void Start()
    {
        gemImage = GetComponentInChildren<Image>();

        gemId = Random.Range(0, Sprites.Length);
        gemImage.sprite = Sprites[gemId];
    }
}
