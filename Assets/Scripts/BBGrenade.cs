using UnityEngine;

public class BBGrenade : MonoBehaviour
{
    public float fuseTime = 2f;
    public float radius = 5f;
    public float damage = 15f;

    [Header("Effects")]
    public ParticleSystem burstEffect;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip throwSound;
    public AudioClip burstSound;

    void Start()
    {
        if (throwSound && audioSource)
            audioSource.PlayOneShot(throwSound);

        Invoke(nameof(Detonate), fuseTime);
    }

    void Detonate()
    {
        if (burstEffect)
            burstEffect.Play();

        if (burstSound && audioSource)
            audioSource.PlayOneShot(burstSound);

        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider col in hits)
        {
            if (col.TryGetComponent(out Target target))
                target.TakeDamage(damage);
        }

        Destroy(gameObject, 0.2f);
    }
}
