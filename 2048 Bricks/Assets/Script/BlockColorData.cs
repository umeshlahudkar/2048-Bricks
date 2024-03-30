using UnityEngine;

[CreateAssetMenu( fileName = "BlockColorData", menuName = "SO/BlockColorData")]
public class BlockColorData : ScriptableObject
{
    [SerializeField] private BlockColor[] blockColors;

    public Color GetBlockColor(int number)
    {
        Color color = Color.white;

        for(int i = 0; i < blockColors.Length; i++)
        {
            if(blockColors[i].number == number)
            {
                color = blockColors[i].color;
                break;
            }
        }

        return color;
    }
}
