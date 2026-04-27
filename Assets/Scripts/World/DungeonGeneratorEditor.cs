#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

#if UNITY_EDITOR
[CustomEditor(typeof(DungeonGenerator))]
public class DungeonGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DungeonGenerator gen = (DungeonGenerator)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Generate Dungeon"))
        {
            gen.GenerateMap();
            EditorUtility.SetDirty(gen);
        }

        if (GUILayout.Button("Random Seed"))
        {
            gen.seed = (uint)Random.Range(0, int.MaxValue);
            EditorUtility.SetDirty(gen);
        }
    }
}
#endif
