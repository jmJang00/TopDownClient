using UnityEngine;
using System.Collections.Generic;

public class PlayerInfo
{
    public long AccountId;
    public string Nickname;
}

public class AccountManager : MonoBehaviour
{
    private static AccountManager s_instance;

    public static AccountManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                Init();
            }

            return s_instance;
        }
    }

    public int AccountId { get; private set; }
    public string Nickname { get; private set; }

    private readonly Dictionary<long, PlayerInfo> _players = new();
    private readonly Dictionary<string, long> _accountIds = new();

    public static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("AccountManager");
            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<AccountManager>();
        }
    }

    public void SetAccount(int accountId, string nickname)
    {
        AccountId = accountId;
        Nickname = nickname;
    }

    public void AddPlayer(PlayerInfo info)
    {
        _players[info.AccountId] = info;
        _accountIds[info.Nickname] = info.AccountId;
    }

    public void SetPlayers(List<PlayerInfo> players)
    {
        _players.Clear();
        _accountIds.Clear();

        foreach (var player in players)
        {
            _players[player.AccountId] = player;
            _accountIds[player.Nickname] = player.AccountId;
        }
    }

    public bool TryGetPlayer(long accountId, out PlayerInfo info)
    {
        return _players.TryGetValue(accountId, out info);
    }

    public string GetNickname(long accountId)
    {
        if (_players.TryGetValue(accountId, out var info))
        {
            return info.Nickname;
        }

        return string.Empty;
    }

    public bool TryGetAccountId(string nickname, out long accountId)
    {
        return _accountIds.TryGetValue(nickname, out accountId);
    }

    public void Clear()
    {
        _players.Clear();
        _accountIds.Clear();
    }
}
