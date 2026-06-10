using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private Item[] items;

    public void TryBuy(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("구매하려는 Item이 없습니다.");
            return;
        }

        bool success = item.TryBuy();

        if (success == false)
            return;

        ApplyItemEffect(item);
    }

    private void ApplyItemEffect(Item item)
    {
        switch (item.Type)
        {
            case Item.ItemType.Jump:
                TemporaryUpgradeManager.Instance.EnableJumpBuff();
                break;

            case Item.ItemType.Shield:
                TemporaryUpgradeManager.Instance.EnableShieldBuff();
                break;

            case Item.ItemType.ScoreBonus:
                TemporaryUpgradeManager.Instance.EnableScoreBonusBuff();
                break;
        }
    }

    public void ResetShopItems()
    {
        foreach (Item item in items)
        {
            if (item == null)
                continue;

            item.ResetItem();
        }
    }
}