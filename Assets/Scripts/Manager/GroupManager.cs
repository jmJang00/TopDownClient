using MoreMountains.TopDownEngine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;
using static UserSession;

public class GroupManager : MonoBehaviour
{
    public class GroupInfo
    {
        public string leaderid;
        public int memberCount;
        public List<GroupMemberItem> members = new List<GroupMemberItem>();

        //멤버 id가 newLeader와 같으면 isLeader true, 아니면 false
        public void ChangeLeader(string newLeader)
        {
            leaderid = newLeader;
            members.ForEach(x => x.isLeader = (x.id == newLeader));
        }
    }

    [SerializeField] private UserSession usersession;
    [SerializeField] private Text groupMsg;
    [SerializeField] private GameObject groupPage;
    [SerializeField] private OptionPopup groupOptionPopup;
    [SerializeField] private Transform memberList;
    [SerializeField] private GroupMemberItem memberitemPrefab;

    private const int MAX_GROUP_MEMBER = 3;
    private GroupMemberItem currentSelected;
    GroupInfo group = new GroupInfo();

    public void CreateGroup(string id)
    {
        groupPage.SetActive(true);
        group.leaderid = id;
        AddMember(id,true);

        RefreshMemberList();
       
    }

    public void AddMember(string id,bool isleader)
    {        
        if(group.members.Count >= MAX_GROUP_MEMBER)
        {
            UpdateMsg("Group is full");
            return;
        }
        group.members.Add(new GroupMemberItem { id = id , isLeader = isleader });
        ++group.memberCount;         
        RefreshMemberList(); 
    }
    public void RemoveMember()
    {   
        if(group.leaderid != UserSession.user.id)
        {                    
            UpdateMsg("Only leader can remove members");           
            return;
        }
        if (currentSelected == null)
        {                   
            UpdateMsg("No member selected");
            return;
        }
        if(currentSelected.id == UserSession.user.id )
        {               
            UpdateMsg("Can't remove yourself");
            return;
        }
        if (group.memberCount <= 1)
        {           
            UpdateMsg("Group is empty");
            return;
        }

        GroupMemberItem member = group.members.Find(x => x.id == currentSelected.id);
        if (member.id != null)
        {
            group.members.Remove(member);
            --group.memberCount;
        }
        else
        {
            UpdateMsg("Member not found");
        }
        RefreshMemberList();
    }
    //그룹멤버 UI 새로고침
    public void RefreshMemberList()
    {
        //기존삭제
        foreach (Transform child in memberList)
        {
            Destroy(child.gameObject);
        }

        //새로생성
        foreach (GroupMemberItem member in group.members)
        {
            GroupMemberItem item = Instantiate(memberitemPrefab, memberList);
            if (!(group.leaderid == member.id))
            {
                item.Init(member.id, this);
                item.SetGroupLeader(false);
            }
            else
            {
                item.Init(member.id, this);
                item.SetGroupLeader(true);
            }                   
        }
    }
    public void ResetGroupState()
    {
        group.members.Clear();
        group.memberCount = 0;
        UpdateMsg("");

        RefreshMemberList();
    }
    public void SelectMember(GroupMemberItem item)
    {
        if (currentSelected != null)
        {
            currentSelected.SetSelected(false);
        }

        currentSelected = item;
        currentSelected.SetSelected(true);
        groupOptionPopup.Open();
    }
    public void ChangeGroupLeader()
    {
        if(group.leaderid != UserSession.user.id)
        {           
            UpdateMsg("You're not the leader");
            return;
        }
        if (currentSelected == null)
        {           
            UpdateMsg("No member selected");
            return;
        }        
        group.members.ForEach(x => x.isLeader = false);

        //남은 멤버 중에서 새로운 그룹장 선택
        //선택된 멤버 id를  group.leaderid로 변경
        if (currentSelected.id == null)
        {
            GroupMemberItem target =  group.members.Find(x => x.id != UserSession.user.id);           
            group.ChangeLeader(target.id);
        }
        else
        {
            //그룹장 이름 변경           
            group.ChangeLeader(currentSelected.id);
        }       
        RefreshMemberList();
    }
    //그룹 떠나기
    //본인이 그룹장이라면 서버에서 그룹장을 새로 임명하거나 그룹을 해체
    public void LeaveGroup()
    {
        if (group.leaderid == UserSession.user.id)
        {
            //내가 그룹장이면 서버에서 그룹장을 바꾸거나 그룹을 해체
            //ChangeGroupLeader();
        }
        GroupMemberItem member = group.members.Find(x => x.id == UserSession.user.id);
        if (member != null)
        {
            group.members.Remove(member);
            --group.memberCount;
        }
        else
        {
            UpdateMsg("Member not found");
        }
        RefreshMemberList();
    }

    //그룹장이 위임하지 않고 그룹을 나간 경우.
    public void LeaderLeaveGroup()
    {
        //서버에서 다음 그룹장 id를 받음

        group.leaderid = string.Empty; //서버에서 받아온 그룹장 id라고 가정 

    }
    private void UpdateMsg(string msg)
    {
        groupMsg.text = msg;
    }
}
