using System.Collections;
using UnityEngine;

public class Ground : MonoBehaviour
{
    [SerializeField] private PlayerCtrl playerCtrl;
    
    private Animator groundAnim;

    void Awake()
    {
        playerCtrl = FindFirstObjectByType<PlayerCtrl>();
        groundAnim = GetComponent<Animator>();
        groundAnim.SetBool("isBroke", false);
    }

    void OnEnable()
    {
        groundAnim.SetBool("isBroke", false);
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (!playerCtrl.DidPlayerExit)
        {
            return;
        }
        if (other.gameObject.CompareTag("Player"))
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(GroundCollapse());
            }
        }
    }

    IEnumerator GroundCollapse()
    {
        yield return new WaitForSeconds(2.25f);

        groundAnim.SetTrigger("Broke");

        yield return new WaitForSeconds(2.25f);
        groundAnim.SetBool("isBroke", true);
        groundAnim.gameObject.SetActive(false);
    }
}
