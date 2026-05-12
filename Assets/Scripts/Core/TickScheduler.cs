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
    public int leadWindow = 2 + 10; // rtt + tickSync period

    private float _accum;
    private float _dt;
    [SerializeField]
    private int _currentTick;

    readonly List<ITickRunner> _runners = new List<ITickRunner>();

    [SerializeField]
    private int _serverTick;     // TickSync로 갱신
    public int _tickDiff;
    private bool _serverInput;

    private bool _hasServerTick; // 초기 동기화 여부

    private MinHeap<ScheduledEvent> _pq
        = new MinHeap<ScheduledEvent>();

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
        _serverInput = true;
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
        //if (!_hasServerTick)
        //{
        //    Alpha = 0f;
        //    return;
        //}

        //if (_serverTick > _currentTick)
        //{
        //    _accum += _dt * (_serverTick - _currentTick);
        //}
        //else
        //{
            _accum += Time.deltaTime;
        //}

        while (_accum >= _dt)
        {
            //if (!CanAdvance())
            //    break;

            RunTick();
            _accum -= _dt;
        }

        //_tickDiff = _currentTick - _serverTick;
        Alpha = Mathf.Clamp01(_accum / _dt);
    }

    bool CanAdvance()
    {
        // 클라가 너무 앞서지 못하게 제한
        return _currentTick < _serverTick + leadWindow;
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
