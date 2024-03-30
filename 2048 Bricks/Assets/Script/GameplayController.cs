using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameplayController : MonoBehaviour
{
    [SerializeField] private Block moverBlock;
    [SerializeField] private Block nextBlock;

    [SerializeField] private BoardGenerator boardGenerator;
    [SerializeField] private BlockColorData blockColorData;

    private int gridRows;
    private int gridColoumns;
    public Block[,] blockGrid;

    [SerializeField] private float normalTimeDelta;
    private float fastTimeDelta;
    private float currentTimeDelta;
    private float elapcedTime;

    private bool needToReadjustBlockColumn = false;
    private bool canMove = true;

    private int[] moverBlockRotation = new int[] {0, 90, 180, 270};

    public delegate void BlockMerge(int newNumber);
    public static BlockMerge OnBlockMerge;

    private void Start()
    {
        currentTimeDelta = normalTimeDelta;
        fastTimeDelta = normalTimeDelta / 4;

        boardGenerator.GenerateBoard(this);

        Block block = blockGrid[0, 2];
        moverBlock.InitBlock(block.Row_ID, block.Column_ID, block.ThisRectTransform.position, block.ThisRectTransform.sizeDelta);

        UpdateNextBlock();
        UpdateMoverBlock();
    }

    private int GetBlockNumber()
    {
        int exponent = Random.Range(1, 7);
        return (int)Mathf.Pow(2, exponent);
    }

    private int GetBlockRotation()
    {
        return moverBlockRotation[Random.Range(0, moverBlockRotation.Length)];
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
            moverBlock.SetBlockIndexIDs(row, col);
            moverBlock.ThisRectTransform.position = blockGrid[row, col].ThisRectTransform.position;
        }
    }

    private void Update()
    {
        if(canMove)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                Rotate();
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Move(MoveDirection.Left);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                Move(MoveDirection.Right);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentTimeDelta = fastTimeDelta;
            }
        }

        elapcedTime += Time.deltaTime;
        if(elapcedTime >= currentTimeDelta)
        {
            Move(MoveDirection.Down);
            elapcedTime = 0;
        }
    }

    private void Rotate()
    {
        int angle = moverBlock.RotationAngle + 90;
        moverBlock.Rotate(angle);
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
                    canMove = false;
                    if(needToReadjustBlockColumn)
                    {
                        if (IsValid(row - 1, col + 1) && !blockGrid[row - 1, col + 1].IsEmpty)
                        {
                            for (int i = row; i >= 0; i--)
                            {
                                if (IsValid(i, col + 1) && blockGrid[i, col + 1].IsEmpty && IsValid(i - 1, col + 1) && !blockGrid[i - 1, col + 1].IsEmpty)
                                {
                                    blockGrid[i, col + 1].PlaceBlock(blockGrid[i - 1, col + 1].BlockNumber, blockGrid[i - 1, col + 1].BlockColor, blockGrid[i - 1, col + 1].RotationAngle);
                                    blockGrid[i - 1, col + 1].ResetBlock();
                                }
                            }
                        }

                        if (IsValid(row - 1, col - 1) && !blockGrid[row - 1, col - 1].IsEmpty)
                        {
                            for (int i = row; i >= 0; i--)
                            {
                                if (IsValid(i, col - 1) && blockGrid[i, col - 1].IsEmpty && IsValid(i - 1, col - 1) && !blockGrid[i - 1, col - 1].IsEmpty)
                                {
                                    blockGrid[i, col - 1].PlaceBlock(blockGrid[i - 1, col - 1].BlockNumber, blockGrid[i - 1, col - 1].BlockColor, blockGrid[i - 1, col - 1].RotationAngle);
                                    blockGrid[i - 1, col - 1].ResetBlock();
                                }
                            }
                        }

                        needToReadjustBlockColumn = false;
                    }
                    else if(CanMergeBlocks())
                    {
                        int newNumber = moverBlock.BlockNumber;

                        // vertical down
                        //if (IsValid(row + 1, col) && !blockGrid[row + 1, col].IsEmpty && blockGrid[row + 1, col].BlockNumber == moverBlock.BlockNumber)
                        if (CanMergeWithMoverBlock(row + 1, col))
                        {
                            newNumber += blockGrid[row + 1, col].BlockNumber;
                            blockGrid[row + 1, col].ResetBlock();
                        }

                        // horizontal right
                        //if (IsValid(row, col + 1) && !blockGrid[row, col + 1].IsEmpty && blockGrid[row, col + 1].BlockNumber == moverBlock.BlockNumber)
                        if (CanMergeWithMoverBlock(row, col + 1))
                        {
                            newNumber += blockGrid[row, col + 1].BlockNumber;
                            blockGrid[row, col + 1].ResetBlock();

                            if(IsValid(row - 1, col + 1) && !blockGrid[row - 1, col + 1].IsEmpty)
                            {
                                needToReadjustBlockColumn = true;
                            }
                        }

                        // horizontal left
                        //if (IsValid(row, col - 1) && !blockGrid[row, col - 1].IsEmpty && blockGrid[row, col - 1].BlockNumber == moverBlock.BlockNumber)
                        if (CanMergeWithMoverBlock(row, col - 1))
                        {
                            newNumber += blockGrid[row, col - 1].BlockNumber;
                            blockGrid[row, col - 1].ResetBlock();

                            if (IsValid(row - 1, col - 1) && !blockGrid[row - 1, col - 1].IsEmpty)
                            {
                                needToReadjustBlockColumn = true;
                            }
                        }

                        moverBlock.UpdateBlock(newNumber, blockColorData.GetBlockColor(newNumber), moverBlock.RotationAngle);
                        
                        if(OnBlockMerge != null)
                        {
                            OnBlockMerge.Invoke(newNumber);
                        }
                    }
                    else
                    {
                        canMove = true;
                        blockGrid[row, col].PlaceBlock(moverBlock.BlockNumber, moverBlock.BlockColor, moverBlock.RotationAngle);
                        SetMoverBlockPos(0, 2);
                        currentTimeDelta = normalTimeDelta;

                        UpdateMoverBlock();
                        UpdateNextBlock();
                    }
                }
                break;
        }
    }

    private bool CanMergeBlocks()
    {
        int row = moverBlock.Row_ID;
        int col = moverBlock.Column_ID;

        bool canMerge = false;

        //if(IsValid(row + 1, col) && !blockGrid[row + 1, col].IsEmpty && blockGrid[row + 1, col].BlockNumber == moverBlock.BlockNumber)
        if(CanMergeWithMoverBlock(row+1, col))
        {
            canMerge |= true;
        }

        //if (IsValid(row, col + 1) && !blockGrid[row, col + 1].IsEmpty && blockGrid[row, col + 1].BlockNumber == moverBlock.BlockNumber)
        if (CanMergeWithMoverBlock(row, col + 1))
        {
            canMerge |= true;
        }

        //if (IsValid(row, col - 1) && !blockGrid[row, col - 1].IsEmpty && blockGrid[row, col - 1].BlockNumber == moverBlock.BlockNumber)
        if (CanMergeWithMoverBlock(row, col - 1))
        {
            canMerge |= true;
        }

        return canMerge;
    }

    private bool CanMergeWithMoverBlock(int row, int col)
    {
        return (IsValid(row, col) && !blockGrid[row, col].IsEmpty &&
             blockGrid[row, col].BlockNumber == moverBlock.BlockNumber && blockGrid[row, col].RotationAngle == moverBlock.RotationAngle);
    }

    private void UpdateMoverBlock()
    {
        moverBlock.UpdateBlock(nextBlock.BlockNumber, nextBlock.BlockColor, nextBlock.RotationAngle);
    }

    private void UpdateNextBlock()
    {
        int nextNumber = GetBlockNumber();
        int angle = GetBlockRotation();
        nextBlock.UpdateBlock(nextNumber, blockColorData.GetBlockColor(nextNumber), angle);
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
