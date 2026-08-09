using MoreMountains.TopDownEngine;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private Bullet _bulletPrefab;
    [SerializeField]
    private Picker _pickerPrefab;
    private ObjectPool<Bullet> _pool;
    private ObjectPool<Picker> _pickerPool;
    private GameScene _game;

    public void Awake()
    {
        _game = GetComponent<GameScene>();
        _pool = new ObjectPool<Bullet>(_bulletPrefab, 32, transform);
        _pickerPool = new ObjectPool<Picker>(_pickerPrefab, 32, transform);
    }

    public NetEntity SpawnAt(int tick, EntityType type, uint id, Vector3 position)
    {
        NetEntity entity = Spawn(type, id, position);

        _game.tickScheduler.ScheduleAt(tick, () =>
        {
            entity.OnSpawn(tick);
            entity.gameObject.SetActive(true);
        });

        return entity;
    }

    public void DespawnAt(int tick, uint id)
    {
        _game.tickScheduler.ScheduleAt(tick, () =>
        {
            NetEntity entity = _game.entitySystem.Get(id);
            if (entity == null)
            {
                return;
            }

            entity.OnDespawn();
            Despawn(entity.type, id);
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
                Player myPlayer = obj.GetComponent<Player>();
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
            case EntityType.MyPlayerH:
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/PlayerHitscan");
                if (prefab == null)
                {
                    Debug.Log("Can't find " + type.ToString());
                    return null;
                }

                GameObject obj = Instantiate(prefab);
                Player myPlayer = obj.GetComponent<Player>();
                myPlayer.entityId = id;
                myPlayer.type = EntityType.MyPlayerH;
                myPlayer.transform.position = position;
                myPlayer.Init();
                _game.entitySystem.Register(id, myPlayer, true);
                myPlayer.gameObject.SetActive(false);
                return myPlayer;
            }
            case EntityType.OtherPlayerH:
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/OtherPlayerHitscan");
                if (prefab == null)
                {
                    Debug.Log("Can't find " + type.ToString());
                    return null;
                }

                GameObject obj = Instantiate(prefab);
                Player player = obj.GetComponent<Player>();
                player.entityId = id;
                player.transform.position = position;
                player.type = EntityType.OtherPlayerH;
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
            case EntityType.Chest:
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/Chest");
                if (prefab == null)
                {
                    Debug.Log("Can't find " + type.ToString());
                    return null;
                }

                GameObject obj = Instantiate(prefab);
                Chest chest = obj.GetComponent<Chest>();
                chest.entityId = id;
                chest.type = EntityType.Chest;
                chest.transform.position = position;
                chest.Init();
                _game.entitySystem.Register(id, chest, true);                
                return chest;
            }
            case EntityType.HealPack:
            {
                GameObject obj = EnumToItemResource.GetPickerPrefab(ItemType.HealPack); 
                if(obj == null)
                {
                    //todo
                    return null;
                }               

                Picker picker = _pickerPool.Acquire();
                picker.picker = obj.GetComponent<DummyPicker>();
                picker.entityId = id;
                picker.transform.position = position;
                picker.type = EntityType.HealPack;
                picker.Init();
                _game.entitySystem.Register(id, picker);
                return picker;
            }
            case EntityType.ExpPack:
            {
                GameObject obj = EnumToItemResource.GetPickerPrefab(ItemType.ExpPack);
                if (obj == null)
                {
                    //todo
                    return null;
                }

                Picker picker = _pickerPool.Acquire();
                picker.picker = obj.GetComponent<DummyPicker>();
                picker.entityId = id;
                picker.transform.position = position;                
                picker.type = EntityType.ExpPack;
                picker.Init();
                _game.entitySystem.Register(id, picker);
                return picker;
            }
            case EntityType.AmmoP:
            {
                GameObject obj = EnumToItemResource.GetPickerPrefab(ItemType.AmmoP);
                if (obj == null)
                {
                    //todo
                    return null;
                }

                Picker picker = _pickerPool.Acquire();
                picker.picker = obj.GetComponent<DummyPicker>();
                picker.entityId = id;
                picker.transform.position = position;
                picker.type = EntityType.AmmoP;
                picker.Init();
                _game.entitySystem.Register(id, picker);
                return picker;
            }
            case EntityType.AmmoH:
            {
                GameObject obj = EnumToItemResource.GetPickerPrefab(ItemType.AmmoH);
                if (obj == null)
                {
                    //todo
                    return null;
                }

                Picker picker = _pickerPool.Acquire();
                picker.picker = obj.GetComponent<DummyPicker>();
                picker.entityId = id;
                picker.transform.position = position;
                picker.type = EntityType.AmmoH;
                picker.Init();
                _game.entitySystem.Register(id, picker);
                return picker;
            }
            case EntityType.Shield:
            {
                GameObject obj = EnumToItemResource.GetPickerPrefab(ItemType.Shield);
                if (obj == null)
                {
                    //todo
                    return null;
                }

                Picker picker = _pickerPool.Acquire();
                picker.picker = obj.GetComponent<DummyPicker>();
                picker.entityId = id;
                picker.transform.position = position;
                picker.type = EntityType.Shield;
                picker.Init();
                _game.entitySystem.Register(id, picker);
                return picker;
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
            case EntityType.MyPlayerH:
            {
                //entity.gameObject.SetActive(false);
                //오브젝트를 false로 설정하면 Pause화면이 나옴
                GameObject.Destroy(entity.gameObject);
                break;
            }
            case EntityType.OtherPlayerH:
            {
                //entity.gameObject.SetActive(false);
                //오브젝트를 false로 설정하면 Pause화면이 나옴
                GameObject.Destroy(entity.gameObject);
                break;
            }
            case EntityType.Projectile:
            {
                entity.gameObject.SetActive(false);
                _pool.Release((Bullet)entity);
                break;
            }
            case EntityType.Chest:
            {
                GameObject.Destroy(entity.gameObject);
                break;
            }
            case EntityType.HealPack:
            case EntityType.ExpPack:
            case EntityType.AmmoP:
            case EntityType.AmmoH:
            case EntityType.Shield:
            {
                entity.gameObject.SetActive(false);
                Picker picker = entity as Picker;
                GameObject.Destroy(picker.picker.gameObject);
                picker.picker = null;
                _pickerPool.Release(picker);                                
                break;
            }                    
        }
    }
}
