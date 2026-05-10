using UnityEngine;

[CreateAssetMenu(fileName = "GridMapSO", menuName = "Scriptable Objects/GridMapSO")]
public class GridMapSO : ScriptableObject
{
    public int width;
    public int height;

    // 0 = empty, 1 = blocked
    public byte[] data;

    public int tileSize;

    public void Copy(GridMap map)
    {
        width = map.width;
        height = map.height;
        data = new byte[height * width];
        System.Buffer.BlockCopy(map.data, 0, data, 0, height * width);
    }
}
