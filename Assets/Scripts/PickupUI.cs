using UnityEngine;
using TMPro;

public class PickupUI : MonoBehaviour
{
    public static PickupUI Instance;
    public TMP_Text pickupText;

    void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show(string msg)
    {
        pickupText.gameObject.SetActive(true);
        pickupText.text = msg;
    }

    public void Hide()
    {
        pickupText.gameObject.SetActive(false);
    }
}
