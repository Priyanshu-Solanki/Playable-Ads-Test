using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public int x;
    public int y;
    public int offset;
    public GameObject tilePrefab;
    private BackgroundTile[,] allTiles;
    public GameObject[] candies;
    public GameObject[,] allCandies;

    public GameObject startButton;
    public GameObject gameScreen;
    public int moveCount;
    public GameObject endScreen;

    public int score;
    public Text scoreText;
    public int movesLeft;
    public Text movesText;

    private Candy candyScript;
    // Start is called before the first frame update
    void Start()
    {
        allTiles = new BackgroundTile[x, y];
        allCandies = new GameObject[x, y];
    }

    public void TapToPlay()
    {
        startButton.SetActive(false);
        gameScreen.SetActive(true);
        SetUp();
    }

    public void SetUp()
    {
        for(int i=0;i<x;i++)
        {
            for(int j=0;j<y;j++)
            {
                GameObject backgroundTile = Instantiate(tilePrefab,new Vector2(i,j), Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "(" + i + "," + j + ")";

                int candyUse = Random.Range(0, candies.Length);
                int maxIterations = 0;
                while(MatchesAt(i,j,candies[candyUse])  && maxIterations < 100)
                {
                    candyUse = Random.Range(0, candies.Length);
                    maxIterations++;
                }

                maxIterations = 0;

                GameObject candy = Instantiate(candies[candyUse], new Vector2(i, j + offset), Quaternion.identity);
                candy.GetComponent<Candy>().row = j;
                candy.GetComponent<Candy>().column = i;
                candy.transform.parent = this.transform;
                candy.name = "(" + i + "," + j + ")";
                allCandies[i, j] = candy;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
       
    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if(column>1 && row>1)
        {
            if(allCandies[column - 1, row].tag == piece.tag && allCandies[column - 2, row].tag == piece.tag)
            {
                return true;
            }
            if (allCandies[column, row-1].tag == piece.tag && allCandies[column, row-2].tag == piece.tag)
            {
                return true;
            }
        }
        else if(column<=1 || row<=1)
        {
            if(row>1)
            {
                if (allCandies[column, row - 1].tag == piece.tag && allCandies[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (allCandies[column - 1, row].tag == piece.tag && allCandies[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void DestroyMatchesAt(int column, int row)
    {
        if(allCandies[column,row].GetComponent<Candy>().isMatched)
        {
            Destroy(allCandies[column, row]);
            allCandies[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        for(int i = 0; i<x; i++)
        {
            for(int j=0;j<y;j++)
            {
                if(allCandies[i,j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRowCoroutine());
    }

    private IEnumerator DecreaseRowCoroutine()
    {
        int nullCount = 0;
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                if (allCandies[i, j] == null)
                {
                    nullCount++;
                }
                else if(nullCount>0)
                {
                    allCandies[i, j].GetComponent<Candy>().row -= nullCount;
                    allCandies[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.2f);
        StartCoroutine(FillBoardCoroutine());
        if (moveCount >= 3)
        {
            endScreen.SetActive(true);
        }

    }

    private void RefillBoard()
    {
        for(int i=0; i<x; i++)
        {
            for(int j=0; j<y; j++)
            {
                if(allCandies[i,j] == null)
                {
                    Vector2 tempPos = new Vector2(i, j + offset);
                    int candyUse = Random.Range(0, candies.Length);
                    GameObject piece = Instantiate(candies[candyUse], tempPos, Quaternion.identity);
                    allCandies[i, j] = piece;
                    piece.GetComponent<Candy>().row = j;
                    piece.GetComponent<Candy>().column = i;
                }
            }
        }
    }
    private bool MatchesOnBoard()
    {
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                if (allCandies[i, j] != null)
                {
                    if (allCandies[i, j].GetComponent<Candy>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCoroutine()
    {
        RefillBoard();
        yield return new WaitForSeconds(.2f);

        while(MatchesOnBoard())
        {
            yield return new WaitForSeconds(.2f);
            DestroyMatches();
        }
    }
}
