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

        // 세이브 로드 하면 안 됨
        //MMGameEvent.Trigger("Load");
    }

    public override void TriggerEndLevelEvents()
    {
        TopDownEngineEvent.Trigger(TopDownEngineEventTypes.LevelEnd, null);

        // 세이브하면 안 됨
        //MMGameEvent.Trigger("Save");
    }
}
