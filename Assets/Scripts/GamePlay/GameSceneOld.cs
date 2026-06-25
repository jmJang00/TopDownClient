using UnityEngine;

public class GameSceneOld : GameScene
{
    public Transform[] spawnPositions;

    public void Start()
    {
        tickScheduler = GetComponent<TickScheduler>();
        entitySystem = GetComponent<EntitySystem>();
        spawnManager = GetComponent<SpawnManager>();
        NetworkManager.Instance.SetGameScene(this);

        Vector3 pos = spawnPositions[0].position;
        spawnManager.SpawnAt(
            tickScheduler.GetCurrentTick(), EntityType.MyPlayerH, 0, pos);

        pos.y -= 20;
        spawnManager.SpawnAt(
            tickScheduler.GetCurrentTick(), EntityType.OtherPlayerH, 1, pos);


    }

    public int tickCount = 0;
    public float tickRate = 20f;
    public float remainTick = 0.15f;
    public void Update()
    {
        remainTick += Time.deltaTime;

        float interval = 1f / tickRate;

        while (remainTick >= interval)
        {
            remainTick -= interval;
            tickScheduler.UpdateTick(tickCount++);
        }        
    }

}
