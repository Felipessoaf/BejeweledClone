using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScreen : MonoBehaviour
{
    void Update()
    {
        if(Input.anyKeyDown)
        {
            GameManager.StartMenuDelegate?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
