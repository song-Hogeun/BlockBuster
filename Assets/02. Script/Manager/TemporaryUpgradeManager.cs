using UnityEngine;

public class TemporaryUpgradeManager : MonoBehaviour
{
    public static TemporaryUpgradeManager Instance { get; private set; }

    public bool HasJumpBuff { get; private set; }
    public bool HasShieldBuff { get; private set; }
    public bool HasScoreBonusBuff { get; private set; }

    public float ScoreMultiplier => HasScoreBonusBuff ? 1.5f : 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void EnableJumpBuff()
    {
        HasJumpBuff = true;
        Debug.Log("점프 버프 활성화");
    }

    public void EnableShieldBuff()
    {
        HasShieldBuff = true;
        Debug.Log("쉴드 버프 활성화");
    }

    public void EnableScoreBonusBuff()
    {
        HasScoreBonusBuff = true;
        Debug.Log("점수 보너스 버프 활성화");
    }

    public void ResetTemporaryUpgrades()
    {
        HasJumpBuff = false;
        HasShieldBuff = false;
        HasScoreBonusBuff = false;

        Debug.Log("일시 아이템 효과 초기화");
    }
}