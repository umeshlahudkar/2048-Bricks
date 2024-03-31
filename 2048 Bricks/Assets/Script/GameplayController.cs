using UnityEngine;

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

    private bool canMoveColumnAfterMerge = false;
    private bool canInput = true;

    private int[] moverBlockRotation = new int[] {0, 90, 180, 270};

    private GameState gameState = GameState.Waiting;

    private UIController uiController;
    private ScoreController scoreController;

    private Vector2 startPos;
    private float minSwipeDistance = 5f;


    public void StartGame(UIController _uIController, ScoreController _scoreController)
    {
        uiController = _uIController;
        scoreController = _scoreController;

        canInput = true;
        currentTimeDelta = normalTimeDelta;
        fastTimeDelta = normalTimeDelta / 4;

        boardGenerator.GenerateBoard(this);

        Block block = blockGrid[0, 2];
        moverBlock.InitBlock(block.Row_ID, block.Column_ID, block.ThisRectTransform.position, block.ThisRectTransform.sizeDelta);

        moverBlock.gameObject.SetActive(true);
        nextBlock.gameObject.SetActive(true);

        UpdateNextBlock();
        UpdateMoverBlock();

        gameState = GameState.Running;
    }

    public void SetGameState(GameState gameState)
    {
        this.gameState = gameState;
    }

    private int GetBlockNumber()
    {
        int exponent = Random.Range(1, 3);
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

    private void MoveMoverBlockAt(int row, int col)
    {
        if(IsValidIndex(row, col))
        {
            moverBlock.SetBlockIndex(row, col);
            moverBlock.ThisRectTransform.position = blockGrid[row, col].ThisRectTransform.position;
        }
    }

    private void Update()
    {
        if(gameState != GameState.Running) { return; }

        if (canInput)
        {
#if UNITY_WINDOWS
            HandleKeyboardInput();
#elif UNITY_ANDROID
            HandleTouchInput();
#endif
        }

        elapcedTime += Time.deltaTime;
        if(elapcedTime >= currentTimeDelta)
        {
            Move(MoveDirection.Down);
            elapcedTime = 0;
        }
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
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

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); 

            if (touch.phase == TouchPhase.Began)
            {
                startPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                Vector2 swipeDelta = touch.position - startPos;
                float swipeDistance = swipeDelta.magnitude;

                if (swipeDistance < minSwipeDistance)
                {
                    Rotate();
                }
                else
                {
                    if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                    {
                        if (swipeDelta.x > 0)
                        {
                            Move(MoveDirection.Right);
                        }
                        else
                        {
                            Move(MoveDirection.Left);
                        }
                    }
                    else
                    {
                        if (swipeDelta.y > 0)
                        {
                            Rotate();
                        }
                        else
                        {
                            currentTimeDelta = fastTimeDelta;
                        }
                    }
                }
            }
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

                if(IsValidIndex(row, col - 1) && blockGrid[row, col -1].IsEmpty)
                {
                    MoveMoverBlockAt(row, col - 1);
                }
                break;

            case MoveDirection.Right:

                if (IsValidIndex(row, col + 1) && blockGrid[row, col + 1].IsEmpty)
                {
                    MoveMoverBlockAt(row, col + 1);
                }
                break;

            case MoveDirection.Down:

                if (!canMoveColumnAfterMerge && IsValidIndex(row + 1, col) && blockGrid[row + 1, col].IsEmpty)
                {
                    MoveMoverBlockAt(row + 1, col);
                }
                else
                {
                    // place block
                    canInput = false;
                    currentTimeDelta = normalTimeDelta;

                    if (canMoveColumnAfterMerge)
                    {
                        if (IsValidIndex(row - 1, col + 1) && !blockGrid[row - 1, col + 1].IsEmpty)
                        {
                            for (int i = row; i >= 0; i--)
                            {
                                if (IsValidIndex(i, col + 1) && blockGrid[i, col + 1].IsEmpty && IsValidIndex(i - 1, col + 1) && !blockGrid[i - 1, col + 1].IsEmpty)
                                {
                                    blockGrid[i, col + 1].PlaceBlock(blockGrid[i - 1, col + 1].BlockNumber, blockGrid[i - 1, col + 1].BlockColor, blockGrid[i - 1, col + 1].RotationAngle);
                                    blockGrid[i - 1, col + 1].ResetBlock();
                                }
                            }
                        }

                        if (IsValidIndex(row - 1, col - 1) && !blockGrid[row - 1, col - 1].IsEmpty)
                        {
                            for (int i = row; i >= 0; i--)
                            {
                                if (IsValidIndex(i, col - 1) && blockGrid[i, col - 1].IsEmpty && IsValidIndex(i - 1, col - 1) && !blockGrid[i - 1, col - 1].IsEmpty)
                                {
                                    blockGrid[i, col - 1].PlaceBlock(blockGrid[i - 1, col - 1].BlockNumber, blockGrid[i - 1, col - 1].BlockColor, blockGrid[i - 1, col - 1].RotationAngle);
                                    blockGrid[i - 1, col - 1].ResetBlock();
                                }
                            }
                        }

                        canMoveColumnAfterMerge = false;
                    }
                    else if(CanMoverBlockMerge())
                    {
                        int newNumber = moverBlock.BlockNumber;

                        // vertical down
                        if (CanMoverBlockMergeWith(row + 1, col))
                        {
                            newNumber += blockGrid[row + 1, col].BlockNumber;
                            blockGrid[row + 1, col].ResetBlock();
                        }

                        // horizontal right
                        if (CanMoverBlockMergeWith(row, col + 1))
                        {
                            newNumber += blockGrid[row, col + 1].BlockNumber;
                            blockGrid[row, col + 1].ResetBlock();

                            // checks for - after merge if any blocks are in coloumn of the merged blocks
                            if(IsValidIndex(row - 1, col + 1) && !blockGrid[row - 1, col + 1].IsEmpty)
                            {
                                canMoveColumnAfterMerge = true;
                            }
                        }

                        // horizontal left
                        if (CanMoverBlockMergeWith(row, col - 1))
                        {
                            newNumber += blockGrid[row, col - 1].BlockNumber;
                            blockGrid[row, col - 1].ResetBlock();

                            // checks for - after merge if any blocks are in coloumn of the merged blocks
                            if (IsValidIndex(row - 1, col - 1) && !blockGrid[row - 1, col - 1].IsEmpty)
                            {
                                canMoveColumnAfterMerge = true;
                            }
                        }

                        moverBlock.UpdateBlock(newNumber, blockColorData.GetBlockColor(newNumber), moverBlock.RotationAngle);

                        scoreController.AddScore(newNumber);
                    }
                    else
                    {
                        blockGrid[row, col].PlaceBlock(moverBlock.BlockNumber, moverBlock.BlockColor, moverBlock.RotationAngle);

                        if(!IsGameOver())
                        {
                            MoveMoverBlockAt(0, 2);

                            UpdateMoverBlock();
                            UpdateNextBlock();
                            canInput = true;
                        }
                        else
                        {
                            uiController.OpenGameOverScreen();
                        }
                        
                    }
                }
                break;
        }
    }

    private bool IsGameOver()
    {
        return !blockGrid[0, 2].IsEmpty;
    }

    private bool CanMoverBlockMerge()
    {
        int row = moverBlock.Row_ID;
        int col = moverBlock.Column_ID;
       
        return CanMoverBlockMergeWith(row + 1, col) ||
           CanMoverBlockMergeWith(row, col + 1) ||
           CanMoverBlockMergeWith(row, col - 1);
    }

    private bool CanMoverBlockMergeWith(int row, int col)
    {
        return (IsValidIndex(row, col) && !blockGrid[row, col].IsEmpty &&
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

    private bool IsValidIndex(int row, int col)
    {
        return (row >= 0 && row < this.gridRows && col >= 0 && col < gridColoumns);
    }

    public void ResetGameplay()
    {
        if(blockGrid != null)
        {
            for(int i = 0; i < gridRows; i++)
            {
                for(int j = 0; j < gridColoumns; j++)
                {
                    Destroy(blockGrid[i, j].gameObject);
                }
            }

            blockGrid = null;
        }

        gameState = GameState.Waiting;
        currentTimeDelta = 0;
        elapcedTime = 0;

        canInput = false;
        canMoveColumnAfterMerge = false;

        nextBlock.ResetBlock();
        moverBlock.ResetBlock();
    }
   
}


public enum MoveDirection
{
    None = 0,
    Left,
    Right,
    Down
}

public enum GameState
{
    Waiting,
    Running,
    Paused,
    GameOver
}
