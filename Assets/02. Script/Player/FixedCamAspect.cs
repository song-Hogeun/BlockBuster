using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraAspect : MonoBehaviour
{
    private Camera cam;

    private float baseWidth = 720f;
    private float baseHeight = 1280f;

    void Awake()
    {
        cam = GetComponent<Camera>();
        UpdateView();
    }

    void Update()
    {
        UpdateView();
    }

    void UpdateView()
    {
        float targetAspect = baseWidth / baseHeight;
        float windowAspect = (float)Screen.width / Screen.height;

        float baseSize = baseHeight / 200f;

        if (windowAspect > targetAspect)
        {
            cam.orthographicSize = baseSize;
        }
        else
        {
            cam.orthographicSize = baseSize * (targetAspect / windowAspect);
        }
    }
}