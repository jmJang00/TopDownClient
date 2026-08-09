using System.Collections.Generic;
using UnityEngine;

public class WorldUIManager : MonoBehaviour
{
    public static WorldUIManager Instance;

    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private Canvas _canvas;

    [SerializeField]
    private UI_NamePlate _prefab;

    private readonly List<UI_NamePlate> _active = new();
    private ObjectPool<UI_NamePlate> _pool;

    private void Awake()
    {
        Instance = this;
        _pool = new ObjectPool<UI_NamePlate>(_prefab, 5, transform);
    }

    public UI_NamePlate CreateNamePlate(Transform target, string nickname)
    {
        UI_NamePlate plate;

        plate = _pool.Acquire();

        plate.Initialize(target, nickname);

        _active.Add(plate);

        return plate;
    }


    public void RemoveNamePlate(UI_NamePlate plate)
    {
        plate.Release();

        _active.Remove(plate);

        _pool.Release(plate);
    }


    private void LateUpdate()
    {
        for (int i = 0; i < _active.Count; i++)
        {
            UI_NamePlate plate = _active[i];

            Vector3 screenPos = _camera.WorldToScreenPoint(plate.GetWorldPosition());

            bool visible =
                screenPos.z > 0 &&
                screenPos.x >= 0 &&
                screenPos.x <= Screen.width &&
                screenPos.y >= 0 &&
                screenPos.y <= Screen.height;

            plate.gameObject.SetActive(visible);


            if (!visible)
                continue;

            plate.UpdateFade();
        }
    }
}
