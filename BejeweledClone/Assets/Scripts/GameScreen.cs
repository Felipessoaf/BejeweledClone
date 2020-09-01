using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScreen : MonoBehaviour
{
    void Start()
    {
        GameManager.StartMenuDelegate += Show;
    }

    private void OnDestroy()
    {
        GameManager.StartMenuDelegate -= Show;
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
