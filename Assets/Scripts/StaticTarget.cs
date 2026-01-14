using UnityEngine;
using System.Collections;

public class StaticTarget : MonoBehaviour
{
    public Target target;
    public float respawnTime = 3f;

    void OnEnable()
    {
        target.OnHit += HandleHit;
    }

    void OnDisable()
    {
        target.OnHit -= HandleHit;
    }

    void HandleHit()
    {
        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        // Wyłączamy tylko HitBox (nie cały obiekt)
        target.gameObject.SetActive(false);

        yield return new WaitForSeconds(respawnTime);

        target.gameObject.SetActive(true);
    }
}
