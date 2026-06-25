using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;
using System;
using System.Collections;

public class HitscanWeaponRevised : HitscanWeapon
{
    [Header("Laser")]
    public LineRenderer LaserPrefab;
    public float LaserDuration = 1.0f;

    public override void SpawnProjectile(Vector3 spawnPosition, bool triggerObjectActivation = true)
    {
        //LineRendering
        base.SpawnProjectile(spawnPosition, triggerObjectActivation);
        DrawLaser();
        return;
    }

    protected virtual void DrawLaser()
    {
        if (LaserPrefab == null)
            return;

        Vector3 start = _origin;
        Vector3 end;

        if (_hitObject != null)
        {
            end = _hitPoint;
        }
        else
        {
            end = _origin
                + (_randomSpreadDirection.normalized
                * HitscanMaxDistance);
        }

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
