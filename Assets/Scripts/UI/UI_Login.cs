using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_LoginScene : UI_Panel 
{
	public GameObject AccountName;
    public GameObject Password;
    public GameObject Nickname;

    public MMTouchButton CreateButton;
    public MMTouchButton LoginButton;

    public void Start()
	{
        CreateButton.ButtonReleased.AddListener(OnClickCreateButton);
        LoginButton.ButtonReleased.AddListener(OnClickLoginButton);
	}

	public async void OnClickCreateButton()
	{
		string account = AccountName.GetComponent<TMP_InputField>().text;
		string password = Password.GetComponent<TMP_InputField>().text;
        string nickname = Nickname.GetComponent<TMP_InputField>().text;

        if (string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }

		CreateAccountPacketReq packet = new CreateAccountPacketReq()
		{
			AccountName = account,
			Password = password,
            Nickname = nickname
		};

        C_ReqLoginChatServer p = new C_ReqLoginChatServer();
        var res = await NetworkManager.Instance.GameSendRequest<S_ResLoginChatServer>(p);
        if (res.loginOk)
        {

        }

		WebManager.Instance.SendPostRequest<CreateAccountPacketRes>("account/create", packet, (res) =>
		{
			Debug.Log(((CreateAccountError)res.ErrorCode).ToString());
			AccountName.GetComponent<TMP_InputField>().text = "";
			Password.GetComponent<TMP_InputField>().text = "";
            Nickname.GetComponent<TMP_InputField>().text = "";
		});
	}

	public void OnClickLoginButton()
	{
		Debug.Log("OnClickLoginButton");

		string account = AccountName.GetComponent<TMP_InputField>().text;
		string password = Password.GetComponent<TMP_InputField>().text;
        if (string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }

		LoginAccountPacketReq packet = new LoginAccountPacketReq()
		{
			AccountName = account,
			Password = password
		};

		WebManager.Instance.SendPostRequest<LoginAccountPacketRes>("account/login", packet, (res) =>
		{
			Debug.Log(res.LoginOk);
			AccountName.GetComponent<TMP_InputField>().text = "";
			Password.GetComponent<TMP_InputField>().text = "";
            Nickname.GetComponent<TMP_InputField>().text = "";

			if (res.LoginOk)
			{
				NetworkManager.Instance.ConnectToGame(res.SessionKey, res.ServerInfo.Ip, res.ServerInfo.Port);
			}
		});
	}

    public override bool RequestShow()
    {
        throw new System.NotImplementedException();
    }

    public override bool RequestHide()
    {
        throw new System.NotImplementedException();
    }
}
