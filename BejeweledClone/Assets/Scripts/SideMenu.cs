using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SideMenu : MonoBehaviour
{
    #region Variables
    [Header("References")]
    public TMP_Text ScoreText;
    public TMP_Text LastScoreUpdateText;

    public Button NewGameButton;
    public Button QuitGameButton;
    #endregion

    #region Unity methods
    void Start()
    {
        NewGameButton.onClick.AddListener(StartGame);
        QuitGameButton.onClick.AddListener(QuitGame);
    }

    private void OnEnable()
    {
        GameManager.ScoreUpdateDelegate += OnScoreUpdate;
    }

    private void OnDisable()
    {
        GameManager.ScoreUpdateDelegate -= OnScoreUpdate;
    }
    #endregion

    private void StartGame()
    {
        GameManager.StartGameDelegate?.Invoke();
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    private void OnScoreUpdate(int score, int scoreUpdate)
    {
        ScoreText.text = "Score: " + score;
        LastScoreUpdateText.text = "+" + scoreUpdate;
    }
}
