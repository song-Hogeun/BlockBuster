using System.Collections;
using UnityEngine;

public class TempGround : MonoBehaviour
{
    private GameManager gameManager;
    

    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
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
            while (gameManager.IsGamePaused)
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
