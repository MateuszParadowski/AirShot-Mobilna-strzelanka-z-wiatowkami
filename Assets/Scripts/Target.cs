using UnityEngine;

public class Target : MonoBehaviour
{
    [Header("Airsoft Settings")]
    public bool airsoftMode = true;

    [Header("HP (used only if airsoftMode = false)")]
    public float health = 100f;

    public System.Action OnHit;   // sygnał do PopUpTarget

    public void TakeDamage(float amount)
    {
        if (airsoftMode)
        {
            Hit();
            return;
        }

        health -= amount;

        if (health <= 0f)
            Hit();
    }

    void Hit()
    {
        OnHit?.Invoke();   // informuje PopUpTarget
        gameObject.SetActive(false);
    }
}
