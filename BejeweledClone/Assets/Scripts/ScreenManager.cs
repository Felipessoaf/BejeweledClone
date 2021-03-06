﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    #region Variables
    [Header("References")]
    public GameObject MenuScreen;
    public GameObject GameScreen;
    public GameObject GameOverScreen;
    #endregion

    void Start()
    {
        MenuScreen.SetActive(true);
        GameScreen.SetActive(true);
        GameOverScreen.SetActive(true);

        StartCoroutine(DeactivateScreens());
    }

    IEnumerator DeactivateScreens()
    {
        yield return new WaitForEndOfFrame();
        GameOverScreen.SetActive(false);
    }
}
