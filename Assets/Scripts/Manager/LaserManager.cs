using System.Collections;
using UnityEngine;

public class LaserManager : MonoBehaviour
{
    static LaserManager _instance;
    public static LaserManager Instance { get { return _instance; } }

    [Header("Laser")]
    public LineRenderer LaserPrefab;
    public float LaserDuration = 1.0f;

    private void Start()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    public virtual void DrawLaser(Vector3 start, Vector3 end)
    {

        if (LaserPrefab == null)
            return;

        LineRenderer lr = Instantiate(LaserPrefab);

        lr.positionCount = 2;

        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        StartCoroutine(FadeLaser(lr));
    }

    protected virtual IEnumerator FadeLaser(
        LineRenderer lr)
    {
        float timer = 0f;

        Color startColor = lr.startColor;
        Color endColor = lr.endColor;

        while (timer < LaserDuration)
        {
            timer += Time.deltaTime;

            float alpha =
                Mathf.Lerp(1f, 0f, timer / LaserDuration);

            startColor.a = alpha;
            endColor.a = alpha;

            lr.startColor = startColor;
            lr.endColor = endColor;

            yield return null;
        }

        Destroy(lr.gameObject);
    }
}
