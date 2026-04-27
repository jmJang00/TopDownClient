using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridMapSO))]
public class GridMapSOEditor : Editor
{
    const int CELL_SIZE = 12;

    public override void OnInspectorGUI()
    {
        GridMapSO so = (GridMapSO)target;

        DrawDefaultInspector();

        if (so.data == null || so.data.Length != so.width * so.height)
            return;

        GUILayout.Space(10);

        DrawGrid(so);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(so);
        }
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

                byte val = so.data[z * so.width + x];

                // 색상
                EditorGUI.DrawRect(cell, val == 1 ? Color.black : Color.white);

                // 클릭 처리
                if (e.type == EventType.MouseDown && cell.Contains(e.mousePosition))
                {
                    so.data[z * so.width + x] = (byte)(val == 1 ? 0 : 1);
                    GUI.changed = true;
                }
            }
        }
    }
}
