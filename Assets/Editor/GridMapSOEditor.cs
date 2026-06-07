using System.Collections.Generic;
using System.IO;
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

        GUILayout.Label(
            "0=Empty  1=Blocked  2=Waypoint  3=Spawn");
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

                Color color =
                    val < COLORS.Length
                    ? COLORS[val]
                    : Color.magenta;

                EditorGUI.DrawRect(cell, color);

                EditorGUI.DrawRect(
                    new Rect(cell.xMax - 1, cell.y, 1, cell.height),
                    Color.gray);

                EditorGUI.DrawRect(
                    new Rect(cell.x, cell.yMax - 1, cell.width, 1),
                    Color.gray);

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

    void ExportBinary(GridMapSO so)
    {
        int fineWidth = so.width * so.tileSize;
        int fineHeight = so.height * so.tileSize;

        byte[] fineGrid = new byte[fineWidth * fineHeight];

        List<Vector2> waypoints = new List<Vector2>();
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

                // waypoint
                if (value == 2)
                {
                    waypoints.Add(new Vector2(centerX, centerY));
                    value = 0;
                }
                // spawn
                else if (value == 3)
                {
                    spawns.Add(new Vector2(centerX, centerY));
                    value = 0;
                }

                // collision expand
                for (int y = 0; y < so.tileSize; ++y)
                {
                    int dstY = startY + y;
                    int rowOffset = dstY * fineWidth;

                    for (int x = 0; x < so.tileSize; ++x)
                    {
                        int dstX = startX + x;

                        fineGrid[rowOffset + dstX] =
                            (byte)(value == 1 ? 1 : 0);
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

        using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
        using (BinaryWriter bw = new BinaryWriter(fs))
        {
            // collision map
            bw.Write(fineWidth);
            bw.Write(fineHeight);
            bw.Write(fineGrid);

            // waypoints
            bw.Write(waypoints.Count);

            for (int i = 0; i < waypoints.Count; ++i)
            {
                bw.Write(waypoints[i].x);
                bw.Write(waypoints[i].y);
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
