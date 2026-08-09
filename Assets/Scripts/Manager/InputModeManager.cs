using System.Collections;
using UnityEngine;


public enum InputMode
{    
    Game,
    Spectate,
    Chat,
    Inventory,
    UI,
}

public interface IInputModeChangeable
{
    abstract InputModeChangeableInfo GetInputModeInfo();
}

public struct InputModeChangeableInfo
{
    public string Name;
    public string Description;

    public override string ToString()
    {
        return $"Name:[{Name}] Description:[{Description}]";
    }
}



public class InputModeManager : MonoBehaviour
{

    public static InputModeManager Instance { get { Init(); return _instance; } }
    private static InputModeManager _instance;

    public InputMode CurrentMode { get; private set; }
    private InputMode _defaultMode = InputMode.Game;

    private IInputModeChangeable _owner = null;
    private string _ownerDescription = string.Empty;

    private void Awake()
    {
        Init();
    }
    private static void Init()
    {
        if (_instance == null)
        {
            GameObject go = GameObject.Find("InputModeManager");
            DontDestroyOnLoad(go);
            _instance = go.GetComponent<InputModeManager>();
        }
    }

    public void SetDefault(InputMode mode)
    {
        if(mode != InputMode.Game && mode != InputMode.Spectate)
        {
            Debug.LogError("Default 모드는 Game 혹은 Spectate만 가능합니다.");
            throw new System.Exception("디폴트모드 변환실패 매개변수 타입 확인.");
            //return;
        }

        _defaultMode = mode;
    }

    public bool Enter(InputMode mode, IInputModeChangeable owner)
    {
        if(_owner != null)
        {
            Debug.LogError($"모드진입실패 : 선점대상정보 -> {owner.GetInputModeInfo().ToString()}");
            throw new System.Exception("모드진입실패 _owner 디버그 + Release 코드확인요망");
            //return false;
        }

        if(owner == null)
        {
            Debug.LogError($"모드진입실패 : IModeChangeable 매게변수가 null 입니다");
            throw new System.Exception("모드진입실패 매개변수 확인요망");
            //return false;
        }
       
        _owner = owner;
        CurrentMode = mode;
        Debug.Log($"모드진입성공 : {owner.GetInputModeInfo().ToString()}");
        return true;
    }

    public bool Release(IInputModeChangeable owner)
    {
        if(_owner == null)
        {
            Debug.LogError($"모드반환실패 : 반환할 진입모드 없음");
            throw new System.Exception("모드반환실패 Enter코드 정상적으로 삽입했는지 확인 요망");
            //return false;
        }

        if(owner == null)
        {
            Debug.LogError($"모드반환실패 : IModeChangeable 매게변수가 null 입니다");
            throw new System.Exception("모드반환실패 매개변수 확인요망");
            //return false;
        }

        if (_owner != owner)
        {
            Debug.LogError($"모드반환실패 : 선점대상과 해제대상이 다릅니다 \n" +
                $"선점대상정보 -> {_owner.GetInputModeInfo().ToString()}\n" +
                $"반환요청대상정보 -> {owner.GetInputModeInfo().ToString()}");
            return false;
        }
        
        GotoDefault();
        _owner = null;
        Debug.Log($"모드반환성공 : 반환요정대상정보 -> {owner.GetInputModeInfo().ToString()}");
        return true;
    }     

    private void GotoDefault()
    {        
        CurrentMode = _defaultMode;
    }
}
