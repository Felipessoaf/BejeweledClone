using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoardManager : MonoBehaviour
{
    [Header("References")]
    public GameObject GemPrefab;

    public bool CanMove { get; private set; } = true;

    private Vector3[,] GemPos;
    private Vector3[,] GemOutPos;
    private Gem[,] GemBoard;

    private int lineMax = 8, columnMax = 8;

    private static BoardManager _instance;

    public static BoardManager GetInstance()
    {
        return _instance;
    }

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

    void Start()
    {
        GemBoard = new Gem[lineMax, columnMax];
        GemPos = new Vector3[lineMax, columnMax];
        GemOutPos = new Vector3[lineMax, columnMax];
        StartCoroutine(GenerateBoard());
    }

    IEnumerator GenerateBoard()
    {
        yield return new WaitForEndOfFrame();
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

                //print("Gem[" + i + "," + j + "]");
                //print(pos);
                //print(g.transform.localPosition);

                columnsToCheck.Add(j);
            }
            linesToCheck.Add(i);
        }

        //Check Matches
        List<Gem> matchedGems = CheckMatch(linesToCheck, columnsToCheck, true);

        //if (matchedGems.Count > 0)
        //{
        //    //print("gerando novo board");
        //    //StartCoroutine(GenerateBoard());
        //    foreach (var gem in matchedGems)
        //    {
        //        gem.gameObject.SetActive(false);
        //    }
        //}
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
        List<Gem> matchedGems = CheckMatch(linesToCheck, columnsToCheck, true);

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

    private List<Gem> CheckMatch(List<int> linesToCheck, List<int> columnsToCheck, bool destroyGems)
    {
        List<Gem> matchedGems = new List<Gem>();

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
                    }
                    possibleMatch.Clear();
                }
            }
        }

        if (matchedGems.Count > 0)
        {
            //TODO: yield return?
            StartCoroutine(DestroyGems(matchedGems));
        }

        return matchedGems;
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

        CheckMatch(linesToCheck, columnsToCheck, true);
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
