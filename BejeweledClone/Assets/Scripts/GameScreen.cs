using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScreen : MonoBehaviour
{
    void Start()
    {
        Hide();
        GameManager.StartMenuDelegate += Show;
        GameManager.GameOverDelegate += Hide;
    }

    private void OnDestroy()
    {
        GameManager.StartMenuDelegate -= Show;
        GameManager.GameOverDelegate -= Hide;
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
