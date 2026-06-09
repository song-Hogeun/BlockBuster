using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStickUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private PlayerCtrl playerCtrl;
    private DotLineUI dotLineUI;
    private Transform playerTf;

    [SerializeField] private GameObject backgroundUI;
    [SerializeField] private GameObject handlerUI;

    [SerializeField] private float maxDist = 95f;

    private Vector3 startPos, currPos, playerDir;

    private int touchId = -1;

    void Start()
    {
        playerCtrl = FindFirstObjectByType<PlayerCtrl>();
        dotLineUI = FindFirstObjectByType<DotLineUI>();
        playerTf = GameObject.FindGameObjectWithTag("Player").transform;
        backgroundUI.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != touchId) 
            return;

        currPos = eventData.position;
        Vector3 dragDir = (currPos - startPos);
    
        float distance = Mathf.Min(dragDir.magnitude, maxDist);
        float movePower = distance / maxDist;

        playerDir = -1 * movePower * dragDir.normalized;

        handlerUI.transform.position = startPos + dragDir.normalized * distance;

        dotLineUI.UpdateDots(playerTf.position, playerTf.position + (playerDir.normalized * movePower * 3f));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (touchId != -1 && touchId != eventData.pointerId)
            return;
        touchId = eventData.pointerId;
        backgroundUI.SetActive(true);
        backgroundUI.transform.position = eventData.position;
        startPos = eventData.position;
        playerDir = Vector3.zero;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != touchId)
            return;

        Vector3 finalPos = eventData.position;
        Vector3 finalDir = (finalPos - startPos);

        float distance = Mathf.Min(finalDir.magnitude, maxDist);
        float movePower = distance / maxDist;

        playerDir = -1 * movePower * finalDir.normalized;

        playerCtrl.InputJoyStick(playerDir.x, playerDir.y);

        handlerUI.transform.localPosition = Vector3.zero;
        backgroundUI.SetActive(false);

        startPos = Vector3.zero;
        currPos = Vector3.zero;

        dotLineUI.HideDots();
    }
}
