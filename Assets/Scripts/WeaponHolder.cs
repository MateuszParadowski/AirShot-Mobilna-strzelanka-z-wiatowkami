using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    public Transform slotA;
    public Transform slotB;
    public WeaponController weaponController;
    public Camera playerCamera;

    GameObject currentA;
    GameObject currentB;
    GameObject activeWeapon;

    public void PickupPrimary(GameObject prefab)
    {
        if (currentA) Destroy(currentA);

        currentA = Instantiate(prefab, slotA);
        currentA.transform.localPosition = Vector3.zero;
        currentA.transform.localRotation = Quaternion.identity;

        Weapon w = currentA.GetComponent<Weapon>();
        w.cam = playerCamera;

        SetActive(currentA);
    }

    public void PickupSecondary(GameObject prefab)
    {
        if (currentB) Destroy(currentB);

        currentB = Instantiate(prefab, slotB);
        currentB.transform.localPosition = Vector3.zero;
        currentB.transform.localRotation = Quaternion.identity;

        Weapon w = currentB.GetComponent<Weapon>();
        w.cam = playerCamera;

        if (!activeWeapon)
            SetActive(currentB);
    }

    public void SwitchWeapon()
    {
        if (!currentA || !currentB) return;

        if (activeWeapon == currentA)
            SetActive(currentB);
        else
            SetActive(currentA);
    }

    void SetActive(GameObject weapon)
    {
        if (currentA) currentA.SetActive(false);
        if (currentB) currentB.SetActive(false);

        activeWeapon = weapon;
        activeWeapon.SetActive(true);

        // 🔥 NAJWAŻNIEJSZA LINIA
        weaponController.currentWeapon = activeWeapon.GetComponent<Weapon>();
    }

    public void AddAmmoToAllWeapons(int amount)
    {
        if (currentA)
        {
            Weapon w = currentA.GetComponent<Weapon>();
            w.reserveAmmo += amount;
        }

        if (currentB)
        {
            Weapon w = currentB.GetComponent<Weapon>();
            w.reserveAmmo += amount;
        }
    }

    public bool HasPrimary()
    {
        return currentA != null;
    }

    public bool HasSecondary()
    {
        return currentB != null;
    }

    public Weapon GetPrimaryWeapon()
    {
        return currentA ? currentA.GetComponent<Weapon>() : null;
    }

    public Weapon GetSecondaryWeapon()
    {
        return currentB ? currentB.GetComponent<Weapon>() : null;
    }

    public Weapon GetActiveWeapon()
    {
        return activeWeapon ? activeWeapon.GetComponent<Weapon>() : null;
    }
}