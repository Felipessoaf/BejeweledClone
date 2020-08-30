using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressBar : MonoBehaviour
{
    #region Variables
    [Header("References")]
    public Image FillImage;

    public TMP_Text LevelText;
    #endregion

    #region Unity methods

    private void OnEnable()
    {
        GameManager.ProgressUpdateDelegate += OnProgressUpdate;
        GameManager.LevelUpdateDelegate += OnLevelUpdate;
    }

    private void OnDisable()
    {
        GameManager.ProgressUpdateDelegate -= OnProgressUpdate;
        GameManager.LevelUpdateDelegate -= OnLevelUpdate;
    }
    #endregion

    private void OnProgressUpdate(float progress)
    {
        FillImage.fillAmount = progress;
    }

    private void OnLevelUpdate(int level)
    {
        LevelText.text = level.ToString();
    }
}
