using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GroupMemberItem : MonoBehaviour
{
    public Text memberIDText;
    public GameObject leaderIcon; // 그룹 리더 아이콘

    public string id;    
    public bool isLeader = false;
    public int level;

    [SerializeField] private GroupManager groupManager;
    [SerializeField] private MMTouchButton mainButton;
    [SerializeField] private UnityEngine.UI.Image background;
    [SerializeField] private UnityEngine.UI.Image backgroundselect;
     private Color normalColor = Color.white;
    private Color selectedColor = Color.gray;
    private void Awake()
    {
        if (mainButton == null)
        {
            mainButton = transform.Find("Container/Background").GetComponent<MMTouchButton>();
        }
        if (background == null)
        {
            Debug.Log("Background connect success");
            background = transform.Find("Container/Background").GetComponent<UnityEngine.UI.Image>();
        }
        if (backgroundselect == null)
        {
            Debug.Log("Background connect success");
            backgroundselect = transform.Find("Container/Backgroundsel").GetComponent<UnityEngine.UI.Image>();
        }
    }
    public void Init(string id, GroupManager manager)
    {
        this.id = id;
        groupManager = manager;
        memberIDText.text = id;       
        //기존 이벤트 제거
        mainButton.ButtonPressedFirstTime.RemoveAllListeners();

        //연결
        mainButton.ButtonPressedFirstTime.AddListener(OnClickMemberID);
    }
    public void OnClickMemberID()
    {
        groupManager.SelectMember(this);
       
    }
    public void SetGroupLeader(bool isLeader)
    {
        this.isLeader = isLeader;
        //그룹 리더인 경우 텍스트 색상을 변경하거나 아이콘을 표시하는 등의 UI 업데이트를 수행할 수 있습니다.
        if (isLeader)
        {
           leaderIcon.SetActive(true); // 그룹 리더 아이콘 활성화
        }
        else
        {
            leaderIcon.SetActive(false); // 그룹 리더 아이콘 비활성화
        }
    }
    public void SetSelected(bool selected)
    {
        //background.color = selected ? selectedColor : normalColor;       
        if (selected)
        {
            background.color = selectedColor;            
            //background.gameObject.SetActive(false);
            //backgroundselect.gameObject.SetActive(true);
        }
        else
        {
            background.color = normalColor;
            //background.gameObject.SetActive(true);
            //backgroundselect.gameObject.SetActive(false);
        }
    }
}