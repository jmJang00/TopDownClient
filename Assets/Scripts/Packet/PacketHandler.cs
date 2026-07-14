using MoreMountains.InventoryEngine;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class PacketHandler
{
    internal static void S_ResLoginGameServerHandler(PacketSession session, IPacket packet)
    {
        UIEventBus.Publish(packet);
    }

    public static void S_NtfCreateMyCharacterHandler(PacketSession session, IPacket packet)
    {
        S_NtfCreateMyCharacter pkt = packet as S_NtfCreateMyCharacter;

        if (NetworkManager.Instance.game)
        {
             MyPlayer player = null;
            if ((WeaponType)pkt.weaponId == WeaponType.Rifle)
            {
                player = NetworkManager.Instance.spawnManager.SpawnAt(pkt.serverTick, EntityType.MyPlayer, pkt.entityId, new Vector3(0, 2, 0)) as MyPlayer;

            }
            else
            {
                player = NetworkManager.Instance.spawnManager.SpawnAt(pkt.serverTick, EntityType.MyPlayerH, pkt.entityId, new Vector3(0, 2, 0)) as MyPlayer;
            }

            player.gameObject.GetComponent<PlayerHealth>().SetHealth(pkt.hp);
        }
    }

    public static void S_NtfCreateOtherCharacterHandler(PacketSession session, IPacket packet)
    {
        S_NtfCreateOtherCharacter pkt = packet as S_NtfCreateOtherCharacter;

        if (NetworkManager.Instance.game)
        {
            Player player = null;
            if ((WeaponType)pkt.weaponId == WeaponType.Rifle)
            {
                player = NetworkManager.Instance.spawnManager.SpawnAt(pkt.serverTick, EntityType.OtherPlayer, pkt.entityId, new Vector3(0, 2, 0)) as Player;

            }
            else
            {
                player = NetworkManager.Instance.spawnManager.SpawnAt(pkt.serverTick, EntityType.OtherPlayerH, pkt.entityId, new Vector3(0, 2, 0)) as Player;
            }

            player.gameObject.GetComponent<PlayerHealth>().SetHealth(pkt.hp);
        }
    }

    public static void S_NtfDeleteCharacterHandler(PacketSession session, IPacket packet)
    {
        S_NtfDeleteCharacter pkt = packet as S_NtfDeleteCharacter;

        if (NetworkManager.Instance.game)
        {
            if (NetworkManager.Instance.entitySystem.MyCharacter.entityId == pkt.entityId)
            {
                NetworkManager.Instance.spawnManager.DespawnAt(pkt.serverTick, EntityType.MyPlayer, pkt.entityId);
            }
            else
            {
                NetworkManager.Instance.spawnManager.DespawnAt(pkt.serverTick, EntityType.OtherPlayer, pkt.entityId);
            }
        }
    }

    public static void S_MoveStartHandler(PacketSession session, IPacket packet)
    {
        S_MoveStart pkt = packet as S_MoveStart;


        if (NetworkManager.Instance.game)
        {
            NetEntity entity = NetworkManager.Instance.entitySystem.Get(pkt.entityId);
            entity.DispatchPacket(NetBehaviourType.Controller, packet);
        }

    }

    public static void S_NtfTickSyncHandler(PacketSession session, IPacket packet)
    {
        S_NtfTickSync pkt = packet as S_NtfTickSync;

        if (NetworkManager.Instance.game)
        {
            NetworkManager.Instance.tickScheduler.UpdateTick(pkt.serverTick);
        }
    }

    public static void S_RotateStartHandler(PacketSession session, IPacket packet)
    {
        S_RotateStart pkt = packet as S_RotateStart;


        if (NetworkManager.Instance.game)
        {
            NetEntity entity = NetworkManager.Instance.entitySystem.Get(pkt.entityId);
            entity.DispatchPacket(NetBehaviourType.Aim, packet);
        }
    }

    internal static void S_NtfMoveStateHandler(PacketSession session, IPacket packet)
    {
        S_NtfMoveState pkt = packet as S_NtfMoveState;

        if (NetworkManager.Instance.game)
        {
            NetEntity entity = NetworkManager.Instance.entitySystem.Get(pkt.entityId);
            entity.DispatchPacket(NetBehaviourType.Controller, packet);
        }
    }

    internal static void S_NtfRotateStateHandler(PacketSession session, IPacket packet)
    {
        S_NtfRotateState pkt = packet as S_NtfRotateState;

        if (NetworkManager.Instance.game)
        {
            NetEntity entity = NetworkManager.Instance.entitySystem.Get(pkt.entityId);
            entity.DispatchPacket(NetBehaviourType.Aim, packet);
        }

    }

    internal static void S_ShootStartHandler(PacketSession session, IPacket packet)
    {
        S_ShootStart pkt = packet as S_ShootStart;

        if (NetworkManager.Instance.game)
        {
            NetEntity entity = NetworkManager.Instance.entitySystem.Get(pkt.entityId);
            entity.DispatchPacket(NetBehaviourType.Projectile, packet);
        }

    }

    internal static void S_NtfSpawnProjectileHandler(PacketSession session, IPacket packet)
    {
        S_NtfSpawnProjectile pkt = packet as S_NtfSpawnProjectile;

        if (NetworkManager.Instance.game)
        {
            NetworkManager.Instance.spawnManager.SpawnAt(pkt.currentTick, EntityType.Projectile, pkt.entityId, Vector2.zero);
            NetEntity entity = NetworkManager.Instance.entitySystem.Get(pkt.entityId);
            entity.DispatchPacket(NetBehaviourType.BulletMovement, packet);
        }
    }

    internal static void S_NtfDespawnProjectileHandler(PacketSession session, IPacket packet)
    {
        S_DespawnProjectile pkt = packet as S_DespawnProjectile;

        if (NetworkManager.Instance.game)
        {
            NetworkManager.Instance.spawnManager.DespawnAt(pkt.currentTick, EntityType.Projectile, pkt.entityId);
        }
    }

    internal static void S_NtfProjectileHitHandler(PacketSession session, IPacket packet)
    {
        S_NtfProjectileHit pkt = packet as S_NtfProjectileHit;

        if (NetworkManager.Instance.game)
        {
            NetEntity bullet = NetworkManager.Instance.entitySystem.Get(pkt.bulletId);
            bullet.DispatchPacket(NetBehaviourType.BulletMovement, packet);

            NetEntity collision = NetworkManager.Instance.entitySystem.Get(pkt.collisionId);
            collision.DispatchPacket(NetBehaviourType.Health, packet);
        }
    }

    internal static void S_NtfSpawnItemPickerHandler(PacketSession session, IPacket packet)
    {
        S_NtfSpawnItemPicker pkt = packet as S_NtfSpawnItemPicker;

        if (NetworkManager.Instance.game)
        {
            NetworkManager.Instance.spawnManager.SpawnAt(pkt.serverTick, (EntityType)pkt.itemType, pkt.entityId, new Vector3(pkt.targetX, 1, pkt.targetY));
        }        
    }

    internal static void S_NtfDespawnItemPickerHandler(PacketSession session, IPacket packet)
    {
        S_NtfDespawnItemPicker pkt = packet as S_NtfDespawnItemPicker;

        if (NetworkManager.Instance.game)
        {
            NetworkManager.Instance.spawnManager.Despawn((EntityType)pkt.itemType, pkt.entityId);
        }
    }

    internal static void C_ReqPickupItemPickerHandler(PacketSession session, IPacket packet)
    {

    }

    internal static void S_GameStartHandler(PacketSession session, IPacket packet)
    {
        S_GameStart pkt = packet as S_GameStart;
        if (NetworkManager.Instance.game)
        {
            NetworkManager.Instance.tickScheduler.UpdateTick(pkt.tick);
            NetworkManager.Instance.OnGameStart(pkt.success);
        }
    }

    internal static void S_GameEndHandler(PacketSession session, IPacket packet)
    {
        S_GameEnd pkt = packet as S_GameEnd;
        NetworkManager.Instance.OnGameEnd(pkt.win);
    }

    internal static void S_MatchFoundHandler(PacketSession session, IPacket packet)
    {
        S_MatchFound pkt = packet as S_MatchFound;
        UIEventBus.Publish(packet);
        NetworkManager.Instance.OnGameFound(pkt.success);
    }

    internal static void S_ReturnToLobbyHandler(PacketSession session, IPacket packet)
    {
        NetworkManager.Instance.OnReturnToLobby();

    }

    internal static void S_NtfCreateChestHandler(PacketSession session, IPacket packet)
    {
        S_NtfCreateChest pkt = packet as S_NtfCreateChest;

        if (NetworkManager.Instance.game)
        {
            NetworkManager.Instance.spawnManager.SpawnAt(pkt.serverTick, EntityType.Chest, pkt.entityId, new Vector3(pkt.targetX, 1.0f, pkt.targetY));
        }
    }

    internal static void S_NtfDestroyChestHandler(PacketSession session, IPacket packet)
    {
        S_NtfDestroyChest pkt = packet as S_NtfDestroyChest;

        if (NetworkManager.Instance.game)
        {
            NetworkManager.Instance.spawnManager.DespawnAt(pkt.serverTick, EntityType.Chest, pkt.entityId);
        }
    }

    internal static void S_ResChestInfoHandler(PacketSession session, IPacket packet)
    {
        S_ResChestInfo pkt = packet as S_ResChestInfo;

        MyChestInventoryManager manager = MyChestInventoryManager.Instance;
        if (manager == null)
        {
            //아직 매니저 초기화 안됨.
            return;
        }

        InventoryItem[] items = new InventoryItem[pkt.itemLists.Count];
        for (int i = 0; i < pkt.itemLists.Count; ++i)
        {
            items[i] = EnumToItemResource.GetNewInventoryItem((EntityType)pkt.itemLists[i].itemType);
            items[i].Quantity = (int)pkt.itemLists[i].quantity;
        }

        manager.CurrentChestInventory.SetInventoryFromItemArray(items);
       // MMInventoryEvent.Trigger(MMInventoryEventType.Redraw, null, $"Chest{pkt.chestId}Inventory", null, 0, 0, "Player1");
    }

    internal static void S_ResInventoryToChestHandler(PacketSession session, IPacket packet)
    {

    }

    internal static void S_ResChestToInventoryHandler(PacketSession session, IPacket packet)
    {

    }

    internal static void S_ResInventoryInfoHandler(PacketSession session, IPacket packet)
    {
        S_ResInventoryInfo pkt = packet as S_ResInventoryInfo;


        MyChestInventoryManager manager = MyChestInventoryManager.Instance;
        if (manager == null)
        {
            //아직 매니저 초기화 안됨.
            return;
        }

        InventoryItem[] items = new InventoryItem[pkt.itemLists.Count];
        for (int i = 0; i < pkt.itemLists.Count; ++i)
        {
            items[i] = EnumToItemResource.GetNewInventoryItem((EntityType)pkt.itemLists[i].itemId);
            items[i].Quantity = (int)pkt.itemLists[i].quantity;
        }

        manager.CurrentPlayerInventory.SetInventoryFromItemArray(items);
    }
}
