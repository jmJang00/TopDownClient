using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;


public class ChatVisibilityController : MonoBehaviour
{

    [SerializeField]
    private CanvasGroup scrollView;

    [SerializeField]
    private float fadeDuration = 1f;


    private float hideTimer;

    private bool isHideWaiting;

    private Coroutine fadeCoroutine;



    private void Awake()
    {
        scrollView.alpha = 0f;
        scrollView.blocksRaycasts = false;
    }


    private void Update()
    {
        if (!isHideWaiting)
            return;


        hideTimer -= Time.deltaTime;


        if (hideTimer <= 0f)
        {
            isHideWaiting = false;
            StartFadeOut();
        }
    }



    public void Show()
    {
        StopFade();

        scrollView.alpha = 1f;
        scrollView.blocksRaycasts = true;
        isHideWaiting = false;
    }



    public void HideAfter(float delay)
    {
        StopFade();

        isHideWaiting = true;
        hideTimer = delay;
    }



    public void ShowAndHideAfter(float delay)
    {
        Show();

        isHideWaiting = true;
        hideTimer = delay;
    }



    private void StartFadeOut()
    {
        if (fadeCoroutine != null)
            return;


        fadeCoroutine = StartCoroutine(FadeOut());
    }



    private IEnumerator FadeOut()
    {
        float startAlpha = scrollView.alpha;

        float time = 0f;


        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            scrollView.alpha =
                Mathf.Lerp(startAlpha, 0f, time / fadeDuration);


            yield return null;
        }


        scrollView.alpha = 0f;
        scrollView.blocksRaycasts = false;

        fadeCoroutine = null;
    }



    private void StopFade()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
    }
}