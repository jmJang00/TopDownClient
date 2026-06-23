using MoreMountains.MMInterface;
using UnityEngine;

public class OptionPopup : MMPopup
{

    [SerializeField] private GroupManager groupManager;
    [SerializeField] private CommunityManager communityManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {        
        Initialization();
    }
    protected override void Initialization()
    {
        base.Initialization();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    public override void Open()
    {
        this.gameObject.SetActive(true);
        Start();
        base.Open();
    }    
    public override void Close()
    {        
        base.Close();
    }   
   

}
