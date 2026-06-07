using MoreMountains.Feedbacks;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Splines.Interpolators;
struct MoveState
{
    public Vector2 pos;
    public Vector2 target;
    public float angle;
}

struct MoveInput
{
    public Vector2 target;
    public PlayerDirection dir;
}

public class PlayerController : NetBehaviour, ITickable<MoveState, MoveInput>
{
    public GridMapSO gridMapSO;

    public float radius = 0.3f;
    public float moveSpeed = 4.0f;
    public float stopThreshold = 0.05f;
    public float spawnHeight = 1;
    public Vector2 Target { get { return _renderTarget; } }

    int[] ticks = new int[100];
    MoveInput[] inputs = new MoveInput[100];

    // 내부 상태
    private GridMap _map;

    private MoveState _state;
    private Vector2 _renderTarget;
    private ReplayerRunner<MoveState, MoveInput> _runner;

    private TopdownControllerRevised _topdown;

    public bool hasAuthority;

    public override int RenderingOrder => 0;

    public override ITickRunner Runner => _runner;

    public override NetBehaviourType Type => NetBehaviourType.Controller;

    public override void Init()
    {
        base.Init();
        _topdown = GetComponent<TopdownControllerRevised>();
        _map = new GridMap(gridMapSO);
        _runner = new ReplayerRunner<MoveState, MoveInput>(this, hasAuthority, Entity.renderDelay, 
            onApplied : (int tick, MoveInput input) => 
            {
                // 서버 연동
                C_MoveStart pkt = new C_MoveStart();
                pkt.targetX = input.target.x;
                pkt.targetY = input.target.y;
                pkt.clientTick = tick;
                pkt.dir = (byte)input.dir;
                NetworkManager.Instance.Send(pkt.Write());

                // 서버 시뮬레이션
                //_tickScheduler.ScheduleAfter(3, () =>
                //{
                //    S_MoveStart pkt = new S_MoveStart();
                //    pkt.targetX = input.target.x;
                //    pkt.targetY = input.target.y;
                //    pkt.acceptTick = tick;
                //    pkt.dir = (byte)input.dir;
                //    DispatchPacket(pkt);
                //});

                //_tickScheduler.ScheduleAfter(3, () =>
                //{
                //    NetEntity entity = NetworkManager.Instance.entitySystem.Get(1);
                //    S_MoveStart pkt = new S_MoveStart();
                //    pkt.targetX = input.target.x;
                //    pkt.targetY = input.target.y;
                //    pkt.acceptTick = tick;
                //    pkt.dir = (byte)input.dir;
                //    entity.DispatchPacket(NetBehaviourType.Controller, pkt);
                //});
            });
    }

    public override void OnSpawn(int tick)
    {
        base.OnSpawn(tick);
        _tickScheduler.Register(_runner);
        _state.pos = new Vector2(transform.position.x, transform.position.z);
        _state.target = _state.pos;
        _renderTarget = _state.target;
        transform.position = new Vector3(transform.position.x, spawnHeight, transform.position.z);
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        _tickScheduler.Unregister(_runner);
    }

    public Vector2 GetPositionAt(int tick)
    {
        return _runner.GetState(tick).pos;
    }

    public Vector2 GetServerPosition()
    {
        return _state.pos;
    }

    public Vector2 GetRenderPosition()
    {
        if (_runner.TryGetRenderPair(out var prev, out var curr))
        {
            Vector2 renderPos = Vector2.Lerp(prev.pos, curr.pos, _tickScheduler.Alpha);
            return renderPos;
        }

        return default;
    }

    public override void DispatchPacket(IPacket packet)
    {
        switch (packet.Protocol)
        {
            case (ushort)PacketID.S_MoveStart:
            {
                var p = packet as S_MoveStart;
                var input = new MoveInput
                {
                    target = new Vector2(p.targetX, p.targetY),
                    dir = (PlayerDirection)p.dir,
                };

                _runner.EnqueueServerInput(p.acceptTick, input);
                break;
            }
            case (ushort)PacketID.S_MoveState:
            {
                var p = packet as S_MoveState;
                var state = new MoveState
                {
                    angle = 0,
                    pos = new Vector2(p.serverX, p.serverY),
                    target = new Vector2(p.targetX, p.targetY)
                };

                _runner.EnqueueServerState(p.currentTick, state);
                break;
            }
            default:
            {
                break;
            }
        }
    }

    Vector2 ClampTarget(GridMap map, Vector2 start, Vector2 target, float moveSpeed, float tickDelta)
    {
        Vector2 dir = target - start;

        float lenSq = dir.x * dir.x + dir.y * dir.y;
        if (lenSq < 1e-8f)
            return start;

        float len = Mathf.Sqrt(lenSq);
        float invLen = 1.0f / len;

        dir.x *= invLen;
        dir.y *= invLen;

        float step = moveSpeed * tickDelta; // 핵심: 틱 기반 이동 거리
        float maxDist = len;

        Vector2 cur = start;
        Vector2 lastSafe = start;

        float traveled = 0.0f;

        while (traveled < maxDist)
        {
            float remain = maxDist - traveled;
            float curStep = (remain < step) ? remain : step;

            Vector2 next;
            next.x = cur.x + dir.x * curStep;
            next.y = cur.y + dir.y * curStep;

            if (CheckWall(map, next))
                break;

            lastSafe = next;
            cur = next;
            traveled += curStep;
        }

        return lastSafe;
    }

    // ===== 외부 입력 =====
    public void SetMoveTarget(PlayerDirection dir, Vector2 target)
    {
        Vector2 clamped = ClampTarget(_map, _state.pos, target, moveSpeed, _tickScheduler.GetDeltaTime());

        MoveInput input;
        input.target = clamped;
        input.dir = dir;
        _renderTarget = clamped;
        _runner.EnqueueClientInput(_tickScheduler.GetCurrentTick(), input);
    }

    public static int ToCell(float v)
    {
        return Mathf.FloorToInt(v);
    }

    public void SetMap(GridMap map)
    {
        this._map = map;
    }

    public void Tick(int tick, float dt)
    {
        Vector2 dir = _state.target - _state.pos;
        float dist = dir.magnitude;

        if (dist < stopThreshold)
        {
            _state.pos = _state.target;
            return;
        }

        float step = moveSpeed * dt;

        Vector2 nextPos;
        if (dist <= step)
        { 
            nextPos = _state.target;
        }
        else
        { 
            nextPos = _state.pos + dir.normalized * step;
        }

        if (!CheckWall(_map, nextPos))
        {
            _state.pos = nextPos;
        }
        else
        {
            // 충돌 시 즉시 정지
            _state.target = _state.pos;
        }
    }

    bool CheckWall(GridMap map, Vector2 nextPos)
    {
        int minX = ToCell(nextPos.x - radius);
        int maxX = ToCell(nextPos.x + radius);
        int minZ = ToCell(nextPos.y - radius);
        int maxZ = ToCell(nextPos.y + radius);

        for (int z = minZ; z <= maxZ; z++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                if (!map.IsBlocked(x, z))
                    continue;

                if (CircleVsCell(nextPos.x, nextPos.y, radius, x, z))
                {
                    return true;
                }
            }
        }
        return false;
    }

    bool CircleVsCell(float cx, float cy, float radius, float cellX, float cellY)
    {
        float nearestX = Mathf.Clamp(cx, cellX, cellX + 1.0f);
        float nearestY = Mathf.Clamp(cy, cellY, cellY + 1.0f);

        float dx = cx - nearestX;
        float dy = cy - nearestY;

        return (dx * dx + dy * dy) <= (radius * radius);
    }

    void ITickable<MoveState, MoveInput>.ApplyInput(in MoveInput input)
    {
        _state.target = input.target;
        _state.angle = PlayerInput.dirAngle[(byte)input.dir];
    }

    MoveState ITickable<MoveState, MoveInput>.CaptureState()
    {
        MoveState state;
        state.pos = _state.pos;
        state.target = _state.target;
        state.angle = _state.angle;
        return state;
    }

    void ITickable<MoveState, MoveInput>.RestoreState(in MoveState state)
    {
        _state.pos = state.pos;
        _state.target = state.target;
        _state.angle = state.angle;
    }

    public override void OnRender(float alpha)
    {
        if (_runner.TryGetRenderPair(out var prev, out var curr))
        {
            Vector3 renderPos = Vector3.Lerp(prev.pos, curr.pos, alpha);
            //float angle = Mathf.LerpAngle(prev.angle, curr.angle, alpha);
            float angle = curr.angle;
            float rad = angle * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));
            transform.position = new Vector3(renderPos.x, transform.position.y, renderPos.y);
            if (prev.pos == curr.pos)
            {
                _topdown.SetMovement(Vector3.zero);
            }
            else
            {
                _topdown.SetMovement(moveSpeed * dir);
            }
        }
    }

    public void OnDrawGizmos()
    {
        if (_init == false)
            return;

        Gizmos.color = Color.red;
        Vector3 renderPos = new Vector3(_renderTarget.x, transform.position.y, _renderTarget.y);
        Gizmos.DrawSphere(renderPos, 0.2f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, renderPos);

        int tick = _tickScheduler.GetCurrentTick();
        int cnt = _runner.CollectInputs(tick - 32, tick, ticks, inputs, 32);
        if (cnt <= 0)
            return;

        Gizmos.color = Color.green;

        Vector3 prevPos = new Vector3(inputs[0].target.x, transform.position.y, inputs[0].target.y);
#if UNITY_EDITOR
            // 숫자 표시 (tick or index)
            Handles.Label(prevPos + Vector3.up * 0.2f, "0");
#endif

        // 첫 점
        Gizmos.DrawSphere(prevPos, 0.15f);

        for (int i = 1; i < cnt; i++)
        {
            Vector3 currPos = new Vector3(inputs[i].target.x, transform.position.y, inputs[i].target.y);

            // 점
            Gizmos.DrawSphere(currPos, 0.15f);

            // 선
            Gizmos.DrawLine(prevPos, currPos);

#if UNITY_EDITOR
            // 숫자 표시 (tick or index)
            Handles.Label(currPos + Vector3.up * 0.2f, i.ToString());
#endif

            prevPos = currPos;
        }
    }
}
