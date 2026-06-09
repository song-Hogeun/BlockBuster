using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance; 

    [SerializeField] private GameObject blockPrefab; 
    private Queue<Block> poolingObjectQueue = new Queue<Block>();

    [SerializeField] private int boxNum = 100;

    public int ActiveBlockCount { get; private set; } 

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Init(boxNum);
    }

    private void Init(int initCount) 
    {
        for (int i = 0; i < initCount; i++) 
        {
            poolingObjectQueue.Enqueue(CreateBlock()); 
        }
    }

    private Block CreateBlock() 
    {
        var newObj = Instantiate(blockPrefab).GetComponent<Block>(); 
        newObj.gameObject.SetActive(false); 
        newObj.transform.SetParent(transform); 

        return newObj;
    }

    public static Block GetObject() 
    {
        Instance.ActiveBlockCount++; 

        if (Instance.poolingObjectQueue.Count > 0) 
        {
            var obj = Instance.poolingObjectQueue.Dequeue(); 
            obj.transform.SetParent(Instance.transform); 
            obj.gameObject.SetActive(true); 

            return obj;
        }
        else 
        {
            var newObj = Instance.CreateBlock(); 
            newObj.gameObject.SetActive(true); 
            newObj.transform.SetParent(Instance.transform); 

            return newObj;
        }
    }

    public void ReturnObject(Block block)
    {
        block.gameObject.SetActive(false);
        block.transform.SetParent(Instance.transform);
        poolingObjectQueue.Enqueue(block);
        ActiveBlockCount--; 
    }
}
