using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    public enum PowerType { Spring, Gas, Electric, HPA }
    public enum FireMode { Single, Burst, Auto }

    [Header("Weapon Type")]
    public PowerType powerType = PowerType.Electric;
    public FireMode fireMode = FireMode.Single;

    [Header("References")]
    [HideInInspector] public Camera cam;
    public AudioSource audioSource;

    [Header("Muzzle")]
    public Transform muzzlePoint;
    public ParticleSystem muzzleFlash;

    [Header("Effects")]
    public GameObject hitEffect;
    public GameObject bbTracerPrefab;

    [Header("Audio")]
    public AudioClip springShot;
    public AudioClip gasShot;
    public AudioClip electricShot;
    public AudioClip hpaShot;
    public AudioClip reloadSound;
    public AudioClip cockSound;
    public AudioClip fireModeSwitchSound;

    [Header("Stats")]
    public float damage = 25f;
    public float range = 100f;
    public float fireRate = 0.15f;

    [Header("Burst")]
    public int burstCount = 3;

    [Header("Ammo")]
    public int magazineSize = 30;
    public int currentAmmo;
    public int reserveAmmo = 120;
    public float reloadTime = 1.5f;

    [Header("ADS (per-weapon)")]
    public Vector3 hipPosition;
    public Vector3 adsPosition;
    public float adsSpeed = 8f;

    [Header("Ammo Warnings")]
    public int lowAmmoThreshold = 5;

    [Header("Spring Audio")]
    public AudioSource cockSource; // OSOBNY AudioSource

    [Header("Ammo Limits")]
    public int maxReserveAmmo = 120;

    float nextTimeToFire;
    bool isReloading;
    bool isBursting;
    bool isCocking;

    // ===== KLUCZOWE FLAGI =====
    bool needsCockAfterReload = false;
    bool reloadFromEmpty = false;
    bool initialized;

   void OnEnable()
    {
        if (!initialized)
        {
            currentAmmo = magazineSize;
            initialized = true;
        }

        isCocking = false;
        reloadFromEmpty = false;
    }

    // ================= FIRE =================
    public void HandleFire(bool fireInput)
    {
        if (!fireInput || isReloading)
            return;

        // ===== SPRING =====
        if (powerType == PowerType.Spring)
        {
            if (isCocking)
                return;

            if (currentAmmo <= 0)
            {
                if (reserveAmmo > 0)
                {
                    reloadFromEmpty = true;
                    Reload();
                }
                return;
            }

            Shoot();
            StartCoroutine(SpringCockRoutine());
            return;
        }


        // ===== GAS / ELECTRIC / HPA =====
        if (currentAmmo <= 0)
        {
            if (reserveAmmo > 0)
            {
                reloadFromEmpty = true;
                needsCockAfterReload = true;
                Reload();
            }
            return;
        }

        if (Time.time < nextTimeToFire)
            return;

        nextTimeToFire = Time.time + fireRate;

        if (fireMode == FireMode.Burst)
        {
            if (!isBursting)
                StartCoroutine(BurstFire());
        }
        else
        {
            Shoot();
        }
    }

    // ================= SHOOT =================
    void Shoot()
    {
        currentAmmo--;

        if (muzzleFlash)
            muzzleFlash.Play();

        PlayShotSound();

        Vector3 hitPoint;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, range))
        {
            hitPoint = hit.point;

            if (hit.collider.TryGetComponent(out Target t))
                t.TakeDamage(damage);

            if (hitEffect)
            {
                GameObject impact = Instantiate(
                    hitEffect,
                    hit.point,
                    Quaternion.LookRotation(hit.normal)
                );
                Destroy(impact, 2f);
            }
        }
        else
        {
            hitPoint = cam.transform.position + cam.transform.forward * range;
        }

        if (bbTracerPrefab && muzzlePoint)
        {
            GameObject bb = Instantiate(
                bbTracerPrefab,
                muzzlePoint.position,
                muzzlePoint.rotation
            );

            BBTracer tracer = bb.GetComponent<BBTracer>();
            if (tracer)
                tracer.Init(hitPoint);
        }
    }

    IEnumerator BurstFire()
    {
        isBursting = true;

        for (int i = 0; i < burstCount; i++)
        {
            if (currentAmmo <= 0)
                break;

            Shoot();
            yield return new WaitForSeconds(fireRate);
        }

        isBursting = false;
    }

    // ================= AUDIO =================
    void PlayShotSound()
    {
        if (!audioSource) return;

        AudioClip clip = powerType switch
        {
            PowerType.Spring => springShot,
            PowerType.Gas => gasShot,
            PowerType.Electric => electricShot,
            PowerType.HPA => hpaShot,
            _ => null
        };

        if (clip)
            audioSource.PlayOneShot(clip);
    }

    // ================= RELOAD =================
    public void Reload()
    {
        if (isReloading)
            return;

        if (currentAmmo >= magazineSize)
            return; // 🔥 PEŁNY MAG – brak dźwięku i animacji

        if (reserveAmmo <= 0)
            return;

        StartCoroutine(ReloadRoutine());
    }


    IEnumerator ReloadRoutine()
    {
        isReloading = true;

        if (reloadSound && audioSource)
            audioSource.PlayOneShot(reloadSound);

        yield return new WaitForSeconds(reloadTime);

        int needed = magazineSize - currentAmmo;
        int load = Mathf.Min(needed, reserveAmmo);

        currentAmmo += load;
        reserveAmmo -= load;

        isReloading = false;

        // ===== SPRING =====
        if (powerType == PowerType.Spring)
        {
            if (reloadFromEmpty)
            {
                BeginSpringCock();
            }

            reloadFromEmpty = false;
            yield break;
        }

        // ===== GAS / ELECTRIC / HPA =====
        if (needsCockAfterReload && reloadFromEmpty)
        {
            Cock();
        }

        needsCockAfterReload = false;
        reloadFromEmpty = false;
    }

    // ================= COCK =================
    void Cock()
    {
        if (cockSound && audioSource)
            audioSource.PlayOneShot(cockSound);
    }

    IEnumerator SpringCockRoutine()
    {
        Debug.Log("SPRING COCK START");

        isCocking = true;

        if (cockSound && cockSource)
            audioSource.PlayOneShot(cockSound);

        // ⛔ BLOKADA STRZAŁU TRWA DOKŁADNIE TYLE, ILE DŹWIĘK
        yield return new WaitForSeconds(cockSound.length);

        isCocking = false;

        Debug.Log("SPRING COCK END");
    }


    // ================= FIRE MODE =================
    public void SwitchFireMode()
    {
        if (powerType == PowerType.Spring)
            return;

        fireMode++;
        if ((int)fireMode > 2)
            fireMode = 0;

        if (fireModeSwitchSound && audioSource)
            audioSource.PlayOneShot(fireModeSwitchSound);
    }
    void BeginSpringCock()
    {
        if (isCocking)
            return;

        StartCoroutine(SpringCockRoutine());
    }

    public void AddReserveAmmoClamped(int amount)
    {
        reserveAmmo = Mathf.Min(reserveAmmo + amount, maxReserveAmmo);
    }
}
