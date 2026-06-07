using JetBrains.Annotations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ReplayerRunner<TState, TInput> : ITickRunner
{
    private Replayer<TState, TInput> _replayer;
    private bool _hasPending;
    private bool _hasAuthority;
    private int _pendingTick;
    private TInput _pendingInput;
    private int _latestTick;
    private int _lastClientTick;
    private int _serverAckTick;
    private int _renderDelay;
    private Action<int, TInput> _onAppliedAction;

    public struct PendingInput
    {
        public int tick;
        public TInput input;
    }

    public struct PendingState
    {
        public int tick;
        public TState state;
    }

    // 틱 순서대로 들어간다고 가정
    private Queue<PendingInput> _clientPendingInputs;
    private Queue<PendingInput> _serverPendingInputs;
    private Queue<PendingState> _serverPendingStates;

    public ReplayerRunner(ITickable<TState, TInput> target, bool hasAuthority, int renderDelay, Action<int, TInput> onApplied)
    {
        _replayer = new Replayer<TState, TInput>(target);
        _hasAuthority = hasAuthority;
        _renderDelay = renderDelay;
        _onAppliedAction = onApplied;
        _latestTick = 0;
        _serverAckTick = -1;
        _hasPending = false;
        _pendingTick = 0;
        _lastClientTick = 0;
        _clientPendingInputs = new Queue<PendingInput>();
        _serverPendingInputs = new Queue<PendingInput>();
        _serverPendingStates = new Queue<PendingState>();
    }

    public void EnqueueClientInput(int tick, in TInput input, int coolTime = 0)
    {
        if (_hasAuthority)
        {
            // 다음 틱 시뮬레이션까지 하나의 인풋만 가능
            if (tick >= _lastClientTick + coolTime)
            {
                _hasPending = true;
                _pendingTick = tick;
                _pendingInput = input;
                _lastClientTick = tick;
            }
        }
    }

    public void EnqueueServerInput(int tick, in TInput input)
    {
        _serverPendingInputs.Enqueue(new PendingInput { tick = tick, input = input });
    }

    public void EnqueueServerState(int tick, in TState state)
    {
        _serverPendingStates.Enqueue(new PendingState { tick = tick, state = state });
    }

    public void OnTick(int tick, float dt)
    {
        ProcessInput(tick, dt);
        _replayer.Tick(tick, dt);
        _latestTick = tick;
    }

    void ProcessServerInput(int currentTick, ref int rollbackTick)
    {
        // 서버로부터 온 입력을 우선 적용
        while (_serverPendingInputs.Count > 0)
        {
            PendingInput serverPending = _serverPendingInputs.Peek();
            // 클라이언트가 너무 뒤쳐지는 경우 서버 입력이 버퍼를 덮어씌우는 걸 막는다
            if (_serverAckTick > currentTick + Replayer<TState, TInput>.BUFFER_SIZE / 2)
            {
                break;
            }

            int minTick = int.MaxValue;

            if (_hasAuthority)
            {
                if (_clientPendingInputs.Count == 0)
                {
                    throw new Exception("Enqueue server input failed");
                    // 클라이언트가 패킷을 보내는 건 클라이언트 입력이 선반영되는 시점
                    // 보내지도 않은 패킷에 대해서 서버 입력 응답이 오는 건 오류
                }
                else
                {
                    PendingInput clientPending = _clientPendingInputs.Dequeue();
                    if (clientPending.tick != serverPending.tick)
                    {
                        minTick = Mathf.Min(minTick, clientPending.tick);
                        _replayer.RemoveInput(clientPending.tick);

                        // 요청이 취소된 경우가 아니라면
                        if (serverPending.tick != -1)
                        {
                            minTick = Mathf.Min(minTick, serverPending.tick);
                            _replayer.AddInput(serverPending.tick, serverPending.input);

                        }
                    }
                }
            }
            else
            {
                Debug.Assert(serverPending.tick != -1, "Invalid server pending tick");

                minTick = Mathf.Min(minTick, serverPending.tick);
                _replayer.AddInput(serverPending.tick, serverPending.input);
            }

            if (minTick < currentTick)
            {
                rollbackTick = Mathf.Min(rollbackTick, minTick);
            }

            _serverAckTick = Mathf.Max(_serverAckTick, serverPending.tick);

            _serverPendingInputs.Dequeue();
        }
    }

    bool ProcessServerSnapshots(int currentTick, ref int snapshotTick, ref TState snapshot)
    {
        bool hasSnapshot = false;

        while (_serverPendingStates.Count > 0)
        {
            PendingState serverPending = _serverPendingStates.Peek();

            // 클라이언트가 너무 뒤쳐지는 경우 서버 입력이 버퍼를 덮어씌우는 걸 막는다
            if (_serverAckTick > currentTick + Replayer<TState, TInput>.BUFFER_SIZE)
            {
                break;
            }

            // 렌더틱 이전의 가장 가까운 서버 스냅샷을 찾는다
            if (serverPending.tick <= currentTick - _renderDelay)
            {
                if (serverPending.tick > snapshotTick)
                {
                    snapshot = serverPending.state;
                    snapshotTick = serverPending.tick;
                    hasSnapshot = true;
                }
            }
            else
            {
                // 미래에 있는 스냅샷은 나중에 적용하기 위해 남겨둔다
                // 미래에 스냅샷이 있는 경우는 클라이언트가 뒤쳐져 있다는 뜻
                // 서버에 입력을 보내도 서버가 받아들여주는 건 이후의 틱 뿐이기 때문에
                // 한 번 사용된 스냅샷은 더 이상 사용되지 않는다
                break;
            }

            _serverAckTick = Mathf.Max(_serverAckTick, serverPending.tick);

            _serverPendingStates.Dequeue();
        }

        return hasSnapshot;
    }

    void ProcessClientInput(int currentTick, ref int rollbackTick)
    {
        // 클라이언트의 입력을 처리
        if (_hasAuthority && _hasPending)
        {
            // 현재 입력을 처리할 때, 서버의 ack 틱보다 작을 수는 없다
            if (_pendingTick > _serverAckTick)
            {
                _hasPending = false;
                _replayer.AddInput(_pendingTick, _pendingInput);
                _clientPendingInputs.Enqueue(
                    new PendingInput { tick = _pendingTick, input = _pendingInput });
                _onAppliedAction(_pendingTick, _pendingInput);

                // 서버로부터 온 입력이 없을 때만 기준으로 삼는다
                if (_pendingTick < currentTick)
                {
                    rollbackTick = Mathf.Min(rollbackTick, _pendingTick);
                }
            }
            else 
            {
                // 틱을 미루고 클라이언트가 서버의 ack 틱을 따라잡을 때까지 입력을 보류
                _pendingTick = _serverAckTick;
            }
        }
    }

    void ProcessInput(int currentTick, float dt)
    {
        int snapshotTick = int.MinValue;
        int rollbackTick = int.MaxValue;
        TState snapshot = default;
        bool hasSnapshot = false;

        ProcessServerInput(currentTick, ref rollbackTick);

        hasSnapshot = ProcessServerSnapshots(currentTick, ref snapshotTick, ref snapshot);

        ProcessClientInput(currentTick, ref rollbackTick);

        // 마지막 스냅샷만 복원해서 시뮬레이션 틱을 줄인다
        if (hasSnapshot)
        {
            _replayer.OverwriteState(snapshotTick, snapshot);
            _replayer.ReSimulate(snapshotTick + 1, dt);
        }
        else if (rollbackTick != int.MaxValue)
        {
            _replayer.ReSimulate(rollbackTick, dt);
        }
    }

    public bool TryGetRenderPair(out TState prev, out TState curr)
    {
        return _replayer.TryGetRenderPair(_latestTick - _renderDelay, out prev, out curr);
    }

    public TState GetState(int tick)
    {
        return _replayer.GetState(tick);
    }

    public int CollectInputs(int fromTick, int toTick,
        int[] outTicks, TInput[] outInputs, int maxCount)
    {
        return _replayer.CollectInputs(fromTick, toTick, outTicks, outInputs, maxCount);
    }
}
