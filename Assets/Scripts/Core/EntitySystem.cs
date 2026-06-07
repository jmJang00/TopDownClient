using System.Collections.Generic;
using UnityEngine;

public class EntitySystem : MonoBehaviour
{
    public NetEntity MyCharacter { get; set; }
    private Dictionary<uint, NetEntity> _map = new Dictionary<uint, NetEntity>(1024);

    public NetEntity Get(uint id)
    {
        NetEntity e = null;
        _map.TryGetValue(id, out e);
        return e;
    }

    public void Register(uint id, NetEntity entity, bool mine = false)
    {
        entity.entityId = id;
        _map[id] = entity;
        if (mine) 
        { 
            MyCharacter = entity;
        }
    }

    public void Remove(uint id)
    {
        NetEntity e;

        if (MyCharacter?.entityId == id)
        {
            MyCharacter = null;
        }

        if (_map.TryGetValue(id, out e))
        {
            _map.Remove(id);
        }
    }

    public void Clear()
    {
        _map.Clear();
    }

    public void RunRender(float alpha)
    {
        foreach (var kv in _map)
        {
            kv.Value.OnRender(alpha);
        }
    }
}
