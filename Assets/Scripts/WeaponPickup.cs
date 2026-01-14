using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public GameObject weaponPrefab;
    public bool isPrimary;

    bool playerInRange;
    WeaponHolder currentHolder;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        currentHolder = other.GetComponentInChildren<WeaponHolder>();
        if (!currentHolder) return;

        playerInRange = true;
        PickupUI.Instance.Show("Press E to pick up");
        SetHighlight(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        currentHolder = null;
        PickupUI.Instance.Hide();
        SetHighlight(false);
    }

    void Update()
    {
        if (!playerInRange || currentHolder == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isPrimary)
                currentHolder.PickupPrimary(weaponPrefab);
            else
                currentHolder.PickupSecondary(weaponPrefab);

            PickupUI.Instance.Hide();
            Destroy(gameObject);
        }
    }

    void SetHighlight(bool state)
    {
        Renderer[] rends = GetComponentsInChildren<Renderer>();
        foreach (var r in rends)
        {
            r.material.EnableKeyword("_EMISSION");
            r.material.SetColor("_EmissionColor", state ? Color.yellow * 1.5f : Color.black);
        }
    }
}
