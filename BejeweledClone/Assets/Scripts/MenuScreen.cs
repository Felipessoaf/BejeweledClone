using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScreen : MonoBehaviour
{
    #region Variables
    [Header("References")]
    public Button StartButton;
    #endregion

    private void Start()
    {
        StartButton.onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        GameManager.StartMenuDelegate?.Invoke();
        gameObject.SetActive(false);
    }
}
