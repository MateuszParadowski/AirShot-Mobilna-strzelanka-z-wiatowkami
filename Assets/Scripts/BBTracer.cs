using UnityEngine;

public class BBTracer : MonoBehaviour
{
    public float speed = 80f;
    Vector3 target;

    public void Init(Vector3 hitPoint)
    {
        target = hitPoint;
        Destroy(gameObject, 0.25f);
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );
    }
}
