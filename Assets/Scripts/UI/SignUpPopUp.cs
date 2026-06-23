using MoreMountains.Feedbacks;
using MoreMountains.MMInterface;
using UnityEngine;
using UnityEngine.UI;

public class SignUpPopUp : MMPopup
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created      
    [SerializeField] private InputField IF_newid;
    [SerializeField] private InputField IF_newpassword;
    [SerializeField] private InputField IF_newpasswordcheck;
    [SerializeField] private Text TXT_idcheckresult;
    [SerializeField] private Text TXT_passwordcheckresult;
    [SerializeField] private Text TXT_signupresult;


    private string newid;
    private string newpassword;
    private string newpasswordcheck;
    private bool idChecked = false;
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
        TXT_idcheckresult.text = "";
        TXT_passwordcheckresult.text = "";
        TXT_signupresult.text = "";
        //this.gameObject.SetActive(false);
        base.Close();
    }
   

    public void OnClickCheckNewId()
    {
        //Debug.Log("OnClickCheckNewId");
        newid = IF_newid.text;      
        
        bool result = true; //SendPacketCheckID(newid);
        if (!result)
        {
            //ID 중복
            TXT_idcheckresult.text = "ID is already taken.";
        }
        else
        {
            //사용가능
            TXT_idcheckresult.text = "ID is available.";
            idChecked = true;
        }

    }
    public void OnClickSignUp()
    {         
        //Debug.Log("OnClickSignUp");
        newpassword = IF_newpassword.text;
        newpasswordcheck = IF_newpasswordcheck.text;

        if (!idChecked)
        {
            TXT_idcheckresult.text = "Please check the ID first.";
            TXT_signupresult.text = "Sign Up is Failed.";
            return;
        }
        if(!passwordmatched)
        {
            TXT_passwordcheckresult.text = "Passwords do not match.";
            TXT_signupresult.text = "Sign Up is Failed.";
            return;
        }
        //SendPacketSignUp(newid, newpassword);
        TXT_signupresult.text = "Sign Up is successful.";
    }
    public void OnClickClose()
    {        
        Close();
    }
}
