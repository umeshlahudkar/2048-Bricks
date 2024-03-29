using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverBlock : Block
{
    public override void InitBlock(int row, int col, Vector3 pos, Vector2 size, string name)
    {
        rowID = row;
        columnID = col;
        ThisRectTransform.position = pos;
        ThisRectTransform.sizeDelta = size;
    }

    public void SetBlockIndex(int row, int col)
    {
        rowID = row;
        columnID = col;
    }
}
