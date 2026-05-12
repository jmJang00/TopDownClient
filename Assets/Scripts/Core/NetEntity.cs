using System;
using System.Collections.Generic;
using UnityEngine;
public class NetEntity : MonoBehaviour
{
    public string ownerName = "NetworkManager";
    public int entityId;
    public int renderDelay;
    public bool active;
    public EntityType type;
    public NetworkManager Network { get; set; }
    public TickScheduler TickScheduler { get; set; }
    public EntitySystem EntitySystem { get; set; }

    private NetBehaviour[] _behaviours;
    private byte[] _routeTable;

    public virtual void Init() 
    {
        Network = GameObject.Find(ownerName).GetComponent<NetworkManager>();
        TickScheduler = Network.tickScheduler;
        EntitySystem = Network.entitySystem;
        _behaviours = GetComponents<NetBehaviour>();
        _routeTable = new byte[(int)NetBehaviourType.Max];

        for (int i = 0; i < _routeTable.Length; i++)
        {
            _routeTable[i] = 0xFF;
        }

        Array.Sort(_behaviours, (a, b) => a.RenderingOrder - b.RenderingOrder);

        for (int i = 0; i < _behaviours.Length; ++i)
        {
            _behaviours[i].Entity = this;
            int idx = (int)_behaviours[i].Type;

            Debug.Assert(_routeTable[idx] == 0xFF, "Duplicate BehaviourType detected");

            _routeTable[idx] = (byte)i;

            _behaviours[i].Init();
        }
    }

    public void OnEnable()
    {
        active = true;
    }

    public void OnDisable()
    {
        active = false;
    }

    public virtual void OnRender(float alpha)
    {
        if (!active)
            return;

        for (int i = 0; i < _behaviours.Length; ++i)
        {
            _behaviours[i].OnRender(alpha);
        }
    }

    public void DispatchPacket(NetBehaviourType type, IPacket packet)
    {
        int idx = _routeTable[(int)type];

        Debug.Assert(idx != 0xFF, "Behaviour not found");

        _behaviours[idx].DispatchPacket(packet);
    }

    public virtual void OnSpawn(int tick) 
    {
        for (int i = 0; i < _behaviours.Length; ++i)
            _behaviours[i].OnSpawn(tick);
    }

    public virtual void OnDespawn() 
    { 
        for (int i = 0; i < _behaviours.Length; ++i)
            _behaviours[i].OnDespawn();
    }
}
