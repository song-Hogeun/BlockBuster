using System;
using System.Collections;
using UnityEngine;

public class BackgroundFollow : MonoBehaviour
{
    private Transform target;

    [SerializeField] private SpriteRenderer[] spriteRenderer;

    [SerializeField] private Vector3 offset = new Vector3(0, 1, -10);

    [SerializeField] private Vector3 minBoundary = new Vector3(-2f, 0f, 0f);
    [SerializeField] private Vector3 maxBoundary = new Vector3(2f, 0f, 0f);

    [SerializeField] private float damp = 10f;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void LateUpdate()
    {
        Vector3 targetPos = target.position + offset;

        if (targetPos.y > minBoundary.y)
        {
            minBoundary.y = targetPos.y;
            offset.y = -1f;
        }

        maxBoundary.y = target.position.y + 10f;

        Vector3 smoothPos = Vector3.Lerp(transform.position, targetPos, damp * Time.deltaTime);

        smoothPos.x = Mathf.Clamp(smoothPos.x, minBoundary.x, maxBoundary.x);
        smoothPos.y = Mathf.Clamp(smoothPos.y, minBoundary.y + offset.y, maxBoundary.y);

        transform.position = smoothPos;
    }
}
