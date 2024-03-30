using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Block : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI numbetText;
    [SerializeField] private Image blockImg;
    [SerializeField] private RectTransform rectTransform;
    private int rowID;
    private int columnID;

    private bool isEmpty;
    private int blockNumber;

    private Color blockColor;

    public int Row_ID { get { return rowID; } }

    public int Column_ID { get { return columnID; } }

    public RectTransform ThisRectTransform { get { return rectTransform; } }

    public bool IsEmpty { get { return isEmpty; } }

    public int BlockNumber { get { return blockNumber; } }

    public Color BlockColor { get { return blockColor; } }

    public void InitBlock(int row, int col, Vector3 pos, Vector2 size, string name)
    {
        rowID = row;
        columnID = col;
        rectTransform.localPosition = pos;
        rectTransform.sizeDelta = size;
        gameObject.name = name;
        isEmpty = true;
    }

    public void InitBlock(int row, int col, Vector3 pos, Vector2 size)
    {
        rowID = row;
        columnID = col;
        rectTransform.position = pos;
        rectTransform.sizeDelta = size;
    }

    public void PlaceBlock(int number, Color color)
    {
        isEmpty = false;
        blockNumber = number;
        numbetText.text = number.ToString();
        blockColor = color;
        blockImg.color = blockColor;
        gameObject.SetActive(true);
    }

    public void UpdateBlock(int number, Color color)
    {
        blockNumber = number;
        blockColor = color;

        numbetText.text = blockNumber.ToString();
        blockImg.color = blockColor;
    }

    public void ResetBlock()
    {
        gameObject.SetActive(false);
        isEmpty = true;
        blockNumber = 0;
        blockColor = Color.white;
    }

    public void SetBlockIndexIDs(int row, int col)
    {
        rowID = row;
        columnID = col;
    }
}
