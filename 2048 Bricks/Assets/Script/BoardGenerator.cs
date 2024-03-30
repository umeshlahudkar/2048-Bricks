using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    [Header("Board Data")]
    [SerializeField] private int rows;
    [SerializeField] private int colums;

    [Header("block Prefab")]
    [SerializeField] private Block blockPrefab;

    [Header("Board Canvas")]
    [SerializeField] private RectTransform canvasRect;

    public void GenerateBoard(GameplayController gameplayController)
    {
        float screenWidth = canvasRect.rect.width;
        float totalWidth = screenWidth * 0.80f;

        float totalSpace = totalWidth * 0.10f;
        float blockSpace = totalSpace / (colums + 1);

        float totalBlockSize = totalWidth - totalSpace;
        float blockSize = totalBlockSize / colums;

        float totalHeight = (blockSize * rows) + ((rows + 1) * blockSpace);

        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(totalWidth, totalHeight);

        gameplayController.InitGrid(rows, colums);

        float startX = GetStartPointX(blockSize, colums, blockSpace);
        float startY = GetStartPointY(blockSize, rows, blockSpace);

        float currentX = startX;
        float currentY = startY;

        Vector3 pos = Vector3.zero;
        Vector2 sizeDelta = Vector2.zero;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < colums; j++)
            {
                Block block = Instantiate(blockPrefab, transform);
                pos.x = currentX;
                pos.y = currentY;
                sizeDelta.x = sizeDelta.y = blockSize;
                block.InitBlock(i, j, pos, sizeDelta, "Block " + i + " " + j);

                gameplayController.blockGrid[i, j] = block;

                currentX = currentX + blockSize + blockSpace;
            }

            currentX = startX;
            currentY = currentY - (blockSize + blockSpace);
        }
    }

    private float GetStartPointX(float blockSize, int columnSize, float blockSpace)
    {
        float totalWidth = (blockSize * columnSize) + ((columnSize - 1) * blockSpace);
        return -((totalWidth / 2) - (blockSize / 2));
    }

    
    private float GetStartPointY(float blockSize, int rowSize, float blockSpace)
    {
        float totalHeight = (blockSize * rowSize) + ((rowSize - 1) * blockSpace);
        return ((totalHeight / 2) - (blockSize / 2));
    }
}
