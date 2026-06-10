using System;
using UnityEngine;

public class GoodsManager : MonoBehaviour
{
    public static GoodsManager Instance;
    
    private const string CoinKey = "CurrentCoin";

    private int currentCoin;

    private void Awake()
    {
        Instance = this;
    }

    public int GetCoin()
    {
        return currentCoin;
    }

    public void AddCoin(int coin)
    {
        if (coin <= 0)
            return;

        currentCoin += coin;
        SaveCoin();
    }

    public bool TrySpendCoin(int coin)
    {
        if (coin <= 0)
            return false;

        if (currentCoin < coin)
        {
            Debug.Log("코인이 부족합니다.");
            return false;
        }

        currentCoin -= coin;
        SaveCoin();

        return true;
    }

    private void SaveCoin()
    {
        PlayerPrefs.SetInt(CoinKey, currentCoin);
        PlayerPrefs.Save();
    }
}