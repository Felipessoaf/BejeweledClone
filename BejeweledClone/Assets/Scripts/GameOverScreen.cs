using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    #region Variables
    [Header("References")]
    public TMP_Text HighscoreText;

    #endregion

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
        HighscoreText.text = "HIGHSCORE: " + PlayerPrefs.GetInt("Highscore");
    }
}
