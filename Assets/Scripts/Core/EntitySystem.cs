using System.Collections.Generic;
using UnityEngine;

public class EntitySystem : MonoBehaviour
{
    private Dictionary<int, NetEntity> _map = new Dictionary<int, NetEntity>(1024);

    public NetEntity Get(int id)
    {
        NetEntity e;
        _map.TryGetValue(id, out e);
        return e;
    }

    public void Register(int id, NetEntity entity)
    {
        entity.entityId = id;
        _map[id] = entity;
        entity.OnSpawn();
    }

    public void Remove(int id)
    {
        NetEntity e;
        if (_map.TryGetValue(id, out e))
        {
            e.OnDespawn();
            _map.Remove(id);
        }
    }

    public void Clear()
    {
        foreach (var kv in _map)
        {
            kv.Value.OnDespawn();
        }
        _map.Clear();
    }
}
