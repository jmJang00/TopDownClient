using MoreMountains.TopDownEngine;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UserSession;

public class CommunityManager : MonoBehaviour
{
    public class FriendInfo
    {        
        public string id;
        public int level;
        public bool isOnline;
    }
    [SerializeField] private GroupManager groupManager;
    [SerializeField] UserSession usersession;
    [SerializeField] private CommunityPopup communityPopup;
    [SerializeField] private OptionPopup friendOptionpopup;
    [SerializeField] private OptionPopup requestOptionpopup;
     private Transform fListParent;
     private CommunityUseritem fItemPrefab;  

    private CommunityUseritem currentSelected;    
    
    private Dictionary<string,CommunityUseritem> friendUiList = new();
   
    public void OpenCommunityPopup()
    {
        communityPopup.Open();
    }
    public bool RequestFriend(string fid)
    {          
        Debug.Log("Requesting friend: " + fid);

        //SendPacketRequestFriend(fid);
        bool result = true;
        
        if(!result)
        {
            //fail
            //if 존재하지 않는 유저
            //이미 친구목록에 존재하는 유저
            return false;
        }
        else
        {           
            //success           
            UserSession.friendList.Add(new UserInfo() { id = fid, level = 0, isOnline = true }); //서버에서 받아온 친구정보라고 가정            
            RefreshFriendList();
            return true;
        }
    }    
    public void InitFriendList(Transform parent, CommunityUseritem itemPrefab)
    {
        fListParent = parent;
        fItemPrefab = itemPrefab;
        RefreshFriendList();
    }
    public void RefreshFriendList()
    {
        //기존삭제
        foreach (Transform child in fListParent)
        {
            Destroy(child.gameObject);
        }
        friendUiList.Clear();

        //Get Friend List from Server
        //List<string> friends = GetFriendList();//서버에서 친구목록 받아옴

        //새로 생성
        foreach (UserInfo friend in friendList)
        {
            CommunityUseritem item = Instantiate(fItemPrefab, fListParent);
            item.Init(friend.level, friend.id, this);
            friendUiList.Add(friend.id, item);
        }
    }
    
    /// <summary>
    /// 서버에 친구목록 요청해서 받아옴
    /// </summary>
    /// <returns></returns>
    public void GetFriendList()
    {  
        //서버에서 친구 목록 받아와서 list에 추가
        UserSession.friendList.Add(new UserInfo() { id = "friend1", level = 10, isOnline = true });
    }        
    public void SelectFriend(CommunityUseritem item)
    {
        if (currentSelected != null)
        {
            currentSelected.SetSelected(false);
            //friendOptionpopup.Close();
        }
        currentSelected = item;
        currentSelected.SetSelected(true);
        //Debug.Log("Selected Friend: " + currentSelected._userId);
        friendOptionpopup.Open();

    }
    /// <summary>
    /// 유저를 친구목록에서 삭제
    /// </summary>
    /// <param name="id"></param>
    public void RemoveFriend()
    {
        Debug.Log("RemoveFriend : " + (currentSelected != null ? currentSelected._userId : "None"));

        UserInfo friend = UserSession.friendList.Find(friend => friend.id == currentSelected._userId); //서버에서 친구삭제 처리했다고 가정
        if (friend != null)
        {
            UserSession.friendList.Remove(friend);
        }
        //friendUiList.Remove(currentSelected._userId);
        RefreshFriendList();
    }
    /// <summary>
    /// 유저를 그룹에 초대
    /// </summary>
    /// <param name="id"></param>
    public void InviteUserToGroup()
    {
        Debug.Log("InviteUserToGroup : " + (currentSelected != null ? currentSelected._userId : "None"));
        groupManager.AddMember(currentSelected._userId,false); //서버에서 그룹초대 처리했다고 가정
    }
    public void ResetCommunityState()
    {
        friendList.Clear();
        RefreshFriendList();
    }

    public void AcceptFriend(string friendid)
    {
        //친구 요청 수락

    }
    public void DeclineFriend(string friendid)
    {
        //친구 요청 거절

    }
    
    
}
