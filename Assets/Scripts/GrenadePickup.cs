using UnityEngine;

public class GrenadePickup : MonoBehaviour
{
    public int amount = 1;

    bool playerInRange;
    GrenadeThrow grenade;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        grenade = other.GetComponent<GrenadeThrow>();
        if (!grenade) return;

        playerInRange = true;
        PickupUI.Instance.Show("Press E to take grenade");
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        grenade = null;
        PickupUI.Instance.Hide();
    }

    void Update()
    {
        if (!playerInRange || grenade == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            grenade.grenadeCount += amount;
            PickupUI.Instance.Hide();
            Destroy(gameObject);
        }
    }
}
