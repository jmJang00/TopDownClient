using MoreMountains.Feedbacks;
using UnityEngine;

public enum PlayerDirection : byte
{
    LL,
    LU,
    UU,
    RU,
    RR,
    RD,
    DD,
    LD,
}

public class PlayerInput : MonoBehaviour
{
    public PlayerController controller;

    public float moveSpeed = 5.0f;
    public float predictTime = 1.0f;
    public float updateInterval = 1.0f;

    private float _updateTimer;
    private bool _wasInput;
    private PlayerDirection _prevDir;

    public static readonly float[] dirAngle = new float[]
    {
        180,
        135,
        90,
        45,
        0,
        315,
        270,
        225,
    };

    public static readonly Vector2[] dirs = new Vector2[]
    {
        new Vector2(-1f,  0f), // LL
        new Vector2(-0.71f,  0.71f), // LU
        new Vector2( 0f,  1f), // UU
        new Vector2( 0.71f,  0.71f), // RU
        new Vector2( 1f,  0f), // RR
        new Vector2( 0.71f, -0.71f), // RD
        new Vector2( 0f, -1f), // DD
        new Vector2(-0.71f, -0.71f), // LD
    };

    public static PlayerDirection ToDirection(Vector2 v)
    {
        if (v.sqrMagnitude < 1e-6f)
            return PlayerDirection.UU;

        float maxDot = float.MinValue;
        int best = 0;

        // 루프 대신 직접 언롤하면 분기/인덱스 비용 조금 더 줄일 수 있음
        for (int i = 0; i < 8; ++i)
        {
            float d = v.x * dirs[i].x + v.y * dirs[i].y;
            if (d > maxDot)
            {
                maxDot = d;
                best = i;
            }
        }

        return (PlayerDirection)best;
    }

    public void Start()
    {
        controller = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (!controller.Ready)
        {
            return;
        }

        float dt = Time.deltaTime;
        _updateTimer += dt;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector2 input = new Vector2(h, v);
        bool hasInput = input.sqrMagnitude > 0.0001f;

        if (hasInput)
        {
            // 일정 주기마다만 타겟 갱신
            input.Normalize();
            PlayerDirection dir = ToDirection(input);
            Vector2 d = dirs[(int)dir].normalized;

            bool shouldUpdate = false;

            // 1. 주기 조건
            if (_updateTimer >= updateInterval)
            {
                shouldUpdate = true;
            }
            else
            {
                if (dir != _prevDir)
                {
                    shouldUpdate = true;
                }
            }

            if (shouldUpdate)
            {
                _updateTimer = 0.0f;
                _prevDir = dir;

                Vector2 current = controller.GetServerPosition();
                Vector2 target = current + d * moveSpeed * predictTime;

                controller.SetMoveTarget(_prevDir, target);
                Debug.Log("Start Target " + controller.Target);
            }
        }
        else
        {
            // 입력 끊긴 순간 1회
            if (_wasInput)
            {
                var d = dirs[(int)_prevDir].normalized;

                Vector2 current = controller.GetServerPosition();
                Vector2 target = current + d * moveSpeed * 0.3f;

                controller.SetMoveTarget(_prevDir, target);
                Debug.Log("Stop Target " + controller.Target);
            }

            _updateTimer = updateInterval; // 다음 입력 들어오면 바로 반응하도록
        }

        _wasInput = hasInput;
    }
}

