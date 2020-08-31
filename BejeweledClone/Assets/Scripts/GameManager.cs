using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Delegates
    public delegate void OnStartGame();
    public static OnStartGame StartGameDelegate;

    public delegate void OnScoreUpdate(int score, int scoreUpdate);
    public static OnScoreUpdate ScoreUpdateDelegate;

    public delegate void OnProgressUpdate(float progress);
    public static OnProgressUpdate ProgressUpdateDelegate;

    public delegate void OnLevelUpdate(int level);
    public static OnLevelUpdate LevelUpdateDelegate;

    public delegate void OnPointsText(Vector3 pos, int points);
    public static OnPointsText PointsTextDelegate;

    //TODO:player pref highscore?
    public delegate void OnGameOver();
    public static OnGameOver GameOverDelegate;

    public delegate void OnStartMenu();
    public static OnStartMenu StartMenuDelegate;
    #endregion

    #region Variables
    //Update score: gemComboPoints = base + base*gemsOver3 + comboIndex
    //base: how much one simple 3 match is worth based on current level
    //gemsOver3: total gems - 3, I.E. matches with more than 3 gems generates more points
    //comboIndex: the "index" of that match if more than one match was made 
    private int score;
    private int currentLevel;
    private int baseMatchPoints;
    private int previousLevelPoints;
    private int nextLevelPoints;

    private static GameManager _instance;
    #endregion

    public static GameManager GetInstance()
    {
        return _instance;
    }

    #region Unity methods
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        GameManager.StartGameDelegate += SetupGame;
    }

    private void OnDisable()
    {
        GameManager.StartGameDelegate -= SetupGame;
    }
    #endregion

    private void SetupGame()
    {
        //TODO: make this a json?
        baseMatchPoints = 10;
        score = 0;
        currentLevel = 1;
        previousLevelPoints = 0;
        nextLevelPoints = 1000;

        ScoreUpdateDelegate?.Invoke(score, 0);
        ProgressUpdateDelegate?.Invoke(0);
    }

    public void AddScore(List<List<Gem>> combos)
    {
        //Update score: gemComboPoints = base + base*gemsOver3 + base*comboIndex
        //base: how much one simple 3 match is worth based on current level
        //gemsOver3: total gems - 3, I.E. matches with more than 3 gems generates more points
        //comboIndex: the "index" of that match if more than one match was made 
        int comboScore = 0;
        for (int i = 0; i < combos.Count; i++)
        {
            comboScore += baseMatchPoints*(1 + (combos[i].Count-3) + i);

            PointsTextDelegate?.Invoke(combos[i][combos[i].Count / 2].transform.position, comboScore);
        }

        //Update score
        score += comboScore;
        ScoreUpdateDelegate?.Invoke(score, comboScore);

        //Update progress
        float progress = (float)(score - previousLevelPoints)/(nextLevelPoints - previousLevelPoints);
        ProgressUpdateDelegate?.Invoke(progress);

        //Update Level
        if(progress >= 1)
        {
            //Update values
            currentLevel++;
            baseMatchPoints += 10;
            previousLevelPoints = nextLevelPoints;
            nextLevelPoints += 1000;

            ProgressUpdateDelegate?.Invoke(0);
            LevelUpdateDelegate?.Invoke(currentLevel);
        }
    }
}
