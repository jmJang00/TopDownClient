using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class UIEventBus
{
    static Dictionary<ushort, Action<IPacket>> _handlers = new();

    public static void Subscribe(ushort protocol, Action<IPacket> handler)
    {
        _handlers[protocol] += handler;
    }

    public static void Publish(IPacket packet)
    {
        if (_handlers.TryGetValue(packet.Protocol, out var h))
            h?.Invoke(packet);
    }
}

/* 사용 예시
[핸들러]
public static void S_CreateMyCharacterHandler(PacketSession session, IPacket packet)
{
    PlayerManager.Instance.EnterGame(packet);

    UIEventBus.Publish(packet);
}

[컴포넌트]
UIEventBus.Subscribe((ushort)PacketID.S_CreateMyCharacter, OnCreateCharacter);

void OnCreateCharacter(IPacket packet)
{
    var p = (S_CreateMyCharacter)packet;
    ui.ShowSpawn(p.accountId);
}
*/
