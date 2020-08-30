using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoardManager : MonoBehaviour
{
    #region Variables
    [Header("References")]
    public GameObject GemPrefab;

    public bool CanMove { get; private set; } = true;

    private Vector3[,] GemPos;
    private Vector3[,] GemOutPos;
    private Gem[,] GemBoard;

    private int lineMax = 8, columnMax = 8;

    private static BoardManager _instance;
    #endregion

    public static BoardManager GetInstance()
    {
        return _instance;
    }

    #region Unity methods
    void Awake()
    {
        if(_instance == null)
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
        GameManager.StartGameDelegate += SetupBoard;
        GameManager.LevelUpdateDelegate += OnLevelUpdate;
    }

    private void OnDisable()
    {
        GameManager.StartGameDelegate -= SetupBoard;
        GameManager.LevelUpdateDelegate -= OnLevelUpdate;
    }
    #endregion

    private void OnLevelUpdate(int level)
    {
        //TODO: some animation?
        SetupBoard();
    }

    private void SetupBoard()
    {
        GemBoard = new Gem[lineMax, columnMax];
        GemPos = new Vector3[lineMax, columnMax];
        GemOutPos = new Vector3[lineMax, columnMax];
        StartCoroutine(GenerateBoard());
    }

    IEnumerator GenerateBoard()
    {
        //Wait one frame so that rect value is correct
        yield return new WaitForEndOfFrame();

        if(transform.childCount > 0)
        {
            for (int i = transform.childCount-1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        Rect rect = GetComponent<RectTransform>().rect;

        float diffX = rect.width/ lineMax;
        float diffY = rect.height/ columnMax;

        List<int> linesToCheck = new List<int>();
        List<int> columnsToCheck = new List<int>();

        for (int i = 0; i < lineMax; i++)
        {
            for (int j = 0; j < columnMax; j++)
            {
                Gem g = Instantiate(GemPrefab, transform).GetComponent<Gem>();
                //g.gameObject.name = "Gem[" + i + "," + j + "]";
                g.Line = i;
                g.Column = j;

                Vector2 pos = new Vector2(j * diffX, - i * diffY);
                g.transform.localPosition = pos;

                GemBoard[i, j] = g;
                GemPos[i, j] = pos;
                GemOutPos[i, j] = pos + Vector2.up * rect.height;

                g.GenerateId();

                columnsToCheck.Add(j);
            }
            linesToCheck.Add(i);
        }

        //Check Matches
        StartCoroutine(CheckMatch(linesToCheck, columnsToCheck, true, null));
    }

    public void CheckMove()
    {
        StartCoroutine(CheckMoveCoroutine());
    }

    private IEnumerator CheckMoveCoroutine()
    {
        int diffLine = Mathf.Abs(Gem.DraggedGem.Line - Gem.DropOnGem.Line);
        int diffCol = Mathf.Abs(Gem.DraggedGem.Column - Gem.DropOnGem.Column);

        if (diffLine == 1 && diffCol == 0 || diffLine == 0 && diffCol == 1)
        {
            yield return StartCoroutine(StartCheckMoveMatch());
        }

        //Reset gems
        Gem.DraggedGem = null;
        Gem.DropOnGem = null;
    }

    private IEnumerator StartCheckMoveMatch()
    {
        CanMove = false;

        //Switch gems
        int tempLine = Gem.DraggedGem.Line;
        int tempColumn = Gem.DraggedGem.Column;

        StartCoroutine(MoveGem(Gem.DraggedGem, Gem.DropOnGem.Line, Gem.DropOnGem.Column));
        yield return StartCoroutine(MoveGem(Gem.DropOnGem, tempLine, tempColumn));

        List<int> linesToCheck = new List<int>();
        List<int> columnsToCheck = new List<int>();

        //Add lines
        Utils.AddListElemIfNotExists(linesToCheck, Gem.DraggedGem.Line);
        Utils.AddListElemIfNotExists(linesToCheck, Gem.DropOnGem.Line);

        //Add columns
        Utils.AddListElemIfNotExists(columnsToCheck, Gem.DraggedGem.Column);
        Utils.AddListElemIfNotExists(columnsToCheck, Gem.DropOnGem.Column);

        //Check Matches
        List<Gem> matchedGems = new List<Gem>();
        yield return StartCoroutine(CheckMatch(linesToCheck, columnsToCheck, true, list => {
            matchedGems = list;
        }));

        if (matchedGems.Count == 0)
        {
            //Undo move
            tempLine = Gem.DraggedGem.Line;
            tempColumn = Gem.DraggedGem.Column;

            StartCoroutine(MoveGem(Gem.DraggedGem, Gem.DropOnGem.Line, Gem.DropOnGem.Column));
            yield return StartCoroutine(MoveGem(Gem.DropOnGem, tempLine, tempColumn));
        }

        CanMove = true;
    }

    private IEnumerator CheckMatch(List<int> linesToCheck, List<int> columnsToCheck, bool destroyGems, System.Action<List<Gem>> callback)
    {
        List<Gem> matchedGems = new List<Gem>();
        List<int> matchCombos = new List<int>();

        //TODO: simplify line/column same code

        //Lines
        foreach (var line in linesToCheck)
        {
            List<Gem> possibleMatch = new List<Gem>();
            for (int i = 0; i < columnMax; i++)
            {
                Gem current = GemBoard[line, i];
                Gem next = (i < columnMax - 1) ? GemBoard[line, i + 1] : null;

                if (next && current.GemId == next.GemId)
                {
                    Utils.AddListElemIfNotExists(possibleMatch, current);
                    Utils.AddListElemIfNotExists(possibleMatch, next);
                }
                else
                {
                    if (possibleMatch.Count >= 3)
                    {
                        foreach (var gem in possibleMatch)
                        {
                            Utils.AddListElemIfNotExists(matchedGems, gem);
                        }
                        //Store each match separately to count the points
                        matchCombos.Add(possibleMatch.Count);
                    }
                    possibleMatch.Clear();
                }
            }
        }

        //Columns
        foreach (var column in columnsToCheck)
        {
            List<Gem> possibleMatch = new List<Gem>();
            for (int i = lineMax - 1; i >= 0; i--)
            {
                Gem current = GemBoard[i, column];
                Gem next = (i > 0) ? GemBoard[i - 1, column] : null;

                if (next && current.GemId == next.GemId)
                {
                    Utils.AddListElemIfNotExists(possibleMatch, current);
                    Utils.AddListElemIfNotExists(possibleMatch, next);
                }
                else
                {
                    if (possibleMatch.Count >= 3)
                    {
                        foreach (var gem in possibleMatch)
                        {
                            Utils.AddListElemIfNotExists(matchedGems, gem);
                        }
                        //Store each match separately to count the points
                        matchCombos.Add(possibleMatch.Count);
                    }
                    possibleMatch.Clear();
                }
            }
        }

        if (matchedGems.Count > 0)
        {
            yield return StartCoroutine(DestroyGems(matchedGems));

            GameManager.GetInstance().AddScore(matchCombos);
        }

        callback?.Invoke(matchedGems);
    }

    private IEnumerator DestroyGems(List<Gem> matchedGems)
    {
        yield return new WaitForEndOfFrame();
        Dictionary<int, List<Gem>> gemsPerColumn = new Dictionary<int, List<Gem>>();

        foreach (var gem in matchedGems)
        {
            gem.gameObject.SetActive(false);
            
            if(!gemsPerColumn.ContainsKey(gem.Column))
            {
                gemsPerColumn.Add(gem.Column, new List<Gem>());
            }

            gemsPerColumn[gem.Column].Add(gem);
        }

        Coroutine lastMove = null;
        foreach (var pair in gemsPerColumn)
        {
            List<Gem> gemsToMove = pair.Value.OrderBy(gem => gem.Line).ToList();
            int firstGemLine = gemsToMove[0].Line;

            for (int i = 0; i < gemsToMove.Count; i++)
            {
                gemsToMove[i].GenerateId();
                gemsToMove[i].Line = gemsToMove[i].Line - gemsToMove.Count - firstGemLine;
                gemsToMove[i].transform.localPosition = GemOutPos[gemsToMove[i].Line + lineMax, gemsToMove[i].Column];
            }
            
            //Adds gems above match (if any) to move down
            for (int i = 0; i < firstGemLine; i++)
            {
                gemsToMove.Add(GemBoard[i, pair.Key]);
            }

            //Moves gems down
            for (int i = 0; i < gemsToMove.Count; i++)
            {
                Gem g = gemsToMove[i];
                int gemsDestroyed = pair.Value.Count;
                int line = g.Line + gemsDestroyed;

                lastMove = StartCoroutine(MoveGem(g, line, g.Column));
            }
        }

        yield return lastMove;

        List<int> linesToCheck = new List<int>();
        List<int> columnsToCheck = new List<int>();

        for (int i = 0; i < lineMax; i++)
        {
            linesToCheck.Add(i);
        }
        for (int i = 0; i < columnMax; i++)
        {
            columnsToCheck.Add(i);
        }

        yield return StartCoroutine(CheckMatch(linesToCheck, columnsToCheck, true, null));
    }

    private IEnumerator MoveGem(Gem gem, int line, int column)
    {
        //Switch positions ids
        gem.Line = line;
        gem.Column = column;

        if (gem.Line > lineMax || gem.Column > columnMax || gem.Line < 0 || gem.Column < 0)
        {
            Debug.LogError("out of bounds");
        }
        //Update GemBoard
        GemBoard[gem.Line, gem.Column] = gem;

        //Update position
        float frac = 0.1f;
        Vector3 initPos = gem.transform.localPosition;
        while (frac <= 1)
        {
            frac += 0.05f;
            yield return new WaitForSeconds(0.005f);
            gem.transform.localPosition = Vector3.Lerp(initPos, GemPos[gem.Line, gem.Column], frac);
        }
    }
}
