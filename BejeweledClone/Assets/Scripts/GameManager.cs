using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public delegate void OnStartGame();
    public static OnStartGame StartGameDelegate;
}
