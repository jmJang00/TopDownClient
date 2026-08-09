using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager s_instance;

    public static PlayerManager Instance => s_instance;

    private readonly Dictionary<uint, Player> _players = new();

    public Player LocalPlayer { get; private set; }


    private void Awake()
    {
        s_instance = this;
    }


    public void AddPlayer(Player player)
    {
        _players[player.entityId] = player;

        if (player.AccountId == AccountManager.Instance.AccountId)
        {
            LocalPlayer = player;
        }
    }


    public bool TryGetPlayer(uint entityId, out Player player)
    {
        return _players.TryGetValue(entityId, out player);
    }


    public void RemovePlayer(uint entityId)
    {
        if (_players.TryGetValue(entityId, out var player))
        {
            _players.Remove(entityId);

            if (player == LocalPlayer)
            {
                LocalPlayer = null;
            }
        }
    }


    public void ClearPlayers()
    {
        _players.Clear();
        LocalPlayer = null;
    }
}
