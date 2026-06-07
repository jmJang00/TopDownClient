using UnityEngine;

public abstract class NetBehaviour : MonoBehaviour
{
    public abstract int RenderingOrder { get; }
    public abstract ITickRunner Runner { get; }
    public abstract NetBehaviourType Type { get; }

    public NetEntity Entity { get; set; }
    public bool Ready { get { return _spawned; } }

    protected bool _init = false;
    protected bool _spawned = false;
    protected TickScheduler _tickScheduler;
    protected EntitySystem _entitySystem;

    public virtual void Init()
    {
        _tickScheduler = Entity.TickScheduler;
        _entitySystem = Entity.EntitySystem;
        _init = true;
    }

    public virtual void OnSpawn(int tick)
    {
        _spawned = true;
    }

    public virtual void OnDespawn()
    {

    }
    
    public virtual void OnRender(float alpha)
    {

    }

    public virtual void DispatchPacket(IPacket packet)
    {

    }
}
