using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    [Header("Board Data")]
    [SerializeField] private int rows;
    [SerializeField] private int colums;

    [SerializeField] private Block blockPrefab;

    [Header("Board Canvas")]
    [SerializeField] private RectTransform canvasRect;

    [Header("Background")]
    [SerializeField] private RectTransform bg;

    public void GenerateBoard(GameplayController gameplayController)
    {
        float screenWidth = canvasRect.rect.width;
        float totalWidth = screenWidth * 0.90f;

        float totalSpace = totalWidth * 0.10f;
        float blockSpace = totalSpace / (colums + 1);

        float totalBlockSize = totalWidth - totalSpace;
        float blockSize = totalBlockSize / colums;

        float totalHeight = (blockSize * rows) + ((rows + 1) * blockSpace);

        bg.sizeDelta = new Vector2(totalWidth, totalHeight);

        GameplayController.Instance.InitGrid(rows, colums);

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
                //block.gameObject.name = "Block " + i + " " + j;
                //block.gameObject.SetActive(true);

                pos.x = currentX;
                pos.y = currentY;

                sizeDelta.x = sizeDelta.y = blockSize;

                //block.ThisRectTransform.localPosition = new Vector3(currentX, currentY, 0);
                //block.ThisRectTransform.sizeDelta = new Vector2(blockSize, blockSize);
                //block.SetBlock(i, j);

                block.InitBlock(i, j, pos, sizeDelta, "Block " + i + " " + j);

                GameplayController.Instance.blockGrid[i, j] = block;

                currentX = currentX + blockSize + blockSpace;
            }

            currentX = startX;
            currentY = currentY - (blockSize + blockSpace);
        }
    }

    public float GetStartPointX(float blockSize, int columnSize, float blockSpace)
    {
        float totalWidth = (blockSize * columnSize) + ((columnSize - 1) * blockSpace);
        return -((totalWidth / 2) - (blockSize / 2));
    }

    
    public float GetStartPointY(float blockSize, int rowSize, float blockSpace)
    {
        float totalHeight = (blockSize * rowSize) + ((rowSize - 1) * blockSpace);
        return ((totalHeight / 2) - (blockSize / 2));
    }
}
