using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;

public class Block : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI numbetText;
    [SerializeField] private Image blockImg;
    [SerializeField] private RectTransform rectTransform;
    private int rowID;
    private int columnID;

    private bool isEmpty;
    private int blockNumber;
    private int rotationAngle;

    private Vector3 initialPos;

    private Color blockColor;

    public int Row_ID { get { return rowID; } }

    public int Column_ID { get { return columnID; } }

    public RectTransform ThisRectTransform { get { return rectTransform; } }

    public bool IsEmpty { get { return isEmpty; } }

    public int BlockNumber { get { return blockNumber; } }

    public int RotationAngle { get { return rotationAngle; } }

    public Color BlockColor { get { return blockColor; } }


    /// <summary>
    /// gets called for the grid blocks
    /// </summary>
    
    public void InitBlock(int row, int col, Vector3 pos, Vector2 size, string name)
    {
        rowID = row;
        columnID = col;
        rectTransform.localPosition = pos;
        rectTransform.sizeDelta = size;
        gameObject.name = name;
        isEmpty = true;
        initialPos = rectTransform.position;
    }

    /// <summary>
    /// gets called for the mover block and Next block
    /// </summary>
    public void InitBlock(int row, int col, Vector3 pos, Vector2 size)
    {
        rowID = row;
        columnID = col;
        rectTransform.position = pos;
        rectTransform.sizeDelta = size;
        initialPos = rectTransform.position;

        gameObject.SetActive(true);
    }

    /// <summary>
    /// assigns block number, color and rotation to this block
    /// </summary>
    public void PlaceBlock(int number, Color color, int rotation)
    {
        isEmpty = false;
        blockNumber = number;
        numbetText.text = number.ToString();
        blockColor = color;
        blockImg.color = blockColor;
        Rotate(rotation, false);
        gameObject.SetActive(true);
    }

    /// <summary>
    /// moves the placeble block from it's current position to this block and copies the number and color
    /// </summary>
    /// <param name="blockToBePlaced"> the blocks whose number, color gets copied to this block </param>
    public void PlaceBlock(Block blockToBePlaced)
    {
        blockToBePlaced.MoveAt(rectTransform.position, () =>
        {
            PlaceBlock(blockToBePlaced.blockNumber, blockToBePlaced.blockColor, blockToBePlaced.rotationAngle);
            blockToBePlaced.ResetBlock();
        });
    }

    /// <summary>
    /// Updates block number, color and rotation
    /// </summary>
    public void UpdateBlock(int number, Color color, int rotation = 0)
    {
        blockNumber = number;
        blockColor = color;

        numbetText.text = blockNumber.ToString();
        blockImg.color = blockColor;

        Rotate(rotation, false);
    }

    public void Rotate(int rotation, bool canPlayRotateEffect = true)
    {
        if(rotation >= 360) { rotation = 0; }
        rotationAngle = rotation;

        if(canPlayRotateEffect)
        {
            rectTransform.DORotate(new Vector3(0, 0, rotationAngle), 0.15f);
        }
        else
        {
            rectTransform.eulerAngles = new Vector3(0, 0, rotationAngle);
        }
    }

    public void MoveAt(Vector3 pos, UnityAction onMovedCallback = null)
    {
        rectTransform.DOMove(pos, 0.15f).OnComplete(() => 
        {
            rectTransform.position = initialPos;
            onMovedCallback?.Invoke();
        });
    }

    public void ResetBlock()
    {
        gameObject.SetActive(false);
        isEmpty = true;
        blockNumber = 0;
        blockColor = Color.white;
        rotationAngle = 0;
    }

    public void SetBlockIndex(int row, int col)
    {
        rowID = row;
        columnID = col;
    }
}
