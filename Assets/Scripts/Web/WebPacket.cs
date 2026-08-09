using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateAccountPacketReq
{
	public string AccountName;
	public string Password;
    public string Nickname;
}

public enum CreateAccountError
{
    Success,
    DuplicateAccountName,
    DuplicateAccountNickName,
}

public class CreateAccountPacketRes
{
    public int ErrorCode;
}

public class LoginAccountPacketReq
{
	public string AccountName;
	public string Password;
}

[Serializable]
public class ServerInfo
{
	public string Ip;
	public int Port;
}

public class LoginAccountPacketRes
{
	public bool LoginOk;
    public string SessionKey;
	public ServerInfo ServerInfo;
}