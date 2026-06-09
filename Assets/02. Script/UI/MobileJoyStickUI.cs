using UnityEngine;
using UnityEngine.EventSystems;

public class MobileJoyStickUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
     private PlayerCtrl playerCtrl;
     private DotLineUI dotLineUI;
     private Transform playerTf;
    
     [Header("Joystick UI")]
     [SerializeField] private GameObject backgroundUI;
     [SerializeField] private GameObject handlerUI;
    
     [Header("Joystick Settings")]
     [SerializeField] private float maxDist = 95f;
     [SerializeField] private float previewDistance = 3f;
    
     private Vector2 startPos;
     private Vector2 currPos;
     private Vector2 inputDir;
    
     private int activePointerId = -1;
     private bool isDragging;
    
     private void Awake()
     {
         playerCtrl = FindFirstObjectByType<PlayerCtrl>();
         dotLineUI = FindFirstObjectByType<DotLineUI>();
    
         GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
         if (playerObj != null)
             playerTf = playerObj.transform;
    
         if (backgroundUI != null)
             backgroundUI.SetActive(false);
     }
    
     public void OnPointerDown(PointerEventData eventData)
     {
         // 이미 다른 손가락이 조이스틱을 잡고 있으면 무시
         if (activePointerId != -1)
             return;
    
         activePointerId = eventData.pointerId;
         isDragging = true;
    
         startPos = eventData.position;
         currPos = startPos;
         inputDir = Vector2.zero;
    
         if (backgroundUI != null)
         {
             backgroundUI.SetActive(true);
             backgroundUI.transform.position = startPos;
         }
    
         if (handlerUI != null)
         {
             handlerUI.transform.position = startPos;
         }
    
         if (dotLineUI != null)
             dotLineUI.HideDots();
     }
    
     public void OnDrag(PointerEventData eventData)
     {
         if (!IsValidPointer(eventData))
             return;
    
         currPos = eventData.position;
    
         UpdateJoystick(currPos);
     }
    
     public void OnPointerUp(PointerEventData eventData)
     {
         if (!IsValidPointer(eventData))
             return;
    
         Vector2 finalPos = eventData.position;
    
         UpdateJoystick(finalPos);
    
         if (playerCtrl != null)
             playerCtrl.InputJoyStick(inputDir.x, inputDir.y);
    
         ResetJoystick();
     }
    
     private void UpdateJoystick(Vector2 pointerPos)
     {
         Vector2 dragDir = pointerPos - startPos;
    
         if (dragDir.sqrMagnitude <= 0.001f)
         {
             inputDir = Vector2.zero;
             return;
         }
    
         float distance = Mathf.Min(dragDir.magnitude, maxDist);
         float movePower = distance / maxDist;
    
         Vector2 normalizedDir = dragDir.normalized;
    
         // 기존 코드와 동일하게 반대 방향으로 발사/이동 입력
         inputDir = -normalizedDir * movePower;
    
         if (handlerUI != null)
         {
             handlerUI.transform.position = startPos + normalizedDir * distance;
         }
    
         if (dotLineUI != null && playerTf != null)
         {
             Vector3 startWorldPos = playerTf.position;
             Vector3 endWorldPos = startWorldPos + new Vector3(inputDir.x, inputDir.y, 0f).normalized * movePower * previewDistance;
    
             dotLineUI.UpdateDots(startWorldPos, endWorldPos);
         }
     }
    
     private void ResetJoystick()
     {
         activePointerId = -1;
         isDragging = false;
    
         startPos = Vector2.zero;
         currPos = Vector2.zero;
         inputDir = Vector2.zero;
    
         if (handlerUI != null)
             handlerUI.transform.localPosition = Vector3.zero;
    
         if (backgroundUI != null)
             backgroundUI.SetActive(false);
    
         if (dotLineUI != null)
             dotLineUI.HideDots();
     }
    
     private bool IsValidPointer(PointerEventData eventData)
     {
         if (!isDragging)
             return false;
    
         if (eventData.pointerId != activePointerId)
             return false;
    
         return true;
     }
}