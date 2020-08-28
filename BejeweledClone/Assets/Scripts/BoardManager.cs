using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("References")]
    public GameObject GemPrefab;

    private Gem[,] GemBoard;
    private Vector3[,] GemPos;

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
                g.gameObject.name = "Gem[" + i + "," + j + "]";

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
}
