using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.UIElements;

public struct AimState
{
    public float currentAngle;
    public float targetAngle;
}

public struct AimInput
{
    public float targetAngle;
}

public class PlayerAimController : NetBehaviour, ITickable<AimState, AimInput>
{
    public override int RenderingOrder => 1;

    public override ITickRunner Runner => _runner;

    public override NetBehaviourType Type => NetBehaviourType.Aim;

    public bool hasAuthority;
    public GameObject ReticlePrefab;

    public float sendInterval = 1;   // 강제 갱신 주기 (sec)
    public float timer;

    public AimInput _input;
    public AimState _state;
    public float Threshold = 15;   // 목표 갱신 임계치 (deg)
    public float RotateSpeed = 45; // deg/sec

    private ReplayerRunner<AimState, AimInput> _runner;
    private GameObject _reticle;
    private Vector3 _mousePosition;
    private Camera _mainCamera;
    private Plane _playerPlane;
    private CharacterHandleWeapon _handleWeapon;
    private WeaponAim3D _weaponAim;
    private Vector3 _direction;
    private float _angle;

    public override void Init()
    {
        base.Init();

        if (hasAuthority)
        {
            _reticle = (GameObject)Instantiate(ReticlePrefab);
            _reticle.transform.SetParent(GUIManager.Instance.MainCanvas.transform);
            _reticle.transform.localScale = Vector3.one;
            if (_reticle.gameObject.MMGetComponentNoAlloc<MMUIFollowMouse>() != null)
            {
                _reticle.gameObject.MMGetComponentNoAlloc<MMUIFollowMouse>().TargetCanvas = GUIManager.Instance.MainCanvas;
            }
			_mainCamera = Camera.main;
			_playerPlane = new Plane(Vector3.up, Vector3.zero);
        }

        _handleWeapon = GetComponent<CharacterHandleWeapon>();
        _runner = new ReplayerRunner<AimState, AimInput>(this, hasAuthority, 
            Entity.renderDelay, (int tick, AimInput input) =>
        {
            _tickScheduler.ScheduleAfter(3, () =>
            {
                S_RotateStart pkt = new S_RotateStart();
                pkt.targetAngle = input.targetAngle;
                pkt.accpetTick = tick;
                DispatchPacket(pkt);
            });

            _tickScheduler.ScheduleAfter(3, () =>
            {
                NetEntity entity = NetworkManager.Instance.entitySystem.Get(1);
                S_RotateStart pkt = new S_RotateStart();
                pkt.targetAngle = input.targetAngle;
                pkt.accpetTick = tick;
                entity.DispatchPacket(NetBehaviourType.Aim, pkt);
            });

            Debug.Log("Set Angle" + _angle);
        });
    }

    public void FixedUpdate()
    {
        if (_init == false)
        {
            return;
        }

        if (_handleWeapon.CurrentWeapon != null)
        {
            _weaponAim =
                _handleWeapon.CurrentWeapon.gameObject.MMGetComponentNoAlloc<WeaponAim3D>();
        }

        if (!hasAuthority)
        {
            return;
        }

        UpdatePlane();
        GetMouseAim();

        timer += Time.deltaTime;

        // 일정 시간마다 강제 동기화
        if (timer >= sendInterval)
        {
            // 현재 target과 입력이 다르면 다시 맞춰줌
            float diff = Mathf.DeltaAngle(_state.targetAngle, _angle);

            if (Mathf.Abs(diff) > 0.1f) // 거의 동일하면 스킵
            {
                //_state.targetAngle = _angle;
                AimInput input;
                input.targetAngle = _angle;
                _runner.EnqueueClientInput(_tickScheduler.GetCurrentTick(), input, 3);
            }

            timer = 0f;
        }
    }

    // 외부 입력 (마우스, 네트워크 등)
    public bool SetTarget(float newAngle)
    {
        float diff = Mathf.DeltaAngle(_state.targetAngle, newAngle);

        // 일정 차이 이상일 때만 갱신
        if (Mathf.Abs(diff) > Threshold)
        {
            //AimInput input;
            //input.targetAngle = newAngle;
            //_runner.EnqueueClientInput(_tickScheduler.GetCurrentTick(), input);
            _state.targetAngle = newAngle;
            Debug.Log("Set Angle" + newAngle);
            
            return true;
        }

        return false;
    }

    public override void OnSpawn(int tick)
    {
        base.OnSpawn(tick);
        _tickScheduler.Register(_runner);
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        _tickScheduler.Unregister(_runner);
    }

    public override void DispatchPacket(IPacket packet)
    {
        if (packet is S_RotateStart p)
        {
            var input = new AimInput
            {
                targetAngle = p.targetAngle,
            };

            _runner.EnqueueServerInput(p.accpetTick, input);
        }
    }

    public override void OnRender(float alpha)
    {
        if (_runner.TryGetRenderPair(out AimState prev, out AimState curr))
        {
            float angle = Mathf.LerpAngle(prev.currentAngle, curr.currentAngle, alpha);
            //float prevAngle = NormalizeAngle(prev.currentAngle);
            //float currAngle = NormalizeAngle(_state.targetAngle);

            //float angle = MMMaths.Remap(0.5f, 0f, 1f, prevAngle, currAngle);
            if (_weaponAim != null)
            {
                Vector3 dir = ToVectorXZ(angle);
                dir.y = transform.position.y;
                _weaponAim.SetCurrentAim(dir);
            }
        }
    }

    public float GetAngleAt(int tick)
    {
        return _runner.GetState(tick).currentAngle;
    }

    public static float NormalizeAngle(float angle)
    {
        angle %= 360f;

        if (angle > 180f)
            angle -= 360f;
        else if (angle < -180f)
            angle += 360f;

        return angle;
    }

    public float ToAngleXZ(Vector3 v)
    {
        // v가 (0,0)일 경우 NaN 방지
        if (v.x == 0f && v.z == 0f)
            return 0f;

        return Mathf.Atan2(v.z, v.x) * Mathf.Rad2Deg;
    }

    public static Vector3 ToVectorXZ(float angleDeg)
    {
        float rad = angleDeg * Mathf.Deg2Rad;

        float x = Mathf.Cos(rad);
        float z = Mathf.Sin(rad);

        return new Vector3(x, 0f, z);
    }

    public virtual void GetMouseAim()
    {
        ComputeReticlePosition();

        _direction.y = transform.position.y;
        _direction = _direction - transform.position;

        _angle = ToAngleXZ(_direction);
    }

    protected virtual void ComputeReticlePosition()
    {
        _mousePosition = InputManager.Instance.MousePosition;
        
        Ray ray = _mainCamera.ScreenPointToRay(_mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);
        float distance;
        if (_playerPlane.Raycast(ray, out distance))
        {
            Vector3 target = ray.GetPoint(distance);
            _direction = target;
        }
    }

    protected virtual void UpdatePlane()
    {
        _playerPlane.SetNormalAndPosition (Vector3.up, _handleWeapon.WeaponAttachment.position);
    }

    public void Tick(int tick, float dt)
    {
        float diff = Mathf.DeltaAngle(_state.currentAngle, _state.targetAngle);
        float maxStep = RotateSpeed * dt;

        if (Mathf.Abs(diff) <= maxStep)
        {
            _state.currentAngle = _state.targetAngle;
        }
        else
        {
            _state.currentAngle += Mathf.Sign(diff) * maxStep;
        }
    }

    void ITickable<AimState, AimInput>.ApplyInput(in AimInput input)
    {
        _state.targetAngle = input.targetAngle;
    }

    AimState ITickable<AimState, AimInput>.CaptureState()
    {
        return _state;
    }

    void ITickable<AimState, AimInput>.RestoreState(in AimState state)
    {
        _state = state;
    }
}
