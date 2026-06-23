using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommunityUseritem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("Main Button")]
    [SerializeField] private MMTouchButton mainButton;
    [SerializeField] private Text nameText;    
    [SerializeField] private UnityEngine.UI.Image background;
    [SerializeField] private UnityEngine.UI.Image backgroundselect;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.gray;

  
    public int _level;
    public string _userId;
    public bool isOnline = false;
    private CommunityPopup communitypopup;
    private CommunityManager communitymanager;
    /// <summary>
    /// 컴포넌트 연결
    /// </summary>
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
    public void Init(int level,string id, CommunityManager ui)
    {
        _level = level;
        _userId = id;
        nameText.text = id;
        communitymanager = ui;        

        //기존 이벤트 제거
        mainButton.ButtonPressedFirstTime.RemoveAllListeners();      

        //연결
        mainButton.ButtonPressedFirstTime.AddListener(OnClickFriendID);
       
    }
    public void Update()
    {
        if(!isOnline)
        {
            nameText.color = Color.black;
        }
        else
        {
            nameText.color = new Color(0f, 0.639f, 1f); // Color00A3FF
        }

    }
    public void OnClickFriendID()
    {
        //선택된 오브젝트의 렌더링을 변경해서 선택됬음을 알려야 함        
        communitymanager.SelectFriend(this);      
        //Debug.Log("Selected Friend: " + userId);
        

        //Debug.Log($"clicked: {_userId}");
        //Debug.Log($"background object: {background.gameObject.name}");
       // Debug.Log($"selectedColor: {selectedColor}");
       // Debug.Log($"actual color: {background.color}");

    }
    /// <summary>
    /// 선택된 오브젝트가 바뀌면 이전 선택된 오브젝트 컬러 변경
    /// </summary>
    /// <param name="selected"></param>
    public void SetSelected(bool selected)
    {       
       //background.color = selected ? selectedColor : normalColor;       
        if(selected)
        {
            background.color = selectedColor;
           // background.gameObject.SetActive(false);
           // backgroundselect.gameObject.SetActive(true);
        }
        else
        {
            background.color = normalColor;
            //background.gameObject.SetActive(true);
            // backgroundselect.gameObject.SetActive(false);
        }
    }
   
}
