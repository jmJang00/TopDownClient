using MoreMountains.TopDownEngine;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

public class PickerSpawnManager : MonoBehaviour
{    
    private GameScene _game;
    private Dictionary<uint, GameObject> _items;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {        
        _game = GetComponent<GameScene>();
        _items = new Dictionary<uint, GameObject>();
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spawn(uint uniqueId, ItemType type, Vector3 pos)
    {
        if(_items.ContainsKey(uniqueId))
        {
            Debug.LogError("이미 등록된 피커 아이템.");
            return;
        }

        GameObject item = EnumToItemResource.GetPickerPrefab(type);
        if(item == null)
        {
            Debug.LogError("등록되지 않은 피커 아이템 로드 요청.");
            return;
        }

        item.transform.position = pos;
        item.SetActive(true);
        _items.Add(uniqueId, item);

        Debug.Log($"아이템[{type}] 생성 Pos[{pos}] Id[{uniqueId}]");
    }

    public void Despawn(uint uniqueId)
    {
        if(!_items.ContainsKey(uniqueId))
        {
            Debug.LogError("등록되어있지 않은 고유 아이디.");
            return;
        }

        GameObject item = _items[uniqueId];
        item.SetActive(false);
        GameObject.Destroy(item);
        _items.Remove(uniqueId);        
    }
}
