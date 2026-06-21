using System;
using System.Collections.Generic;
using UnityEngine;

public class TickScheduler : MonoBehaviour
{
    class ScheduledEvent : IComparable<ScheduledEvent>
    {
        public int targetTick;
        public System.Action action;
        public int id;

        public int CompareTo(ScheduledEvent other)
        {
            return targetTick - other.targetTick;
        }
    }

    public float tickRate = 20f;
    public float Alpha {  get; private set; }

    // 클라가 앞서 나갈 수 있는 허용치
    public int leadWindow = 14; // rtt + tickSync period

    private float _accum;
    private float _dt;
    [SerializeField]
    private int _currentTick;
    private bool _waitingForServer = false;

    readonly List<ITickRunner> _runners = new List<ITickRunner>();

    [SerializeField]
    private int _serverTick;     // TickSync로 갱신

    private bool _hasServerTick; // 초기 동기화 여부

    private MinHeap<ScheduledEvent> _pq = new MinHeap<ScheduledEvent>();

    private HashSet<int> _canceled = new HashSet<int>();

    private int _eventIdGen = 1;

    void Awake()
    {
        _dt = 1.0f / tickRate;
        _currentTick = 0;
    }

    public void UpdateTick(int tick)
    {
        if (!_hasServerTick)
        {
            _hasServerTick = true;
            _currentTick = tick;
        }

        _serverTick = tick;
    }

    public int ScheduleAfter(int delayTick, System.Action action)
    {
        return ScheduleAt(_currentTick + delayTick, action);
    }

    public int ScheduleAt(int tick, System.Action action)
    {
        var e = new ScheduledEvent
        {
            targetTick = tick,
            action = action,
            id = _eventIdGen++
        };

        _pq.Push(e);
        return e.id;
    }

    public void Cancel(int id)
    {
        _canceled.Add(id);
    }

    public void Register(ITickRunner runner)
    {
        _runners.Add(runner);
    }

    public void Unregister(ITickRunner runner)
    {
        _runners.Remove(runner);
    }

    public void Simulate()
    {
        if (!_hasServerTick)
        {
            Alpha = 0f;
            return;
        }

        if (!_waitingForServer)
        {
            if (_currentTick >= _serverTick + leadWindow)
            {
                _waitingForServer = true;
            }
        }
        else
        {
            // 서버가 충분히 따라왔는가?
            if (_serverTick >= _currentTick)
            {
                _waitingForServer = false;
            }
        }

        if (_waitingForServer)
        {
            Alpha = 0f;
            return;
        }

        int diff = _serverTick - _currentTick;

        float scale = 1.0f;
        // 틱 차이가 너무 많이 나면 고정
        if (diff > 10)
        {
            _accum = diff * _dt;
        }
        else
        {
            // 서버가 앞서 있음
            if (diff > 0)
            {
                if (diff >= 5)
                    scale = ((float)6 / 3);
                else if (diff >= 3)
                    scale = ((float)5 / 3);
                else if (diff >= 1)
                    scale = ((float)4 / 3);
            }
            // 클라가 앞서 있음
            // 기본적으로 클라가 앞설 수 밖에 없기 때문에 기대되는 서버 틱 간격 만큼은 1로 예측
            // 5틱마다 보낸다면 차이가 최소 5틱보다는 커야 다른 속도를 적용시킬 수 있음
            else if (diff < 0)
            {
                if (diff <= -7)
                    scale = ((float)1 / 3);
                else if (diff <= -5)
                    scale = ((float)2 / 3);
            }

            _accum += Time.deltaTime * scale;
        }

        while (_accum >= _dt)
        {
            RunTick();
            _accum -= _dt;
        }

        Alpha = Mathf.Clamp01(_accum / _dt);
    }

    void RunTick()
    {
        // 예약 이벤트 실행 (현재 tick 기준)
        while (_pq.Count > 0)
        {
            var e = _pq.Peek();
            if (e.targetTick > _currentTick)
                break;

            _pq.Pop();
            if (_canceled.Contains(e.id))
            {
                _canceled.Remove(e.id);
                continue;
            }

            e.action?.Invoke();
        }

        for (int i = 0; i < _runners.Count; i++)
        {
            _runners[i].OnTick(_currentTick, _dt);
        }

        _currentTick++;
    }

    public int GetCurrentTick()
    {
        return _currentTick;
    }

    public float GetDeltaTime()
    {
        return _dt;
    }
}
