using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Candy : MonoBehaviour
{
    private Vector2 firstTouchPos;
    private Vector2 finalTouchPos;
    public float swipeAngle = 0;
    public float swipeResist = 1f;

    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    private Vector2 tempPosition;
    private GameObject otherCandy;
    private Board board;

    public bool isMatched = false;


    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        //targetX = (int)transform.position.x;
        //targetY = (int)transform.position.y;
        //column = targetX;
        //row = targetY;
        //previousRow = row;
        //previousColumn = column;
    }

    // Update is called once per frame
    void Update()
    {

        FindMatches();
        if(isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(0f, 0f, 0f, 0.2f);
        }

        targetX = column;
        targetY = row;

        if(Mathf.Abs(targetX - transform.position.x) > .1)
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .05f);
            if(board.allCandies[column, row] != this.gameObject)
            {
                board.allCandies[column, row] = this.gameObject;
            }
        }
        else
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;

        }
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .05f);
            if (board.allCandies[column, row] != this.gameObject)
            {
                board.allCandies[column, row] = this.gameObject;
            }
        }
        else
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;

        }
    }
    private void OnMouseDown()
    {
        firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log("Clicked");
    }
    private void OnMouseUp()
    {
        finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log("Released");
        CalculateAngle();
    }
    void CalculateAngle()
    {
        if(Mathf.Abs(finalTouchPos.y - firstTouchPos.y) > swipeResist || Mathf.Abs(finalTouchPos.x - firstTouchPos.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPos.y - firstTouchPos.y, finalTouchPos.x - firstTouchPos.x) * 180 / Mathf.PI;
            Debug.Log(swipeAngle);
            MovePieces();
            board.moveCount++;
            board.movesLeft = 3 - board.moveCount;
            board.movesText.text = "Moves Left : " + board.movesLeft;
        }

    }

    void MovePieces()
    {
        if(swipeAngle>-45 && swipeAngle<=45 && column < board.x-1)
        {
            otherCandy = board.allCandies[column + 1, row];
            previousRow = row;
            previousColumn = column;
            otherCandy.GetComponent<Candy>().column -= 1;
            column += 1;
        }
        else if(swipeAngle > 45 && swipeAngle <= 135 && row<board.y-1)
        {
            otherCandy = board.allCandies[column, row + 1];
            previousRow = row;
            previousColumn = column;
            otherCandy.GetComponent<Candy>().row -= 1;
            row += 1;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            otherCandy = board.allCandies[column - 1, row];
            previousRow = row;
            previousColumn = column;
            otherCandy.GetComponent<Candy>().column += 1;
            column -= 1;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            otherCandy = board.allCandies[column, row-1];
            previousRow = row;
            previousColumn = column;
            otherCandy.GetComponent<Candy>().row += 1;
            row-= 1;
        }
        StartCoroutine(CheckMoveCoroutine());
    }

    public IEnumerator CheckMoveCoroutine()
    {
        yield return new WaitForSeconds(.5f);
        if(otherCandy != null)
        {
            if(!isMatched && !otherCandy.GetComponent<Candy>().isMatched)
            {
                otherCandy.GetComponent<Candy>().row = row;
                otherCandy.GetComponent<Candy>().column = column;
                row = previousRow;
                column = previousColumn;
            }
            else
            {
                board.DestroyMatches();
                board.score += 10;
                board.scoreText.text = "Score : " + board.score;
            }
            otherCandy = null;
        }
    
    }
    void FindMatches()
    {
        if(column > 0 && column < board.x-1)
        {
            GameObject leftCandy1 = board.allCandies[column - 1, row];
            GameObject rightCandy1 = board.allCandies[column +1, row];
            if(leftCandy1!= null && rightCandy1!=null)
            {
                if (leftCandy1.gameObject.tag == this.gameObject.tag && rightCandy1.gameObject.tag == this.gameObject.tag)
                {
                    leftCandy1.GetComponent<Candy>().isMatched = true;
                    rightCandy1.GetComponent<Candy>().isMatched = true;
                    isMatched = true;
                }
            }
        }
        if (row > 0 && row < board.y - 1)
        {
            GameObject downCandy1 = board.allCandies[column , row - 1];
            GameObject upCandy1 = board.allCandies[column, row + 1];
            if(downCandy1!=null && upCandy1!=null)
            {
                if (downCandy1.gameObject.tag == this.gameObject.tag && upCandy1.gameObject.tag == this.gameObject.tag)
                {
                    downCandy1.GetComponent<Candy>().isMatched = true;
                    upCandy1.GetComponent<Candy>().isMatched = true;
                    isMatched = true;
                }
            }

        }
    }
}
