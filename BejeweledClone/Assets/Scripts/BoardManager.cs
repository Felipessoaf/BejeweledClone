using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("References")]
    public GameObject GemPrefab;

    public bool CanMove { get; private set; } = true;

    private Vector3[,] GemPos;
    private Gem[,] GemBoard;

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
        GemBoard = new Gem[8,8];
        GemPos = new Vector3[8, 8];
        StartCoroutine(GenerateBoard());
    }

    IEnumerator GenerateBoard()
    {
        yield return new WaitForEndOfFrame();
        Rect rect = GetComponent<RectTransform>().rect;
        //print(rect);

        float diffX = rect.width/8;
        float diffY = rect.height/8;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Gem g = Instantiate(GemPrefab, transform).GetComponent<Gem>();
                //g.gameObject.name = "Gem[" + i + "," + j + "]";
                g.Line = i;
                g.Column = j;

                Vector2 pos = new Vector2(j * diffX, - i * diffY);
                g.transform.localPosition = pos;

                GemBoard[i, j] = g;
                GemPos[i, j] = pos;

                //print("Gem[" + i + "," + j + "]");
                //print(pos);
                //print(g.transform.localPosition);
            }
        }
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
            yield return StartCoroutine(StartCheckMatch());
        }

        //Reset gems
        Gem.DraggedGem = null;
        Gem.DropOnGem = null;
    }

    private IEnumerator StartCheckMatch()
    {
        CanMove = false;
        yield return StartCoroutine(SwitchGems());

        List<int> linesToCheck = new List<int>();
        List<int> columnsToCheck = new List<int>();

        //Add lines
        Utils.AddListElemIfNotExists(linesToCheck, Gem.DraggedGem.Line);
        Utils.AddListElemIfNotExists(linesToCheck, Gem.DropOnGem.Line);

        //Add columns
        Utils.AddListElemIfNotExists(columnsToCheck, Gem.DraggedGem.Column);
        Utils.AddListElemIfNotExists(columnsToCheck, Gem.DropOnGem.Column);

        yield return new WaitForSeconds(1);
        yield return StartCoroutine(SwitchGems(true));

        CanMove = true;
    }

    private IEnumerator SwitchGems(bool back = false)
    {
        yield return new WaitForEndOfFrame();
        int tempLine, tempColumn;
        Gem from, to;

        from = back ? Gem.DropOnGem : Gem.DraggedGem;
        to = back ? Gem.DraggedGem : Gem.DropOnGem;

        //Store line and column on temp
        tempLine = from.Line;
        tempColumn = from.Column;

        //switch positions ids
        from.Line = to.Line;
        from.Column = to.Column;
        to.Line = tempLine;
        to.Column = tempColumn;

        //Update position
        from.transform.localPosition = BoardManager.GetInstance().GemPos[from.Line, from.Column];
        to.transform.localPosition = BoardManager.GetInstance().GemPos[to.Line, to.Column];
    }
}
