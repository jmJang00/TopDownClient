using System;
using TMPro;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    public int signupresult = 0; //0: not implemented, 1: success, -1: fail
    [SerializeField] UserSession usersession;
    enum RQResult
    {       
        Success = 0,
        Fail = -1
    }   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnEnable()
    {
       //UIEventBus.Subscribe();
    }

    public void TrySignUp(string id, string pw, string nickname,Action<int> onResult)
    {        
        Debug.Log($"TrySignUp id:{id}, pw:{pw}, nickname:{nickname}");       
               
        string account = id;
        string password = pw;
        if (string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(nickname) || string.IsNullOrWhiteSpace(password)) 
        {
            onResult?.Invoke(-1);
            return ;
        }

        CreateAccountPacketReq packet = new CreateAccountPacketReq()
        {
            AccountName = account,            
            Password = password,
            Nickname = nickname
        };

        WebManager.Instance.SendPostRequest<CreateAccountPacketRes>("account/create", packet, (res) =>
        {
           
            //Debug.Log(((CreateAccountError)res.ErrorCode).ToString());
            onResult?.Invoke(res.ErrorCode);

        });
        return;
    }

    public bool TryLogin(string id, string pw)
    {
        //서버에서 로그인 인증
        //서버에서 유저 정보 받아옴                    
        
        string account = id;
        string password = pw;
        if (string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        LoginAccountPacketReq packet = new LoginAccountPacketReq()
        {
            AccountName = account,
            Password = password
        };
        //웹서버 로그인 인증
        WebManager.Instance.SendPostRequest<LoginAccountPacketRes>("account/login", packet, (res) =>
        {
            Debug.Log(res.LoginOk);

            if (res.LoginOk)
            {
                //success
                usersession.UserLogin(id, 1); //서버에서 받아온 유저정보라고 가정  
                NetworkManager.Instance.ConnectToGame(res.SessionKey, res.ServerInfo.Ip, res.ServerInfo.Port);
            }
        });
        return true;
    }

    public bool LogoutFromServer()
    {
        //로그인서버에 로그아웃요청

        //로그아웃 성공
        usersession.UserLogout();

        return true;
    }


}
