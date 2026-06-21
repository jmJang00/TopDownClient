using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(GridMapSO))]
public class GridMapSOEditor : Editor
{
    const int CELL_SIZE = 12;

    static readonly Color[] COLORS =
    {
        Color.white,          // 0
        Color.black,          // 1
        Color.green,          // 2
        Color.cyan,           // 3
    };

    byte selectedValue = 1;

    public override void OnInspectorGUI()
    {
        GridMapSO so = (GridMapSO)target;

        DrawDefaultInspector();

        GUILayout.Space(10);

        DrawPalette();

        GUILayout.Space(10);

        if (GUILayout.Button("Export Binary"))
        {
            ExportBinary(so);
        }

        if (GUILayout.Button("Build Links"))
        {
            BuildLinks(so);

            EditorUtility.SetDirty(so);
        }

        if (so.data == null || so.data.Length != so.width * so.height)
            return;

        GUILayout.Space(10);

        DrawGrid(so);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(so);
        }
    }

    void DrawPalette()
    {
        GUILayout.Label("Brush");

        EditorGUILayout.BeginHorizontal();

        for (byte i = 0; i < COLORS.Length; ++i)
        {
            GUI.backgroundColor = COLORS[i];

            bool clicked = GUILayout.Toggle(
                selectedValue == i,
                i.ToString(),
                "Button",
                GUILayout.Width(40),
                GUILayout.Height(25));

            if (clicked)
                selectedValue = i;
        }

        GUI.backgroundColor = Color.white;

        EditorGUILayout.EndHorizontal();

        GUILayout.Label("0=Empty  1=Blocked  2=Waypoint  3=Spawn");
    }

    void DrawGrid(GridMapSO so)
    {
        Event e = Event.current;

        for (int z = 0; z < so.height; z++)
        {
            Rect rowRect = EditorGUILayout.GetControlRect(false, CELL_SIZE);

            for (int x = 0; x < so.width; x++)
            {
                Rect cell = new Rect(
                    rowRect.x + x * CELL_SIZE,
                    rowRect.y,
                    CELL_SIZE,
                    CELL_SIZE);

                int index = z * so.width + x;

                byte val = so.data[index];

                Color color = val < COLORS.Length ? COLORS[val] : Color.magenta;

                EditorGUI.DrawRect(cell, color);

                EditorGUI.DrawRect(new Rect(cell.xMax - 1, cell.y, 1, cell.height), Color.gray);

                EditorGUI.DrawRect(new Rect(cell.x, cell.yMax - 1, cell.width, 1), Color.gray);

                bool pressed =
                    (e.type == EventType.MouseDown ||
                     e.type == EventType.MouseDrag) &&
                    cell.Contains(e.mousePosition);

                if (pressed && e.button == 0)
                {
                    so.data[index] = selectedValue;

                    GUI.changed = true;
                    e.Use();
                }

                if (pressed && e.button == 1)
                {
                    so.data[index] = 0;

                    GUI.changed = true;
                    e.Use();
                }
            }
        }
    }


    static bool IsBlocked(byte[] fineGrid, int width, int height, int x, int y)
    {
        if (x < 0 || y < 0 || x >= width || y >= height)
        {
            return true;
        }

        return fineGrid[y * width + x] == 1;
    }

    public static bool HasLineOfSight(byte[] grid, int width, int height, float startX, float startY, float endX, float endY)
    {
        int x = Mathf.FloorToInt(startX);
        int y = Mathf.FloorToInt(startY);

        if (IsBlocked(grid, width, height, x, y))
            return false;

        float dx = endX - startX;
        float dy = endY - startY;

        if (dx == 0.0f && dy == 0.0f)
            return true;

        int stepX = Math.Sign(dx);
        int stepY = Math.Sign(dy);

        float tDeltaX;
        float tDeltaY;

        float tMaxX;
        float tMaxY;

        if (stepX != 0)
        {
            tDeltaX = 1.0f / Mathf.Abs(dx);

            float nextBoundaryX = (stepX > 0) ? x + 1.0f : x;

            tMaxX = (nextBoundaryX - startX) / dx;
        }
        else
        {
            tDeltaX = float.PositiveInfinity;
            tMaxX = float.PositiveInfinity;
        }

        if (stepY != 0)
        {
            tDeltaY = 1.0f / Mathf.Abs(dy);

            float nextBoundaryY = (stepY > 0) ? y + 1.0f : y;

            tMaxY = (nextBoundaryY - startY) / dy;
        }
        else
        {
            tDeltaY = float.PositiveInfinity;
            tMaxY = float.PositiveInfinity;
        }

        while (true)
        {
            float nextT = Mathf.Min(tMaxX, tMaxY);

            if (nextT > 1.0f)
                break;

            if (tMaxX < tMaxY)
            {
                x += stepX;
                tMaxX += tDeltaX;
            }
            else if (tMaxY < tMaxX)
            {
                y += stepY;
                tMaxY += tDeltaY;
            }
            else
            {
                // 코너를 정확히 통과

                if (stepX != 0 && IsBlocked(grid, width, height, x + stepX, y))
                {
                    return false;
                }

                if (stepY != 0 && IsBlocked(grid, width, height, x, y + stepY))
                {
                    return false;
                }

                x += stepX;
                y += stepY;

                tMaxX += tDeltaX;
                tMaxY += tDeltaY;
            }

            if (IsBlocked(grid, width, height, x, y))
                return false;
        }

        return true;
    }


    static void BuildLinks(GridMapSO so)
    {
        int fineWidth = so.width * so.tileSize;
        int fineHeight = so.height * so.tileSize;

        byte[] fineGrid = new byte[fineWidth * fineHeight];

        List<Vector2> waypointPos = new List<Vector2>();

        for (int ty = 0; ty < so.height; ++ty)
        {
            for (int tx = 0; tx < so.width; ++tx)
            {
                byte value = so.data[ty * so.width + tx];

                int startX = tx * so.tileSize;
                int startY = ty * so.tileSize;

                float centerX = startX + so.tileSize * 0.5f;
                float centerY = startY + so.tileSize * 0.5f;

                if (value == 2)
                {
                    waypointPos.Add(new Vector2(centerX, centerY));

                    value = 0;
                }

                for (int y = 0; y < so.tileSize; ++y)
                {
                    int dstY = startY + y;
                    int rowOffset = dstY * fineWidth;

                    for (int x = 0; x < so.tileSize; ++x)
                    {
                        int dstX = startX + x;

                        fineGrid[rowOffset + dstX] = (byte)(value == 1 ? 1 : 0);
                    }
                }
            }

        }

        List<int>[] graph = BuildLinks(fineGrid, fineWidth, fineHeight, waypointPos, so.linkDistance);

        List<int> allLinks = new List<int>();

        WaypointNode[] nodes = new WaypointNode[waypointPos.Count];

        for (int i = 0; i < waypointPos.Count; ++i)
        {
            nodes[i].position = waypointPos[i];

            nodes[i].firstLink = allLinks.Count;

            nodes[i].linkCount = graph[i].Count;

            allLinks.AddRange(graph[i]);
        }

        so.waypoints = nodes;
        so.links = allLinks.ToArray();
    }

    public static List<int>[] BuildLinks(byte[] grid, int width, int height, List<Vector2> waypoints, float maxDistance)
    {
        int count = waypoints.Count;

        List<int>[] links = new List<int>[count];

        for (int i = 0; i < count; ++i)
            links[i] = new List<int>();

        float maxDistanceSq = maxDistance * maxDistance;

        for (int i = 0; i < count; ++i)
        {
            Vector2 a = waypoints[i];

            for (int j = i + 1; j < count; ++j)
            {
                Vector2 b = waypoints[j];

                float dx = a.x - b.x;
                float dy = a.y - b.y;

                float distSq = dx * dx + dy * dy;

                if (distSq > maxDistanceSq)
                    continue;

                if (i == 52 && j == 57)
                {
                    Debug.Log("Test");
                }

                if (!HasLineOfSight(grid, width, height, a.x, a.y, b.x, b.y))
                {
                    continue;
                }

                links[i].Add(j);
                links[j].Add(i);
            }
        }

        return links;
    }

    void ExportBinary(GridMapSO so)
    {
        int fineWidth = so.width * so.tileSize;
        int fineHeight = so.height * so.tileSize;

        byte[] fineGrid = new byte[fineWidth * fineHeight];

        List<Vector2> spawns = new List<Vector2>();

        for (int ty = 0; ty < so.height; ++ty)
        {
            for (int tx = 0; tx < so.width; ++tx)
            {
                byte value = so.data[ty * so.width + tx];

                int startX = tx * so.tileSize;
                int startY = ty * so.tileSize;

                float centerX = startX + (so.tileSize * 0.5f);

                float centerY = startY + (so.tileSize * 0.5f);

                if (value == 2)
                {
                    value = 0;
                }
                else if (value == 3)
                {
                    spawns.Add(new Vector2(centerX, centerY));
                    value = 0;
                }

                for (int y = 0; y < so.tileSize; ++y)
                {
                    int dstY = startY + y;
                    int rowOffset = dstY * fineWidth;

                    for (int x = 0; x < so.tileSize; ++x)
                    {
                        int dstX = startX + x;

                        fineGrid[rowOffset + dstX] = (byte)(value == 1 ? 1 : 0);
                    }
                }
            }
        }

        string defaultName = so.name + ".bytes";

        string path = EditorUtility.SaveFilePanel(
                "Export Grid Binary",
                Application.dataPath,
                defaultName,
                "bytes");

        if (string.IsNullOrEmpty(path))
            return;

        using (FileStream fs = new FileStream( path, FileMode.Create, FileAccess.Write))
        using (BinaryWriter bw = new BinaryWriter(fs))
        {
            // collision map
            bw.Write(fineWidth);
            bw.Write(fineHeight);
            bw.Write(fineGrid);

            // waypoints (CSR Header)
            bw.Write(so.waypoints.Length);

            for (int i = 0; i < so.waypoints.Length; ++i)
            {
                WaypointNode node = so.waypoints[i];

                bw.Write(node.position.x);
                bw.Write(node.position.y);

                bw.Write(node.firstLink);
                bw.Write(node.linkCount);
            }

            // CSR Links
            bw.Write(so.links.Length);

            for (int i = 0; i < so.links.Length; ++i)
            {
                bw.Write(so.links[i]);
            }

            // spawns
            bw.Write(spawns.Count);

            for (int i = 0; i < spawns.Count; ++i)
            {
                bw.Write(spawns[i].x);
                bw.Write(spawns[i].y);
            }
        }

        AssetDatabase.Refresh();

        Debug.Log($"Exported GridMap: {path}");
    }
}
