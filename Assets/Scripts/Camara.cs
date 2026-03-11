using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;
    public Vector2 deadZone = new Vector2(2f, 1f);
    public float smoothSpeed = 5f;

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = transform.position;

        float deltaX = target.position.x - transform.position.x;
        if (Mathf.Abs(deltaX) > deadZone.x)
        {
            targetPos.x = Mathf.Lerp(transform.position.x, target.position.x - Mathf.Sign(deltaX) * deadZone.x, Time.deltaTime * smoothSpeed);
        }

        float deltaY = target.position.y - transform.position.y;
        if (Mathf.Abs(deltaY) > deadZone.y)
        {
            targetPos.y = Mathf.Lerp(transform.position.y, target.position.y - Mathf.Sign(deltaY) * deadZone.y, Time.deltaTime * smoothSpeed);
        }

        targetPos.z = transform.position.z;

        transform.position = targetPos;
    }
}