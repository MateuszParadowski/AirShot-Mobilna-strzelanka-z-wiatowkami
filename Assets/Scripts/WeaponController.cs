using UnityEngine;
using TMPro;

public class WeaponController : MonoBehaviour
{
    [Header("Current Weapon")]
    public Weapon currentWeapon;

    [Header("UI")]
    public TMP_Text ammoText;
    public TMP_Text reloadWarningText;

    [Header("ADS")]
    public Camera playerCamera;
    public float hipFOV = 75f;
    public float adsFOV = 55f;
    public float adsSpeed = 8f;

    [Header("PC Keybinds")]
    public KeyCode fireKey = KeyCode.Mouse0;
    public KeyCode reloadKey = KeyCode.R;
    public KeyCode fireModeKey = KeyCode.B;

    bool isAiming;

    void Update()
    {
        if (!currentWeapon)
        {
            ammoText.text = "";
            reloadWarningText.gameObject.SetActive(false);
            return;
        }

        bool fireDown = false;
        bool fireHeld = false;

        if (!Application.isMobilePlatform)
        {
            fireDown = Input.GetKeyDown(fireKey);
            fireHeld = Input.GetKey(fireKey);

            if (Input.GetKeyDown(reloadKey))
            currentWeapon.Reload();

            if (Input.GetKeyDown(fireModeKey))
                currentWeapon.SwitchFireMode();

            if (Input.GetMouseButtonDown(1)) AimDown();
            if (Input.GetMouseButtonUp(1)) AimUp();
        }

        bool fireInput = false;

        switch (currentWeapon.fireMode)
        {
            case Weapon.FireMode.Single:
            case Weapon.FireMode.Burst:
                fireInput = fireDown;
                break;

            case Weapon.FireMode.Auto:
                fireInput = fireHeld;
                break;
        }

        UpdateADS();
        currentWeapon.HandleFire(fireInput);
        UpdateAmmoUI();
        UpdateReloadWarnings();
    }

    void UpdateADS()
    {
        float targetFOV = isAiming ? adsFOV : hipFOV;
        playerCamera.fieldOfView = Mathf.Lerp(
            playerCamera.fieldOfView,
            targetFOV,
            Time.deltaTime * adsSpeed
        );

        Transform pivot = currentWeapon.transform.parent;
        Vector3 targetPos = isAiming
            ? currentWeapon.adsPosition
            : currentWeapon.hipPosition;

        pivot.localPosition = Vector3.Lerp(
            pivot.localPosition,
            targetPos,
            Time.deltaTime * currentWeapon.adsSpeed
        );
    }

    // ===== UI / MOBILE =====
    public void FireButton() => currentWeapon.HandleFire(true);
    public void ReloadButton() => currentWeapon.Reload();
    public void AimDown() => isAiming = true;
    public void AimUp() => isAiming = false;
    public void SwitchFireMode() => currentWeapon.SwitchFireMode();

    void UpdateAmmoUI()
    {
        if (!ammoText || !currentWeapon) return;
        ammoText.text = $"{currentWeapon.currentAmmo}/{currentWeapon.reserveAmmo}";
    }
    void UpdateReloadWarnings()
    {
        if (!reloadWarningText || !currentWeapon) return;

        // ⛔ BRAK AMUNICJI
        if (currentWeapon.currentAmmo == 0 &&
            currentWeapon.reserveAmmo == 0)
        {
            reloadWarningText.gameObject.SetActive(true);
            reloadWarningText.text = "NO AMMO";
            reloadWarningText.color = Color.red;
            return;
        }

        // 🔔 MAŁO AMUNICJI
        if (currentWeapon.currentAmmo <= currentWeapon.lowAmmoThreshold)
        {
            reloadWarningText.gameObject.SetActive(true);
            reloadWarningText.text = "RELOAD";
            reloadWarningText.color = Color.yellow;
            return;
        }

        // ✅ OK
        reloadWarningText.gameObject.SetActive(false);
    }
}
