using System;
using UnityEngine;

public abstract class UI_Panel : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup targetCanvas;

    private bool _isVisible = false;

    public bool IsVisible
    {
        get { return _isVisible; }
    }
    
    public abstract bool RequestShow();
    public abstract bool RequestHide();    

    protected virtual void ShowInternal()
    {
        targetCanvas.alpha = 1f;
        targetCanvas.blocksRaycasts = true;
        targetCanvas.interactable = true;
        _isVisible = true;
        //gameObject.SetActive(true);
    }

    protected virtual void HideInternal()
    {
        targetCanvas.blocksRaycasts = false;
        targetCanvas.interactable = false;
        targetCanvas.alpha = 0f;
        _isVisible = false;
        //gameObject.SetActive(false);
    }

    protected virtual void SetVisible(bool visible)
    {
        if (visible)
            ShowInternal();
        else
            HideInternal();

        //gameObject.SetActive(visible);
    }

    protected virtual void Toggle()
    {        
        SetVisible(!_isVisible);
        //gameObject.SetActive(!gameObject.activeSelf);
    }
}
