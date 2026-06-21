using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class UIEventBus
{
    private static readonly Dictionary<ushort, Action<IPacket>> _handlers = new();

    public static void Subscribe(ushort protocol, Action<IPacket> handler)
    {
        if (_handlers.TryGetValue(protocol, out var existing))
        {
            _handlers[protocol] = existing + handler;
        }
        else
        {
            _handlers.Add(protocol, handler);
        }
    }

    public static void SubscribeOnce(ushort protocol, Action<IPacket> handler)
    {
        Action<IPacket> wrapper = null;

        wrapper = packet =>
        {
            Unsubscribe(protocol, wrapper);
            handler(packet);
        };

        Subscribe(protocol, wrapper);
    }

    public static void Unsubscribe(ushort protocol, Action<IPacket> handler)
    {
        if (!_handlers.TryGetValue(protocol, out var existing))
        {
            return;
        }

        existing -= handler;

        if (existing == null)
        {
            _handlers.Remove(protocol);
        }
        else
        {
            _handlers[protocol] = existing;
        }
    }

    public static void Publish(IPacket packet)
    {
        if (_handlers.TryGetValue(packet.Protocol, out var handler))
        {
            handler?.Invoke(packet);
        }
    }

    public static void Clear()
    {
        _handlers.Clear();
    }
}

/* 사용 예시
[핸들러]
public static void S_CreateMyCharacterHandler(PacketSession session, IPacket packet)
{
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
