using UnityEngine;

public class GameScene : MonoBehaviour
{
    public Transform[] spawnPositions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector3 pos = spawnPositions[0].position;
        NetworkManager.Instance.SpawnAt(
            NetworkManager.Instance.tickScheduler.GetCurrentTick(), EntityType.MyPlayer, 0, pos);

        pos.y -= 20;
        NetworkManager.Instance.SpawnAt(
            NetworkManager.Instance.tickScheduler.GetCurrentTick(), EntityType.OtherPlayer, 1, pos);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
