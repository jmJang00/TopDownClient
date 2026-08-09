#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonGenerator))]
public class DungeonGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (target == null)
            return;

        DrawDefaultInspector();

        DungeonGenerator gen = (DungeonGenerator)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Generate Dungeon"))
        {
            GridMap map = gen.GenerateMap();
            GridMapSO asset = ScriptableObject.CreateInstance<GridMapSO>();
            asset.Copy(map);
            // 저장 경로
            string path = "Assets/Resources/Data/GridMap.asset";

            if (File.Exists(path))
            {
                AssetDatabase.DeleteAsset(path);
            }

            AssetDatabase.CreateAsset(asset, path);

            Debug.Log("GridMap 생성 완료: " + path);

            EditorUtility.SetDirty(asset);
            EditorUtility.SetDirty(gen);

            AssetDatabase.SaveAssets();
        }

        if (GUILayout.Button("Random Seed"))
        {
            gen.seed = (uint)Random.Range(0, int.MaxValue);
            EditorUtility.SetDirty(gen);
        }
    }
}
#endif
