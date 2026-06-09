using System.Collections;
using TMPro;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private PlayerCtrl player;
    private enum LevelType
    {
        Lv1,
        Lv2,
        Lv3,
        Lv4,
        Infinity
    }

    [SerializeField] private LevelType e_level;

    private PlayerCtrl playerCtrl;

    public static float moveSpeed;
    private float currentTime;

    [SerializeField] private float levelUpTime;
    [SerializeField] private float speedAcceleration;
    [SerializeField] private int currentLevel;
    [SerializeField] private float spawnTime;

    void Awake()
    {
        uiManager = FindFirstObjectByType<UIManager>();
    }

    private void Start()
    {
        playerCtrl = FindFirstObjectByType<PlayerCtrl>();

        e_level = LevelType.Lv1;
        currentLevel = 1;
        levelUpTime = 20f;
        speedAcceleration = 0.5f;
        moveSpeed = 1.6f;
        spawnTime = 1f;

        StartCoroutine(CreateBlockLoop());
    }

    private void Update()
    {
        if (uiManager != null && !uiManager.IsGameReady)
            return;

        currentTime += Time.deltaTime;

        if (currentTime >= levelUpTime)
        {
            moveSpeed += speedAcceleration;
            currentTime = 0f;

            CheckLevelUp();
        }
    }

    void CheckLevelUp()
    {
        if (moveSpeed >= 10.0f && currentLevel < 5) e_level = LevelType.Infinity;
        else if (moveSpeed >= 8.0f && currentLevel < 4) e_level = LevelType.Lv4;
        else if (moveSpeed >= 5.0f && currentLevel < 3) e_level = LevelType.Lv3;
        else if (moveSpeed >= 2.5f && currentLevel < 2) e_level = LevelType.Lv2;

        switch (e_level)
        {
            case LevelType.Lv1:
                SetLevelSystem(1, 1, 15f, 0.5f);
                break;
            case LevelType.Lv2:
                SetLevelSystem(2, 2, 15f, 0.55f);
                break;
            case LevelType.Lv3:
                SetLevelSystem(3, 3, 20f, 0.7f);
                break;
            case LevelType.Lv4:
                SetLevelSystem(4, 4, 20f, 0.9f);
                break;
            case LevelType.Infinity:
                SetLevelSystem(5, 5, 25f, 1.0f);
                break;
        }
    }

    private void SetLevelSystem(int _level, float _spawnTime,
        float _levelUpTime, float _speedAcceleration)
    {
        currentLevel = _level;
        spawnTime = _spawnTime;
        levelUpTime = _levelUpTime;
        speedAcceleration = _speedAcceleration;
    }

    IEnumerator CreateBlockLoop()
    {
        float lastX = float.MinValue;
        const float minDist = 1.5f;
        float maxJumpY = playerCtrl.DidPlayerExit
            ? playerCtrl.transform.position.y + Time.deltaTime
            : playerCtrl.transform.position.y;

        while (true)
        {
            if ((uiManager == null || !uiManager.IsGameReady) && !uiManager.IsGameOver)
            {
                yield return null;
                continue;
            }

            int x;
            int tries = 10;
            do
            {
                x = Random.Range(-4, 5);
            } while (Mathf.Abs(x - lastX) < minDist && --tries > 0);

            lastX = x;

            float currY = playerCtrl.transform.position.y;

            if (playerCtrl.ISDashAvail)
            {
                if (currY > maxJumpY)
                    maxJumpY = currY;
            }
            else
            {
                maxJumpY = currY;
            }

            float blockY = playerCtrl.ISDashAvail ? maxJumpY + 13f : currY + 10f;

            int ranNum = Random.Range(0, 201);

            Block block = ObjectPool.GetObject();

            if (ranNum < 1)
            {
                block.itemIndex = Block.ItemIndex.Life;
                block.ItemUpdate();
            }
            else if (ranNum >= 1 && ranNum < 6)
            {
                block.itemIndex = Block.ItemIndex.Jump;
                block.ItemUpdate();
            }
            else if (ranNum >= 6 && ranNum < 11)
            {
                block.itemIndex = Block.ItemIndex.Ground;
                block.ItemUpdate();
            }
            else
            {
                block.itemIndex = Block.ItemIndex.None;
                block.ItemUpdate();
            }

            block.transform.position = new Vector3(x, blockY, 0f);

            if (uiManager.IsGameStarted) spawnTime = Random.Range(0.2f, 0.3f);
            else spawnTime = Random.Range(0.8f, 1.0f);

            yield return new WaitForSeconds(spawnTime);
        }
    }
}