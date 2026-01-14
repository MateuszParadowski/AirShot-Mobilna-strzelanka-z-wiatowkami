using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{
    WeaponHolder holder;

    void Start()
    {
        holder = GetComponentInChildren<WeaponHolder>();
    }

    void Update()
    {
        if (!Application.isMobilePlatform && Input.GetKeyDown(KeyCode.Q))
            holder.SwitchWeapon();
    }
}
