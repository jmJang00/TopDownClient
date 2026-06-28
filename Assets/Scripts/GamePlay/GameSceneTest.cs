using UnityEngine;

public class GameSceneTest : GameScene
{
    public Transform[] spawnPositions;
    public PickerSpawnManager pickerSpawnManager;

    public void Start()
    {
        tickScheduler = GetComponent<TickScheduler>();
        entitySystem = GetComponent<EntitySystem>();
        spawnManager = GetComponent<SpawnManager>();
        pickerSpawnManager = GetComponent<PickerSpawnManager>();
        NetworkManager.Instance.SetGameScene(this);     
    }
    
    public void Update()
    {      

    }

}
