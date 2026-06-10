using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType
    {
        Jump,
        Shield,
        ScoreBonus,
    }

    [Header("Item Info")]
    [SerializeField] private ItemType itemType;
    [SerializeField] private int price = 100;

    [Header("UI")]
    [SerializeField] private GameObject checkMark;

    private bool isBuy;

    public ItemType Type => itemType;
    public int Price => price;
    public bool IsBuy => isBuy;

    private void Awake()
    {
        ActiveCheckMark(false);
    }

    public bool TryBuy()
    {
        if (isBuy)
        {
            Debug.Log($"{itemType} 아이템은 이미 이번 판에 구매했습니다.");
            return false;
        }

        if (GoodsManager.Instance.TrySpendCoin(price) == false)
            return false;

        BuyComplete();
        return true;
    }

    private void BuyComplete()
    {
        isBuy = true;
        ActiveCheckMark(true);

        Debug.Log($"{itemType} 아이템 구매 완료");
    }

    public void ResetItem()
    {
        isBuy = false;
        ActiveCheckMark(false);
    }

    public void ActiveCheckMark(bool isActive)
    {
        if (checkMark != null)
            checkMark.SetActive(isActive);
    }
}