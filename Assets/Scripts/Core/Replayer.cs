using System;
using System.Runtime.CompilerServices;

public class Replayer<TState, TInput>
{
    public static readonly int BUFFER_SIZE = 128;              // 반드시 2의 제곱

    struct Slot
    {
        public int tick;
        public TState state;

        public TInput input;
        public bool hasInput;
    }

    private readonly Slot[] _buffer = new Slot[BUFFER_SIZE];
    private readonly ITickable<TState, TInput> _target;
    private int _latestTick;

    public Replayer(ITickable<TState, TInput> target)
    {
        _target = target;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    int Mask(int tick) => tick & (BUFFER_SIZE - 1);

    // 입력 추가
    public void AddInput(int tick, in TInput input)
    {
        ref var slot = ref _buffer[Mask(tick)];

        slot.tick = tick;
        slot.input = input;
        slot.hasInput = true;
    }

    public bool RemoveInput(int tick)
    {
        ref var slot = ref _buffer[Mask(tick)];
        if (slot.tick == tick && slot.hasInput)
        {
            slot.hasInput = false;
            return true;
        }

        return false;
    }

    public TState GetState(int tick)
    {
        ref var slot = ref _buffer[Mask(tick)];
        if (slot.tick == tick)
        {
            return slot.state;
        }

        return default;
    }

    public void OverwriteState(int tick, in TState state)
    {
        ref var slot = ref _buffer[Mask(tick)];
        slot.state = state;
        slot.tick = tick;
    }

    // 일반 Tick 진행
    public void Tick(int tick, float dt)
    {
        ref var slot = ref _buffer[Mask(tick)];

        if (slot.tick == tick)
        {
            if (slot.hasInput)
            {
                _target.ApplyInput(slot.input);
            }
        }
        else
        {
            if (slot.hasInput)
            {
                // 이전 Input을 무효화
                slot.hasInput = false;
            }
        }

        _target.Tick(tick, dt);

        slot.tick = tick;
        slot.state = _target.CaptureState();

        _latestTick = tick;
    }

    public bool TryGetRenderPair(int renderTick, out TState prev, out TState curr)
    {
        ref var currSlot = ref _buffer[Mask(renderTick)];
        if (currSlot.tick != renderTick)
        {
            prev = default;
            curr = default;
            return false;
        }

        curr = currSlot.state;
        ref var prevSlot = ref _buffer[Mask(renderTick - 1)];
        if (prevSlot.tick != renderTick - 1)
        {
            prev = curr;
        }
        else
        {
            prev = prevSlot.state;
        }
        return true;
    }

    // 과거 입력 도착 시 호출
    public void ReSimulate(int fromTick, float dt)
    {
        //if (fromTick > _latestTick)
        //    return;

        int start = fromTick - 1;

        ref var baseSlot = ref _buffer[Mask(start)];

        if (baseSlot.tick != start)
            return; // snapshot 없음 → 포기 or full sync

        // 2. 상태 복원
        _target.RestoreState(baseSlot.state);

        // 3. replay
        for (int t = start + 1; t <= _latestTick; t++)
        {
            ref var slot = ref _buffer[Mask(t)];

            if (slot.tick == t && slot.hasInput)
            {
                _target.ApplyInput(slot.input);
            }

            _target.Tick(t, dt);

            // 재시뮬 중에도 snapshot 유지
            slot.state = _target.CaptureState();
            slot.tick = t;
        }
    }

    public int CollectInputs(int fromTick, int toTick,
        int[] outTicks, TInput[] outInputs, int maxCount)
    {
        int count = 0;

        if (toTick > _latestTick)
            toTick = _latestTick;

        for (int t = fromTick; t <= toTick; ++t)
        {
            ref var slot = ref _buffer[Mask(t)];

            if (slot.tick == t && slot.hasInput)
            {
                if (count >= maxCount)
                    break;

                outTicks[count] = t;
                outInputs[count] = slot.input;
                count++;
            }
        }

        return count;
    }
}
