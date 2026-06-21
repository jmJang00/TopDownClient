using UnityEngine;

public class GameScene : MonoBehaviour
{
    public TickScheduler tickScheduler;
    public EntitySystem entitySystem;
    public SpawnManager spawnManager;
    public UI_GameEnd gameSelectUI;

    public void Start()
    {
        tickScheduler = GetComponent<TickScheduler>();
        entitySystem = GetComponent<EntitySystem>();
        spawnManager = GetComponent<SpawnManager>();
        NetworkManager.Instance.SetGameScene(this);
    }

    public void ProcessUpdate()
    {
        tickScheduler.Simulate();

        entitySystem.RunRender(tickScheduler.Alpha);
    }

    public void Clear()
    {
        if (entitySystem.MyCharacter)
        {
            GameObject.Destroy(entitySystem.MyCharacter.gameObject);
        }
        entitySystem.Clear();
    }
}
