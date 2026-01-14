using UnityEngine;

public class WeaponViewmodel : MonoBehaviour
{
    public Transform cam;
    public float checkDistance = 0.5f;
    public float liftAmount = 0.25f;
    public float smooth = 8f;

    Vector3 defaultLocalPos;

    void Start()
    {
        defaultLocalPos = transform.localPosition;
    }

    void Update()
    {
        Vector3 targetPos = defaultLocalPos;

        if (Physics.Raycast(cam.position, cam.forward, checkDistance))
        {
            targetPos += Vector3.up * liftAmount;
        }

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPos,
            Time.deltaTime * smooth
        );
    }
}
