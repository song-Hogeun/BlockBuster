using System.Collections.Generic;
using UnityEngine;

public class DotLineUI : MonoBehaviour
{
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private GameObject dotDashPrefab;
    [SerializeField] private GameObject dotNoDashPrefab;
    [SerializeField] private int maxDots = 10;     // 최대 점 개수
    [SerializeField] private float dotSpacing = 30f; // 점 간격 (픽셀)

    private List<RectTransform> dotsDash = new List<RectTransform>();
    private List<RectTransform> dotsNoDash = new List<RectTransform>();
    private UnityEngine.Camera mainCam;
    private PlayerCtrl playerCtrl;

    void Start()
    {
        mainCam = UnityEngine.Camera.main;
        playerCtrl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>();

        // 점 미리 생성
        for (int i = 0; i < maxDots; i++)
        {
            GameObject dotDash = Instantiate(dotDashPrefab, transform);
            dotDash.SetActive(false);
            dotsDash.Add(dotDash.GetComponent<RectTransform>());
        }
        for (int i = 0; i < maxDots; i++)
        {
            GameObject dotNoDash = Instantiate(dotNoDashPrefab, transform);
            dotNoDash.SetActive(false);
            dotsNoDash.Add(dotNoDash.GetComponent<RectTransform>());
        }
    }

    public void UpdateDots(Vector3 worldStartPos, Vector3 worldEndPos)
    {
        // 캐릭터 월드 좌표 → UI 좌표로 변환
        Vector3 startScreenPos = mainCam.WorldToScreenPoint(worldStartPos);
        Vector3 endScreenPos = mainCam.WorldToScreenPoint(worldEndPos);

        Vector3 direction = (endScreenPos - startScreenPos).normalized;

        float distance = Vector3.Distance(startScreenPos, endScreenPos);

        int activeDots = Mathf.Min(maxDots, Mathf.FloorToInt(distance / dotSpacing));

        List<RectTransform> dots = playerCtrl.ISDashAvail ? dotsDash : dotsNoDash;

        List<RectTransform> dotsDis = playerCtrl.ISDashAvail ? dotsNoDash : dotsDash;

        for (int i = 0; i < maxDots; i++)
        {
            if (i < activeDots)
            {
                dots[i].gameObject.SetActive(true);
                dotsDis[i].gameObject.SetActive(false);
                Vector3 pos = startScreenPos + direction * (i * dotSpacing);
                
                dots[i].position = pos;
            }
            else
            {
                dots[i].gameObject.SetActive(false);
            }
        }
    }

    public void HideDots()
    {
        foreach (var dot in dotsDash)
        {
            dot.gameObject.SetActive(false);
        }
        foreach (var dot in dotsNoDash)
        {
            dot.gameObject.SetActive(false);
        }
    }
}
