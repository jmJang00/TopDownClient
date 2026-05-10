using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManagerRevised : LevelManager 
{
    protected override IEnumerator InitializationCoroutine()
    {
        if (SpawnDelay > 0f)
        {
            yield return MMCoroutine.WaitFor(SpawnDelay);    
        }

        BoundsCollider = _collider;
        BoundsCollider2D = _collider2D;
        InstantiatePlayableCharacters();

        if (UseLevelBounds)
        {
            MMCameraEvent.Trigger(MMCameraEventTypes.SetConfiner, null, BoundsCollider, BoundsCollider2D);
        }            
        
        Initialization();

        CheckpointAssignment();

        // we trigger a fade
        MMFadeOutEvent.Trigger(IntroFadeDuration, FadeCurve, FaderID);

        // we trigger a level start event
        TopDownEngineEvent.Trigger(TopDownEngineEventTypes.LevelStart, null);
        MMGameEvent.Trigger("Load");

    }
}
