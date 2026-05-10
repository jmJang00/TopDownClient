using UnityEngine;
public interface ITickable<TState, TInput>
{
    void ApplyInput(in TInput input);
    void Tick(int tick, float dt);
    TState CaptureState();
    void RestoreState(in TState state);
}
