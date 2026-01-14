using UnityEngine;

public class GrenadeThrow : MonoBehaviour
{
    public GameObject grenadePrefab;
    public Transform throwPoint;
    public float throwForce = 12f;

    public int grenadeCount = 0;

    public void ThrowGrenade()
    {
        if (grenadeCount <= 0) return;

        grenadeCount--;

        GameObject grenade = Instantiate(grenadePrefab, throwPoint.position, throwPoint.rotation);
        grenade.GetComponent<Rigidbody>().AddForce(throwPoint.forward * throwForce, ForceMode.Impulse);
    }

    void Update()
    {
        if (!Application.isMobilePlatform && Input.GetKeyDown(KeyCode.G))
            ThrowGrenade();
    }
}
