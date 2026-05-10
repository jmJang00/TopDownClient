using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager
{
    public MyPlayer MyPlayer;
    private Dictionary<uint, Player> _players = new();

    public static PlayerManager Instance { get; } = new();

    public Dictionary<ushort, System.Action> UserHandler = new();

    public Player FindPlayer(uint accountId)
    {
        if (_players.TryGetValue(accountId, out Player player))
        {
            return player;
        }
        else
        {
            return null;
        }
    }

    public void EnterGame(S_CreateMyCharacter packet)
    {
        if (packet.accountId == MyPlayer.AccountId)
            return;

        GameObject go = new GameObject { name = "user" };

        MyPlayer player = go.GetComponent<MyPlayer>();
        MyPlayer = player;
    }

    public void EnterGame(S_CreateOtherCharacter packet)
    {
        if (packet.accountId == MyPlayer.AccountId)
            return;

        GameObject go = new GameObject { name = "user" };

        Player player = go.GetComponent<Player>();
        _players.Add(packet.accountId, player);
    }

    public void LeaveGame(S_DeleteCharacter packet)
    {
        if (MyPlayer.AccountId == packet.accountId)
        {
            GameObject.Destroy(MyPlayer.gameObject);
            MyPlayer = null;
        }
        else
        {
            Player player = null;
            if (_players.TryGetValue(packet.accountId, out player))
            {
                GameObject.Destroy(player.gameObject);
                _players.Remove(packet.accountId);
            }
        }
    }
}
