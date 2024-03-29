using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    public static GameplayController Instance;

    private void Awake()
    {
        Instance = this;
    }

    private int gridRows;
    private int gridColoumns;
    public Block[,] blockGrid;

    public MoverBlock moverBlock;

    [SerializeField] private float normalTimeDelta;
    private float fastTimeDelta;
    private float currentTimeDelta;
    private float elapcedTime;

    public BoardGenerator boardGenerator;



    private void Start()
    {
        currentTimeDelta = normalTimeDelta;
        fastTimeDelta = normalTimeDelta / 4;

        boardGenerator.GenerateBoard(this);

        Block block = blockGrid[0, 2];
        moverBlock.InitBlock(block.Row_ID, block.Column_ID, block.ThisRectTransform.position, block.ThisRectTransform.sizeDelta, string.Empty);
    }

    public void InitGrid(int row, int col)
    {
        this.gridRows = row;
        this.gridColoumns = col;
        blockGrid = new Block[row, col];
    }

    private void SetMoverBlockPos(int row, int col)
    {
        if(IsValid(row, col))
        {
            moverBlock.SetBlockIndex(row, col);
            moverBlock.ThisRectTransform.position = blockGrid[row, col].ThisRectTransform.position;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(MoveDirection.Left);
        }
        else if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(MoveDirection.Right);
        }
        else if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentTimeDelta = fastTimeDelta;
        }

        elapcedTime += Time.deltaTime;
        if(elapcedTime >= currentTimeDelta)
        {
            Move(MoveDirection.Down);
            elapcedTime = 0;
        }
    }

    private void Move(MoveDirection moveDirection)
    {
        int row = moverBlock.Row_ID;
        int col = moverBlock.Column_ID;

        switch (moveDirection)
        {
            case MoveDirection.Left:

                if( IsValid(row, col - 1) && blockGrid[row, col -1].IsEmpty)
                {
                    SetMoverBlockPos(row, col - 1);
                }
                break;

            case MoveDirection.Right:

                if (IsValid(row, col + 1) && blockGrid[row, col + 1].IsEmpty)
                {
                    SetMoverBlockPos(row, col + 1);
                }
                break;

            case MoveDirection.Down:

                if (IsValid(row + 1, col) && blockGrid[row + 1, col].IsEmpty)
                {
                    SetMoverBlockPos(row + 1, col);
                }
                else
                {
                    // place block
                    blockGrid[row, col].PlaceBlock(000);
                    SetMoverBlockPos(0, 2);
                    currentTimeDelta = normalTimeDelta;
                }
                break;
        }
    }

    private bool IsValid(int row, int col)
    {
        return (row >= 0 && row < this.gridRows && col >= 0 && col < gridColoumns);
    }
   
}


public enum MoveDirection
{
    None = 0,
    Left,
    Right,
    Down
}
