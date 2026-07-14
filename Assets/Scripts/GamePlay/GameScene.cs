using MoreMountains.TopDownEngine;
using UnityEngine;

public class GameScene : MonoBehaviour
{
    public TickScheduler tickScheduler;
    public EntitySystem entitySystem;
    public SpawnManager spawnManager;
    public UI_GameEnd gameSelectUI;
    public bool debugPlayer = false;
    private float _debugAccum;
    private int _debugTick;

    public void Start()
    {
        tickScheduler = GetComponent<TickScheduler>();
        entitySystem = GetComponent<EntitySystem>();
        spawnManager = GetComponent<SpawnManager>();
        NetworkManager.Instance.SetGameScene(this);
        if (debugPlayer)
        {
            _debugAccum = 0;
            _debugTick = 0;
            spawnManager.SpawnAt(5, EntityType.MyPlayer, 0, new Vector3(0, 2, 0));
            tickScheduler.ScheduleAfter(5, () =>
            {
                NetEntity entity = entitySystem.Get(0);
                entity.GetComponent<Health>().SetHealth(100);
                VirtualInput(entity, new Vector2(94, 111), 0);
            });
            spawnManager.SpawnAt(5, EntityType.OtherPlayer, 1, new Vector3(0, -18, 0));
            tickScheduler.ScheduleAfter(5, () =>
            {
                NetEntity entity = entitySystem.Get(1);
                VirtualInput(entity, new Vector2(94, 111), 0);
            });
        }
    }

    public void VirtualInput(NetEntity entity, Vector2 pos, float angle)
    {
        entity.isDebugMode = true;

        S_NtfMoveState moveState = new S_NtfMoveState();
        moveState.serverX = pos.x;
        moveState.serverY = pos.y;
        moveState.targetX = pos.x;
        moveState.targetY = pos.y;
        moveState.currentTick = 5;
        entity.DispatchPacket(NetBehaviourType.Controller, moveState);

        S_NtfRotateState state = new S_NtfRotateState();
        state.currentAngle = angle;
        state.targetAngle = angle;
        state.currentTick = 5;
        entity.DispatchPacket(NetBehaviourType.Aim, state);
    }

    public void ProcessUpdate()
    {
        if (debugPlayer)
        {
            _debugAccum += Time.deltaTime;
            float dt = tickScheduler.GetDeltaTime();
            while (_debugAccum >= dt)
            {
                _debugTick++;
                if (_debugTick % 5 == 0)
                {
                    tickScheduler.UpdateTick(_debugTick);
                }
                _debugAccum -= dt;
            }
        }

        tickScheduler.Simulate();

        entitySystem.RunRender(tickScheduler.Alpha);
    }

    public void Clear()
    {
        if (entitySystem.MyCharacter)
        {
            GameObject.Destroy(entitySystem.MyCharacter.gameObject);
        }
        entitySystem.Clear();
    }
}
