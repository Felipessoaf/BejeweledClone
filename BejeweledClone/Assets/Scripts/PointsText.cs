using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PointsText : MonoBehaviour
{
    #region Variables
    [Header("References")]
    public TMP_Text PointsUpText;
    #endregion

    public void SetText(int points)
    {
        PointsUpText.text = "+" + points;
    }

    public void DestroyAfterAnim()
    {
        Destroy(gameObject);
    }
}
