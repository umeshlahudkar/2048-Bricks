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
    protected int rowID;
    protected int columnID;

    private bool isEmpty;
    private int blockNumber;

    public int Row_ID { get { return rowID; } }

    public int Column_ID { get { return columnID; } }

    public RectTransform ThisRectTransform { get { return rectTransform; } }

    public bool IsEmpty { get { return isEmpty; } }
    
    public virtual void InitBlock(int row, int col, Vector3 pos, Vector2 size, string name)
    {
        rowID = row;
        columnID = col;
        rectTransform.localPosition = pos;
        rectTransform.sizeDelta = size;
        gameObject.name = name;
        isEmpty = true;
    }

    public void PlaceBlock(int number)
    {
        isEmpty = false;
        blockNumber = number;
        gameObject.SetActive(true);
    }
}
