using System.Collections;
using UnityEngine;

public class TempGround : MonoBehaviour
{
    private UIManager uiManager;

    void Awake()
    {
        uiManager = FindFirstObjectByType<UIManager>();
    }

    void OnEnable()
    {
        StartCoroutine(DestroyTemp());
    }

    IEnumerator DestroyTemp()
    {
        float timer = 3f;

        while (timer > 0)
        {
            while (uiManager.IsGamePaused)
            {
                yield return null;
            }

            timer -= Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
    void OnBecameInvisible()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }
}
