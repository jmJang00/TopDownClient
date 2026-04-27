using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class PacketHandler
{
    public static void S_CreateMyCharacterHandler(PacketSession session, IPacket packet)
    {
        //S_CreateMyCharacter pkt = packet as S_CreateMyCharacter;
        //ServerSession serverSession = session as ServerSession;

        //PlayerManager.Instance.EnterGame(pkt);
    }

    public static void S_CreateOtherCharacterHandler(PacketSession session, IPacket packet)
    {
        //S_CreateOtherCharacter pkt = packet as S_CreateOtherCharacter;
        //ServerSession serverSession = session as ServerSession;

        //PlayerManager.Instance.EnterGame(pkt);
    }

    public static void S_DeleteCharacterHandler(PacketSession session, IPacket packet)
    {
        //S_DeleteCharacter pkt = packet as S_DeleteCharacter;
        //ServerSession serverSession = session as ServerSession;

        //PlayerManager.Instance.LeaveGame(pkt);
    }

    public static void S_MoveStartHandler(PacketSession session, IPacket packet)
    {
        //S_MoveStart pkt = packet as S_MoveStart;
        //ServerSession serverSession = session as ServerSession;

        //if (pkt.accountId != PlayerManager.Instance.MyPlayer.AccountId)
        //{
        //    Player player = PlayerManager.Instance.FindPlayer(pkt.accountId);
        //    player.MoveStart(pkt);
        //}
    }
}
