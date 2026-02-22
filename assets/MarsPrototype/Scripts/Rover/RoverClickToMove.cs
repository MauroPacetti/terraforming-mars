using UnityEngine;

public class RoverClickToMove : MonoBehaviour
{
    public float speed = 6f;
    public float turnSpeed = 220f;
    public LayerMask groundMask = ~0;

    private Vector3 target;
    private bool hasTarget;

    void Start()
    {
        target = transform.position;
        hasTarget = false;
    }

    void Update()
    {
        if (hasTarget)
        {
            Vector3 to = (target - transform.position);
            to.y = 0f;
            float dist = to.magnitude;
            if (dist < 0.2f)
            {
                hasTarget = false;
                return;
            }

            Vector3 dir = to.normalized;
            Quaternion desired = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desired, turnSpeed * Time.deltaTime);
            transform.position += transform.forward * (speed * Time.deltaTime);
        }
    }

    public void SetTarget(Vector3 worldPos)
    {
        target = worldPos;
        hasTarget = true;
    }
}
