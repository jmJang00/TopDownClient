using MoreMountains.MMInterface;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommunityPopup : MMPopup
{
    
    [SerializeField] private InputField IF_FriendID;
    [SerializeField] private Text TXT_FriendRequestResult;

    [SerializeField] private Transform FriendListParent;
    [SerializeField] private CommunityUseritem FriendItemPrefab;    
    [SerializeField] private OptionPopup FriendOption;
    [SerializeField] private OptionPopup RequestOption;
    [SerializeField] private CommunityManager communityManager;
    [SerializeField] private GroupManager groupmanager;

    [SerializeField] private GameObject FriendPage;
    [SerializeField] private GameObject RequestPage;    
  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {       
        Initialization();
    }
    protected override void Initialization()
    {        
        base.Initialization();
        Init();
        TXT_FriendRequestResult.text = "";
        FriendPage.SetActive(true);
        RequestPage.SetActive(false);      
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    public override void Open()
    {
        Debug.Log("CommunityPopup Open");        
        // Load friend list from community manager       
        this.gameObject.SetActive(true);
        Start();       
        communityManager.InitFriendList(FriendListParent, FriendItemPrefab);
        base.Open();
    }
    public override void Close()
    {               
        Debug.Log("CommunityPopup Close");
        
        FriendOption.Close();
        RequestOption.Close();
        Init();
        base.Close();
    }    
    public void OnClickFriendRequest()
    {
        string fid = IF_FriendID.text;
        
        if(string.IsNullOrWhiteSpace(fid))
        {
            TXT_FriendRequestResult.text = "Please enter a friend ID.";
            return;
        }
        else
        {
            bool result = communityManager.RequestFriend(fid);

            if(result)
            {
                IF_FriendID.text = "";
                TXT_FriendRequestResult.text = "Friend request success.";
            }
            else
            {
                TXT_FriendRequestResult.text = "friend request failed.";
            }
        }    
    }
    public void OnClickFriendPage()
    {
        FriendPageActive();
    }
   private void FriendPageActive()
   {
        Init();
        FriendPage.SetActive(true);
        RequestPage.SetActive(false);        
    }    
    public void OnClickRequestPage()
    {
        RequestPageActive();
    }
    public void OnClickRemoveFriend()
    {
        Debug.Log("OnClickRemoveFriend");
        communityManager.RemoveFriend();        
    }
    public void OnClickInvite()
    {
        communityManager.InviteUserToGroup();
        FriendOption.Close();
    }
    private void RequestPageActive()
    {
        Init();
        RequestPage.SetActive(true);
        FriendPage.SetActive(false);      
    }
    private void Init()
    {
        TXT_FriendRequestResult.text = "";
        IF_FriendID.text = "";
        FriendOption.Close();
        RequestOption.Close();
    }
    public void OpenFreindOption()
    {        
        FriendOption.Open();
    }
    public void OpenRequestOption()
    {
        RequestOption.Open();
    }    
}
