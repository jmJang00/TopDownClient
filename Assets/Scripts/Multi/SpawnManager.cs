using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private Bullet _bulletPrefab;
    private ObjectPool<Bullet> _pool;
    private GameScene _game;

    public void Awake()
    {
        _game = GetComponent<GameScene>();
        _pool = new ObjectPool<Bullet>(_bulletPrefab, 32, transform);
    }

    public void SpawnAt(int tick, EntityType type, uint id, Vector3 position)
    {
        NetEntity entity = Spawn(type, id, position);

        _game.tickScheduler.ScheduleAt(tick, () =>
        {
            entity.OnSpawn(tick);
            entity.gameObject.SetActive(true);
        });
    }

    public void DespawnAt(int tick, EntityType type, uint id)
    {
        _game.tickScheduler.ScheduleAt(tick, () =>
        {
            NetEntity entity = _game.entitySystem.Get(id);
            if (entity == null)
            {
                return;
            }

            entity.OnDespawn();
            Despawn(type, id);
        });
    }

    public NetEntity Spawn(EntityType type, uint id, Vector3 position)
    {
        switch (type)
        {
            case EntityType.MyPlayer:
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/Player");
                if (prefab == null)
                {
                    Debug.Log("Can't find " + type.ToString());
                    return null;
                }

                GameObject obj = Instantiate(prefab);
                MyPlayer myPlayer = obj.GetComponent<MyPlayer>();
                myPlayer.entityId = id;
                myPlayer.type = EntityType.MyPlayer;
                myPlayer.transform.position = position;
                myPlayer.Init();
                _game.entitySystem.Register(id, myPlayer, true);
                myPlayer.gameObject.SetActive(false);
                return myPlayer;
            }
            case EntityType.OtherPlayer:
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/OtherPlayer");
                if (prefab == null)
                {
                    Debug.Log("Can't find " + type.ToString());
                    return null;
                }

                GameObject obj = Instantiate(prefab);
                Player player = obj.GetComponent<Player>();
                player.entityId = id;
                player.transform.position = position;
                player.type = EntityType.OtherPlayer;
                player.Init();
                _game.entitySystem.Register(id, player);
                player.gameObject.SetActive(false);
                return player;
            }
            case EntityType.Projectile:
            {
                Bullet bullet = _pool.Acquire();
                bullet.entityId = id;
                bullet.transform.position = position;
                bullet.type = EntityType.Projectile;
                bullet.Init();
                _game.entitySystem.Register(id, bullet);
                return bullet;
            }
            default:
            {
                return null;
            }
        }
    }

    public void Despawn(EntityType type, uint id)
    {
        NetEntity entity = _game.entitySystem.Get(id);
        if (entity == null)
        {
            Debug.LogWarning("Can't find " + type.ToString());
            return;
        }

        if (entity.type != type)
        {
            Debug.LogWarning("Incorrect entityType " + type.ToString());
            return;
        }

        _game.entitySystem.Remove(id);

        switch (type)
        {
            case EntityType.MyPlayer:
            {
                //entity.gameObject.SetActive(false);
                //오브젝트를 false로 설정하면 Pause화면이 나옴
                GameObject.Destroy(entity.gameObject);
                break;
            }
            case EntityType.OtherPlayer:
            {
                GameObject.Destroy(entity.gameObject);
                break;
            }
            case EntityType.Projectile:
            {
                entity.gameObject.SetActive(false);
                _pool.Release((Bullet)entity);
                break;
            }
        }
    }
}
