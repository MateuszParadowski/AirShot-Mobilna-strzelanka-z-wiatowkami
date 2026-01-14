using UnityEngine;

public class MovingTarget : MonoBehaviour
{
    public Transform pivot;
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    Vector3 target;
    bool alive = true;

    void Start()
    {
        GetComponentInChildren<Target>().OnHit += OnHit;

        pivot.position = pointA.position;
        target = pointB.position;
    }

    void Update()
    {
        if (!alive) return;

        pivot.position = Vector3.MoveTowards(pivot.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(pivot.position, target) < 0.01f)
            target = target == pointA.position ? pointB.position : pointA.position;
    }

    void OnHit()
    {
        alive = false;
        pivot.gameObject.SetActive(false);
        Invoke(nameof(Respawn), 2f);
    }

    void Respawn()
    {
        pivot.gameObject.SetActive(true);
        alive = true;
    }
}
