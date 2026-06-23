using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UserSession : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public class UserInfo
    {
        public string id;
        public int level;
        public bool isOnline;
        public int exp;
    }

    [SerializeField] private UIManager uiManager;
    [SerializeField] private CommunityManager communityManager;
    [SerializeField] private LoginManager loginManager;
    [SerializeField] private GroupManager groupManager;

    public static UserInfo user = new UserInfo();
    public static List<UserInfo> friendList = new List<UserInfo>();
    public static List<UserInfo> groupMemberList = new List<UserInfo>();
    

    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UserLogin(string id,int level)
    {
        user.id = id;
        user.level = level;
        user.isOnline = true;
        user.exp = 0;
        
        //서버에서 친구목록 받아옴
    }

    public void UserLogout()
    {
        user.id = null;
        user.level = 0;
        user.isOnline = false;
        user.exp = 0;        
        
    }
}
