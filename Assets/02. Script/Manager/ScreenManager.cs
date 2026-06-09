using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    void Awake()
    {
        // 세로 고정
        Screen.orientation = ScreenOrientation.Portrait;

        // 화면 자동 회전 비활성화
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = false;
    }
}
