// Assets/Editor/MeshPrefabBuilder.cs

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MeshPrefabBuilder
{
#if UNITY_EDITOR

    [MenuItem("Tools/Build Mesh Prefab With Material")]
    static void Build()
    {
        // -----------------------------
        // 1. Mesh 생성 (Quad)
        // -----------------------------
        Mesh mesh = new Mesh();
        mesh.name = "GeneratedQuad";

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(1, 0, 0);
        vertices[2] = new Vector3(0, 0, 1);
        vertices[3] = new Vector3(1, 0, 1);

        int[] indices = new int[6]
        {
            0, 2, 1,
            2, 3, 1
        };

        Vector2[] uv = new Vector2[4];
        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);

        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.uv = uv;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // -----------------------------
        // 2. Mesh asset 저장
        // -----------------------------
        string meshPath = "Assets/GeneratedQuad.asset";
        AssetDatabase.CreateAsset(mesh, meshPath);

        // -----------------------------
        // 3. Material 생성 + 저장
        // -----------------------------
        string matPath = "Assets/GeneratedMat.mat";

        Material mat = new Material(Shader.Find("Standard"));
        mat.name = "GeneratedMat";

        // 필요하면 색 지정
        mat.color = Color.white;

        AssetDatabase.CreateAsset(mat, matPath);

        AssetDatabase.SaveAssets();

        // -----------------------------
        // 4. GameObject 구성
        // -----------------------------
        GameObject go = new GameObject("GeneratedMeshObject");

        MeshFilter mf = go.AddComponent<MeshFilter>();
        MeshRenderer mr = go.AddComponent<MeshRenderer>();

        // 핵심: shared 사용
        mf.sharedMesh = mesh;
        mr.sharedMaterial = mat;

        // -----------------------------
        // 5. Prefab 저장
        // -----------------------------
        string prefabPath = "Assets/GeneratedQuad.prefab";
        PrefabUtility.SaveAsPrefabAsset(go, prefabPath);

        // -----------------------------
        // 6. 정리
        // -----------------------------
        GameObject.DestroyImmediate(go);

        Debug.Log("Mesh + Material + Prefab 생성 완료");
    }

#endif
}
