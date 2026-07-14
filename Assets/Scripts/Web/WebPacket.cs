using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateAccountPacketReq
{
	public string AccountName;
	public string Password;
}

public class CreateAccountPacketRes
{
	public bool CreateOk;
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