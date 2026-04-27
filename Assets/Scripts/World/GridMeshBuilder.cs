using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridMeshBuilder : MonoBehaviour
{
    public float tileSize = 1.0f;
    public float wallHeight = 2.0f;

    public Material material; // Standard 머티리얼

    List<Vector3> vertices = new List<Vector3>(1024);
    List<int> indices = new List<int>(2048);
    List<Vector2> uvs = new List<Vector2>(1024);

    int width;
    int height;
    byte[,] map = null;

    public void SetMap()
    {
        DungeonGenerator generator = GetComponent<DungeonGenerator>();
        map = generator.Map;
        width = generator.width;
        height = generator.height;
    }

    public void SaveAsPrefab(string folderPath)
    {
#if UNITY_EDITOR
        if (GetComponent<MeshFilter>().sharedMesh == null)
        {
            Debug.LogError("Mesh 없음. 먼저 Build() 호출 필요");
            return;
        }

        // -----------------------------
        // 1. 전체 오브젝트 복사 (자식 포함)
        // -----------------------------
        GameObject rootCopy = Object.Instantiate(gameObject);
        rootCopy.name = "GeneratedMeshObject";

        // -----------------------------
        // 2. Mesh asset화
        // -----------------------------
        Mesh srcMesh = rootCopy.GetComponent<MeshFilter>().sharedMesh;
        Mesh meshCopy = Object.Instantiate(srcMesh);
        meshCopy.name = "GeneratedMesh";

        string meshPath = AssetDatabase.GenerateUniqueAssetPath(folderPath + "/GeneratedMesh.asset");
        AssetDatabase.CreateAsset(meshCopy, meshPath);

        rootCopy.GetComponent<MeshFilter>().sharedMesh = meshCopy;

        // -----------------------------
        // 3. Material asset화
        // -----------------------------
        Material srcMat = rootCopy.GetComponent<MeshRenderer>().sharedMaterial;
        Material matCopy = Object.Instantiate(srcMat);
        matCopy.name = "GeneratedMat";

        string matPath = AssetDatabase.GenerateUniqueAssetPath(folderPath + "/GeneratedMat.mat");
        AssetDatabase.CreateAsset(matCopy, matPath);

        rootCopy.GetComponent<MeshRenderer>().sharedMaterial = matCopy;

        // -----------------------------
        // 4. Prefab 저장 (자식 포함)
        // -----------------------------
        string prefabPath = AssetDatabase.GenerateUniqueAssetPath(folderPath + "/GeneratedMesh.prefab");
        PrefabUtility.SaveAsPrefabAsset(rootCopy, prefabPath);

        // -----------------------------
        // 5. 정리
        // -----------------------------
        GameObject.DestroyImmediate(rootCopy);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Prefab 생성 완료: " + prefabPath);
#endif
    }

    void BuildFloorCollider()
    {
        GameObject go = new GameObject("FloorCollider");
        go.transform.parent = transform;

        float totalWidth = width * tileSize;
        float totalHeight = height * tileSize;

        // 중심 (타일 중심 기준 맞추기)
        float cx = totalWidth * 0.5f;
        float cz = totalHeight * 0.5f;

        go.transform.position = new Vector3(cx, -0.05f, cz);

        BoxCollider col = go.AddComponent<BoxCollider>();

        col.size = new Vector3(
            totalWidth,
            0.1f,          // 얇은 두께
            totalHeight
        );
    }

    void BuildWallColliders()
    {
        // 기존 콜라이더 제거 (재생성 대비)
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("Wall_"))
                Destroy(child.gameObject);
        }

        // 수평 (위 / 아래)
        for (int y = 0; y < height; y++)
        {
            BuildHorizontalColliders(y, 1);
            BuildHorizontalColliders(y, -1);
        }

        // 수직 (좌 / 우)
        for (int x = 0; x < width; x++)
        {
            BuildVerticalColliders(x, 1);
            BuildVerticalColliders(x, -1);
        }
    }

    void BuildHorizontalColliders(int y, int dy)
    {
        int x = 0;

        while (x < width)
        {
            if (!IsWall(x, y, 0, dy))
            {
                x++;
                continue;
            }

            int start = x;

            while (x < width && IsWall(x, y, 0, dy))
                x++;

            int length = x - start;

            CreateHorizontalCollider(start, y, length, dy);
        }
    }

    void BuildVerticalColliders(int x, int dx)
    {
        int y = 0;

        while (y < height)
        {
            if (!IsWall(x, y, dx, 0))
            {
                y++;
                continue;
            }

            int start = y;

            while (y < height && IsWall(x, y, dx, 0))
                y++;

            int length = y - start;

            CreateVerticalCollider(x, start, length, dx);
        }
    }

    bool IsWall(int x, int y, int dx, int dy)
    {
        if (map[y, x] != 1) return false;

        int nx = x + dx;
        int ny = y + dy;

        return (nx < 0 || ny < 0 || nx >= width || ny >= height || map[ny, nx] == 0);
    }

    void CreateHorizontalCollider(int startX, int y, int length, int dy)
    {
        float cx = (startX + length * 0.5f) * tileSize;
        float cz = (y + 0.5f + dy * 0.5f) * tileSize;

        GameObject go = new GameObject("Wall_H");
        go.transform.parent = transform;
        go.transform.position = new Vector3(cx, wallHeight * 0.5f, cz);

        BoxCollider col = go.AddComponent<BoxCollider>();
        col.size = new Vector3(length * tileSize, wallHeight, 0.5f);
    }

    void CreateVerticalCollider(int x, int startY, int length, int dx)
    {
        float cx = (x + 0.5f + dx * 0.5f) * tileSize;
        float cz = (startY + length * 0.5f) * tileSize;

        GameObject go = new GameObject("Wall_V");
        go.transform.parent = transform;
        go.transform.position = new Vector3(cx, wallHeight * 0.5f, cz);

        BoxCollider col = go.AddComponent<BoxCollider>();
        col.size = new Vector3(0.5f, wallHeight, length * tileSize);
    }

    public void Build()
    {
        vertices.Clear();
        indices.Clear();
        uvs.Clear();

        height = map.GetLength(0);
        width = map.GetLength(1);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (map[y, x] != 1) continue;

                AddFloor(x, y);

                CheckWall(x, y, 0, 1);
                CheckWall(x, y, 0, -1);
                CheckWall(x, y, 1, 0);
                CheckWall(x, y, -1, 0);
            }
        }

        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(indices, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();

        var mf = GetComponent<MeshFilter>();
        mf.sharedMesh = mesh;

        var mr = GetComponent<MeshRenderer>();
        mr.sharedMaterial = material;
        mr.sharedMaterial.SetInt("_Cull", (int)CullMode.Off);

        BuildWallColliders();

        BuildFloorCollider();
    }

    void AddQuad(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int start = vertices.Count;

        vertices.Add(v0);
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);

        // UV (기본 타일링)
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(1, 0));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(0, 1));

        indices.Add(start + 0);
        indices.Add(start + 1);
        indices.Add(start + 2);

        indices.Add(start + 0);
        indices.Add(start + 2);
        indices.Add(start + 3);
    }

    void AddFloor(int x, int y)
    {
        float cx = (x + 0.5f) * tileSize;
        float cz = (y + 0.5f) * tileSize;

        float h = tileSize * 0.5f;

        Vector3 v0 = new Vector3(cx - h, 0, cz - h);
        Vector3 v1 = new Vector3(cx + h, 0, cz - h);
        Vector3 v2 = new Vector3(cx + h, 0, cz + h);
        Vector3 v3 = new Vector3(cx - h, 0, cz + h);

        AddQuad(v0, v3, v2, v1);
    }

    void CheckWall(int x, int y, int dx, int dy)
    {
        int nx = x + dx;
        int ny = y + dy;

        if (nx < 0 || ny < 0 || nx >= width || ny >= height || map[ny, nx] == 0)
        {
            AddWall(x, y, dx, dy);
        }
    }

    void AddWall(int x, int y, int dx, int dy)
    {
        float cx = (x + 0.5f) * tileSize;
        float cz = (y + 0.5f) * tileSize;

        Vector3 center = new Vector3(cx, 0, cz);

        Vector3 dir = new Vector3(dx, 0, dy);
        Vector3 right = new Vector3(-dy, 0, dx);

        float half = tileSize * 0.5f;

        Vector3 wallCenter = center + dir * half;

        Vector3 p0 = wallCenter - right * half;
        Vector3 p1 = wallCenter + right * half;
        Vector3 p2 = p1 + Vector3.up * wallHeight;
        Vector3 p3 = p0 + Vector3.up * wallHeight;

        AddQuad(p0, p1, p2, p3);
    }
}
