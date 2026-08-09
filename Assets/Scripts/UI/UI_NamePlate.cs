using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_NamePlate : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _nickname;

    [SerializeField]
    private MMProgressBar _hpBar;

    [SerializeField]
    private CanvasGroup _canvasGroup;


    private Transform _target;
    private float _hideTime;

    private const float SHOW_TIME = 2.0f;
    private const float FADE_TIME = 0.2f;


    public void Initialize(Transform target, string nickname)
    {
        _target = target;

        _nickname.text = nickname;

        _canvasGroup.alpha = 0.0f;
        gameObject.SetActive(true);
    }

    void LateUpdate()
    {
        if (_target == null) return;
        transform.position = _target.position;
        transform.rotation = Camera.main.transform.rotation;
    }

    public void SetHP(float ratio)
    {
        _hpBar.UpdateBar01(ratio);
        ShowHP();
    }


    private void ShowHP()
    {
        _canvasGroup.alpha = 1.0f;
        _hideTime = Time.unscaledTime + SHOW_TIME;
    }


    public void UpdateFade()
    {
        if (_hideTime <= 0)
            return;

        float remain = _hideTime - Time.unscaledTime;

        if (remain <= FADE_TIME)
        {
            _canvasGroup.alpha = Mathf.Clamp01(remain / FADE_TIME);
        }

        if (remain <= 0)
        {
            _canvasGroup.alpha = 0;
            _hideTime = 0;
        }
    }


    public Vector3 GetWorldPosition()
    {
        return _target.position;
    }


    public void Release()
    {
        _target = null;
        _hideTime = 0;

        gameObject.SetActive(false);
    }
}
