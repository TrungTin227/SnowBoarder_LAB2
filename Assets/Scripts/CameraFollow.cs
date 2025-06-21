using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -10);
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = true;

    void Start()
    {
        if (target == null)
        {
            target = FindObjectOfType<PlayerController>().transform;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        if (!followX) desiredPosition.x = transform.position.x;
        if (!followY) desiredPosition.y = transform.position.y;

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}