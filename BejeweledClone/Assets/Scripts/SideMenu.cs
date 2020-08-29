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

    public Button NewGameButton;
    public Button QuitGameButton;
    #endregion

    #region Unity methods
    void Start()
    {
        NewGameButton.onClick.AddListener(OnStartGame);
    }
    #endregion

    private void OnStartGame()
    {
        GameManager.StartGameDelegate?.Invoke();
    }
}
