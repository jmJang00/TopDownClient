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

        for (int i = 0; i < 3; i++)
        {
            C_MoveStart start = new C_MoveStart();
            start.clientTick = 0;
            start.targetX = 1;
            start.targetY = 2;
            NetworkManager.Instance.Send(start.Write());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
