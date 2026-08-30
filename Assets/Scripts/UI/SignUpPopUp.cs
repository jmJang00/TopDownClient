using MoreMountains.Feedbacks;
using MoreMountains.MMInterface;
using UnityEngine;
using UnityEngine.UI;

public class SignUpPopUp : MMPopup
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created      
    [SerializeField] private InputField IF_newid;
    [SerializeField] private InputField IF_nickname;
    [SerializeField] private InputField IF_newpassword;
    [SerializeField] private InputField IF_newpasswordcheck;   
    [SerializeField] private Text TXT_passwordcheckresult;
    [SerializeField] private Text TXT_signupresult; 

    private LoginManager loginmanager;
    private string newid;
    private string nickname;
    private string newpassword;
    private string newpasswordcheck;
    private bool passwordmatched = false;

    protected override void Start()
    {        
        Initialization();
        //gameObject.SetActive(true);
    }   
    protected override void Initialization()
    {                
        base.Initialization();
        newid = IF_newid.text;
        newpassword = IF_newpassword.text;
        newpasswordcheck = IF_newpasswordcheck.text;
        loginmanager = GameObject.Find("LoginManager").GetComponent<LoginManager>();
    }
    // Update is called once per frame
    protected override void  Update()
    {
        base.Update();
        newpassword = IF_newpassword.text;
        newpasswordcheck = IF_newpasswordcheck.text;

        if (!string.IsNullOrWhiteSpace(newpassword) && !string.IsNullOrWhiteSpace(newpasswordcheck))
        {
            if (newpassword != newpasswordcheck)
            {
                TXT_passwordcheckresult.text = "Passwords do not match.";
                passwordmatched = false;
            }
            else
            {
                TXT_passwordcheckresult.text = "Password match";
                passwordmatched = true;
            }
        }
        else
        {
            TXT_passwordcheckresult.text = "";
            passwordmatched = false;
        }
    }
    public override void Open()
    {
        this.gameObject.SetActive(true);
        Start();
        base.Open();      
    }
    public override void Close()
    {       
        IF_newid.text = "";
        IF_newpassword.text = "";
        IF_newpasswordcheck.text = "";
        TXT_passwordcheckresult.text = "";
        TXT_signupresult.text = "";
        //this.gameObject.SetActive(false);
        base.Close();
    }
    public void OnClickSignUp()
    {       
        newid = IF_newid.text;        
        nickname = IF_nickname.text;
        newpassword = IF_newpassword.text;

        loginmanager.TrySignUp(newid, newpassword, nickname, (errcode) =>
        {
            if (errcode == (int)CreateAccountError.Success)
            {
                Debug.Log("회원가입 성공");
                TXT_signupresult.text = "Sign Up success.";
            }
            else if (errcode == (int)CreateAccountError.DuplicateAccountName)
            {
                Debug.Log("이미 존재하는 아이디");
                TXT_signupresult.text = "Duplicate Account Name.";
            }
            else if (errcode == (int)CreateAccountError.DuplicateAccountNickName)
            {
                Debug.Log("이미 존재하는 닉네임");
                TXT_signupresult.text = "Duplicate Nickname.";
            }
            else
            {
                Debug.Log($"회원가입 실패: {errcode}");
                TXT_signupresult.text = "Please Input ID,Nickname,PassWord.";
            }
        });
    }
    public void OnClickClose()
    {        
        Close();
    }   
}
