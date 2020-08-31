using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
        GameManager.GameOverDelegate += Show;
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            GameManager.StartMenuDelegate?.Invoke();
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        GameManager.GameOverDelegate -= Show;
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
