using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int primaryAmmo = 60;
    public int secondaryAmmo = 21;
    public int grenades = 2;

    public float cooldown = 3f;
    bool canUse = true;

    public AudioSource audioSource;
    public AudioClip pickupSound;

    void OnTriggerEnter(Collider other)
    {
        if (!canUse) return;
        if (!other.CompareTag("Player")) return;

        WeaponHolder holder = other.GetComponentInChildren<WeaponHolder>();
        GrenadeThrow gt = other.GetComponent<GrenadeThrow>();

        if (holder)
        {
            Weapon primary = holder.GetPrimaryWeapon();
            Weapon secondary = holder.GetSecondaryWeapon();

            if (primary)
                primary.AddReserveAmmoClamped(primaryAmmo);

            if (secondary)
                secondary.AddReserveAmmoClamped(secondaryAmmo);
        }

        if (gt)
            gt.grenadeCount += grenades;

        if (audioSource && pickupSound)
            audioSource.PlayOneShot(pickupSound);

        StartCoroutine(Cooldown());
    }

    System.Collections.IEnumerator Cooldown()
    {
        canUse = false;
        yield return new WaitForSeconds(cooldown);
        canUse = true;
    }
}
