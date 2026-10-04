using System.IO;
using UnityEngine;

[System.Serializable]
public struct WaypointNode
{
    public Vector2 position;

    public int firstLink;
    public int linkCount;
}

[CreateAssetMenu(fileName = "GridMapSO", menuName = "Scriptable Objects/GridMapSO")]
public class GridMapSO : ScriptableObject
{
    public int width;
    public int height;

    // 0 = empty, 1 = blocked
    public byte[] data;

    public int tileSize;

    public WaypointNode[] waypoints;
    public int[] links;
    public float linkDistance;

    public void Copy(GridMap map)
    {
        width = map.width;
        height = map.height;
        data = new byte[height * width];
        System.Buffer.BlockCopy(map.data, 0, data, 0, height * width);
    }

#if UNITY_EDITOR
    public void DrawWaypoints()
    {
        if (waypoints == null || links == null)
        {
            return;
        }

        UnityEditor.Handles.color = Color.white;

        for (int i = 0; i < waypoints.Length; ++i)
        {
            WaypointNode node = waypoints[i];

            Vector3 from = new Vector3(node.position.x, 0, node.position.y);

            for (int j = 0; j < node.linkCount; ++j)
            {
                int next = links[node.firstLink + j];

                if (next < i)
                    continue;

                Vector2 pos = waypoints[next].position;

                Vector3 to = new Vector3(pos.x, 0, pos.y);

                UnityEditor.Handles.DrawLine(from, to);
            }
        }

        UnityEditor.Handles.color = Color.green;

        for (int i = 0; i < waypoints.Length; ++i)
        {
            Vector2 p = waypoints[i].position;

            Vector3 pos = new Vector3( p.x, 0, p.y);

            UnityEditor.Handles.SphereHandleCap(0, pos, Quaternion.identity, 0.3f, EventType.Repaint);

            UnityEditor.Handles.Label(pos + Vector3.up * 0.2f, i.ToString());
        }
    }

    public void DrawGrid()
    {
        if (width <= 0 || height <= 0 || tileSize <= 0)
            return;

        float mapWidth = width * tileSize;
        float mapHeight = height * tileSize;

        // Blocked Cell
        if (data != null && data.Length == width * height)
        {
            Color fillColor = new Color(1.0f, 0.2f, 0.2f, 0.20f);
            Color outlineColor = new Color(1.0f, 0.2f, 0.2f, 0.6f);

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int index = y * width + x;

                    if (data[index] != 1)
                        continue;

                    float minX = x * tileSize;
                    float minZ = y * tileSize;
                    float maxX = minX + tileSize;
                    float maxZ = minZ + tileSize;

                    Vector3[] vertices =
                    {
                    new Vector3(minX, 0.01f, minZ),
                    new Vector3(minX, 0.01f, maxZ),
                    new Vector3(maxX, 0.01f, maxZ),
                    new Vector3(maxX, 0.01f, minZ),
                };

                    UnityEditor.Handles.DrawSolidRectangleWithOutline(
                        vertices,
                        fillColor,
                        outlineColor);
                }
            }
        }

        // Grid Line
        UnityEditor.Handles.color = new Color(0.3f, 0.8f, 1.0f, 0.7f);

        for (int x = 0; x <= width; ++x)
        {
            float px = x * tileSize;

            UnityEditor.Handles.DrawLine(
                new Vector3(px, 0.02f, 0.0f),
                new Vector3(px, 0.02f, mapHeight));
        }

        for (int y = 0; y <= height; ++y)
        {
            float pz = y * tileSize;

            UnityEditor.Handles.DrawLine(
                new Vector3(0.0f, 0.02f, pz),
                new Vector3(mapWidth, 0.02f, pz));
        }
    }
#endif
}
