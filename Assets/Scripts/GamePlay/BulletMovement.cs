using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;

struct BulletState
{
    public Vector2 pos;
    public Vector2 target;
}

struct BulletInput
{

}

public class BulletMovement : NetBehaviour, ITickable<BulletState, BulletInput> 
{
    private BulletState _state;
    private float _speed = 32;
    private ReplayerRunner<BulletState, BulletInput> _runner;
    public float stopThreshold = 0.05f;
    public float groundOffset = 1.2f;
    private int _spawnTick;
    private Vector2 _spawnPos;

    private bool _initializedFeedbacks = false;
    public MMFeedbacks HitDamageableFeedback;
    public MMFeedbacks HitNonDamageableFeedback;

    public override int RenderingOrder => 0;

    public override ITickRunner Runner => _runner;

    public override NetBehaviourType Type => NetBehaviourType.BulletMovement;

    public override void Init()
    {
        base.Init();
        if (_runner == null)
        {
            _runner = new ReplayerRunner<BulletState, BulletInput>(this, false, Entity.renderDelay, null);
        }

        if (!_initializedFeedbacks)
        {
			HitDamageableFeedback?.Initialization(this.gameObject);
			HitNonDamageableFeedback?.Initialization(this.gameObject);
            _initializedFeedbacks = true;
        }
    }

    public override void OnSpawn(int tick)
    {
        _tickScheduler.Register(_runner);
        transform.position = new Vector3(_spawnPos.x, groundOffset, _spawnPos.y);
    }

    public override void OnDespawn()
    {
        _tickScheduler.Unregister(_runner);
    }
    
    public override void OnRender(float alpha)
    {
        if (_runner.TryGetRenderPair(out var prev, out var curr))
        {
            Vector2 renderPos;

            if (_tickScheduler.GetCurrentTick() + Entity.renderDelay > _spawnTick)
            {
                renderPos = Vector2.Lerp(prev.pos, curr.pos, alpha);
            }
            else
            {
                renderPos = _spawnPos;
            }

            transform.position = new Vector3(renderPos.x, transform.position.y, renderPos.y);
        }
    }

    public override void DispatchPacket(IPacket packet)
    {
        switch (packet.Protocol)
        {
            case (ushort)PacketID.S_SpawnProjectile:
            {
                var p = packet as S_SpawnProjectile;
                BulletState state = new BulletState
                {
                    pos = new Vector2(p.spawnPosX, p.spawnPosY),
                    target = new Vector2(p.targetPosX, p.targetPosY),
                };
                _spawnPos = state.pos;
                _spawnTick = p.currentTick;
                _runner.EnqueueServerState(p.currentTick, state);
                break;
            }
            case (ushort)PacketID.S_ProjectileHit:
            {
                var p = packet as S_ProjectileHit;
                _tickScheduler.ScheduleAt(p.currentTick, () =>
                {
                    HitDamageableFeedback?.PlayFeedbacks(transform.position);
                    NetworkManager.Instance.spawnManager.DespawnAt(
                        _tickScheduler.GetCurrentTick() + 1, 
                        EntityType.Projectile, 
                        Entity.entityId);
                });
                break;
            }
            default:
            {
                break;
            }
        }
        
    }

    public void Tick(int tick, float dt)
    {
        Vector2 dir = _state.target - _state.pos;
        float dist = dir.magnitude;
        float step = _speed * dt;

        if (dist < stopThreshold)
        {
            _state.pos = _state.target;
            HitNonDamageableFeedback?.PlayFeedbacks(transform.position);
            NetworkManager.Instance.spawnManager.DespawnAt(
                _tickScheduler.GetCurrentTick() + Entity.renderDelay, 
                EntityType.Projectile, Entity.entityId);
            return;
        }

        Vector2 nextPos;
        if (dist <= step)
        { 
            nextPos = _state.target;
        }
        else
        { 
            nextPos = _state.pos + dir.normalized * step;
        }

        _state.pos = nextPos;
    }

    void ITickable<BulletState, BulletInput>.ApplyInput(in BulletInput input)
    {
    }

    BulletState ITickable<BulletState, BulletInput>.CaptureState()
    {
        return _state;
    }

    void ITickable<BulletState, BulletInput>.RestoreState(in BulletState state)
    {
        _state = state;
    }
}
