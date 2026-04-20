#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridMeshBuilder))]
public class GridMeshBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridMeshBuilder builder = (GridMeshBuilder)target;

        if (GUILayout.Button("Build Mesh"))
        {
            builder.SetMap();
            builder.Build();
        }

        if (GUILayout.Button("Export Prefab"))
        {
            string path = EditorUtility.OpenFolderPanel("Save Folder", "Assets", "");

            if (!string.IsNullOrEmpty(path))
            {
                // 절대경로 → Assets 기준으로 변환
                if (path.StartsWith(Application.dataPath))
                {
                    string relative = "Assets" + path.Substring(Application.dataPath.Length);
                    builder.SaveAsPrefab(relative);
                }
                else
                {
                    Debug.LogError("Assets 폴더 내부만 가능");
                }
            }
        }
    }
}
#endif
