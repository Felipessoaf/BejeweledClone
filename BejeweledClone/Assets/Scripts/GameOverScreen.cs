using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    #region Variables
    [Header("References")]
    public TMP_Text HighscoreText;
    public Button RestartButton;

    #endregion

    void Start()
    {
        gameObject.SetActive(false);
        GameManager.GameOverDelegate += Show;
        RestartButton.onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        GameManager.StartMenuDelegate?.Invoke();
        gameObject.SetActive(false);
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
