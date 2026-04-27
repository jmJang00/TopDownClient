using UnityEngine;

public class GridMap
{
    public int width;
    public int height;
    public byte[,] data; // 0: empty, 1: wall
    public int tileSize = 1;

    public GridMap(int w, int h)
    {
        width = w;
        height = h;
        data = new byte[h, w];
    }

    public GridMap(byte[,] d)
    {
        height = d.GetLength(0);
        width = d.GetLength(1);
        data = new byte[height, width];
        System.Buffer.BlockCopy(d, 0, data, 0, height * width);
    }

    public GridMap(GridMapSO so)
    {
        width = so.width;
        height = so.height;
        data = new byte[height, width];
        tileSize = so.tileSize;
        System.Buffer.BlockCopy(so.data, 0, data, 0, height * width);
    }

    public bool IsBlocked(int x, int z)
    {
        x /= tileSize;
        z /= tileSize;

        if (x < 0 || z < 0 || x >= width || z >= height)
            return true;

        return data[z, x] == 0;
    }
}
