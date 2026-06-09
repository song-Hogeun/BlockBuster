using UnityEngine;

public class ColliderFollow : MonoBehaviour
{
    private Transform target;

    [SerializeField] private float offset = -5;
    [SerializeField] private float minBoundary = 0f;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void LateUpdate()
    {
        float targetPosY = target.position.y + offset;

        if (targetPosY > minBoundary)
        {
            minBoundary = targetPosY;
        }

        Vector3 pos = transform.position;
        pos.y = minBoundary + offset;
        transform.position = pos;
    }
}
