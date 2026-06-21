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
    public void Draw()
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
#endif
}
