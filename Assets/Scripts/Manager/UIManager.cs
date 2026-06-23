using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Feedbacks;

public class UIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] UserSession usersession;

    public GameObject loginPanel;
    public GameObject lobbyPanel;
    //public GameObject communityPage;
    

    public TMP_InputField idInput;
    public TMP_InputField pwInput;
    [SerializeField] private SignUpPopUp signuppopup;
    [SerializeField] private CommunityPopup communitypopup;
    [SerializeField] private CommunityManager communityManager;
    [SerializeField] private LoginManager loginManager;
    [SerializeField] private GroupManager groupManager;

    protected bool isLogin = false;
 
    public void OnClickSignUp()
    {
        Debug.Log("OnClickSignUp");
        if (signuppopup != null)
        {           
            signuppopup.Open();
        }
        else
            Debug.LogError("SignUpPopUp reference is missing in UIManager.");

        //status.text = "Sign Up is not implemented yet.";
    }

    public void OnClickLogin()
    {
        //로그인 매니저 호출
        Debug.Log("OnClickLogin");
        bool loginResult = loginManager.TryLogin(idInput.text, pwInput.text);

        if(loginResult)
        {            
            isLogin = true;
            loginPanel.SetActive(false);
            lobbyPanel.SetActive(true);
            groupManager.CreateGroup(UserSession.user.id);
            communityManager.GetFriendList();
        }
        else
        {
           // status.text = "login fail";
           // Debug.LogError("Login Failed in Login Manager");
        }      
    }
    protected bool LogoutRQ()
    {
        //Debug.Log("LogoutReQuest to Login Manger");
        return loginManager.LogoutFromServer();
    }

    /// <summary>
    /// UI를 초기 상태로 되돌리는 함수 (로그아웃, 게임종료 시 호출)
    /// </summary>
    public void UIStateReset()
    {
        isLogin = false;

        loginPanel.SetActive(true);
        lobbyPanel.SetActive(false);       
        
        // 입력 초기화 (중요)
        idInput.text = "";
        pwInput.text = "";
        //status.text = "Logout";

        groupManager.ResetGroupState();
        communityManager.ResetCommunityState();
    }
    public void OnClickLogout()
    {
        Debug.Log("OnClickLogout");
        if (LogoutRQ())
        {           
            UIStateReset();
            groupManager.ResetGroupState();
            communityManager.ResetCommunityState();
        }
        else
        {
           // status.text = "Logout Failed";
        }        
    }

    public void OnClickExit()
    {
        Debug.Log("OnClickExit");

        if (isLogin)
        {
            LogoutRQ();
        }

        UIStateReset();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif        
    }

    public void OnClickPlay()
    {
        Debug.Log("OnClickPlay");
        //매치메이킹 매니저 호출
        //status.text = "Start Game is not implemented yet.";
    }
    public void OnClickCommunity()
    {        
        communityManager.OpenCommunityPopup();      
    }   
    public void OnClickCloseCommunity()
    {
        //Debug.Log("OnClickCloseCommunity");
        //communityPage.SetActive(false);
        //communityManager.CloseCommunityPage();
    }    
     public void OnClickCreateGroup()
    {
        Debug.Log("OnClickCreateGroup");
       // groupManager.CreateGroup(Myinfo.id);
    }
    public void OnClickRemoveMember()
    {
        Debug.Log("OnClickRemoveMember");
        groupManager.RemoveMember();
    }
    void Start()
    {
        loginPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        signuppopup.gameObject.SetActive(true);
        communitypopup.gameObject.SetActive(true);
        
    }

    // Update is called once per frame
  
}
