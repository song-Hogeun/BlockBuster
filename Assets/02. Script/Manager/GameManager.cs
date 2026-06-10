using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Managers")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private SoundManager soundManager;

    [Header("Game Objects")]
    [SerializeField] private PlayerCtrl player;
    [SerializeField] private Ground ground;

    [Header("Item")]
    [SerializeField] private GameObject tempGroundPrefab;

    [Header("Start Setting")]
    [SerializeField] private int startCount = 3;

    public bool IsGameReady { get; private set; }
    public bool IsGameStarted { get; private set; }
    public bool IsGamePaused { get; set; }
    public bool IsGameOver { get; private set; }

    private bool hasDieUIShown;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        IsGameReady = false;
        IsGameStarted = false;
        IsGamePaused = false;
        IsGameOver = false;
        hasDieUIShown = false;
    }

    private void Start()
    {
        ResolveReferences();

        uiManager.ShowStartUI();
    }

    private void Update()
    {
        if (!IsGameReady)
            return;

        if (IsGamePaused)
            return;

        if (IsGameOver)
            return;

        if (player == null)
            return;

        uiManager.UpdateScoreText(scoreManager.currScore, scoreManager.maxScore);

        if (player.ISDead)
        {
            HandlePlayerDead();
        }
    }

    private void ResolveReferences()
    {
        if (uiManager == null)
            uiManager = UIManager.Instance;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerCtrl>();

        if (ground == null)
            ground = FindFirstObjectByType<Ground>();
    }

    public void StartPhase()
    {
        if (IsGameStarted)
            return;

        StartCoroutine(StartGameCoroutine());
    }

    private IEnumerator StartGameCoroutine()
    {
        uiManager.ShowInGameUI(false);

        IsGameReady = true;
        IsGameStarted = false;
        IsGameOver = false;

        for (int i = startCount; i > 0; i--)
        {
            uiManager.SetNoticeText(i.ToString());

            if (soundManager != null)
                soundManager.EffectSoundPlay("Number");

            yield return new WaitForSeconds(1f);
        }

        uiManager.SetNoticeText("");
        uiManager.SetJoystickActive(true);

        IsGameStarted = true;
    }

    private void HandlePlayerDead()
    {
        if (hasDieUIShown)
            return;

        IsGameOver = true;
        hasDieUIShown = true;

        if (ground != null)
        {
            ground.StopAllCoroutines();
            ground.gameObject.SetActive(false);
        }

        uiManager.ShowDieUI(scoreManager.currScore, scoreManager.maxScore);
    }

    public void RestartGame()
    {
        ComboManager comboManager = ComboManager.Instance;

        if (comboManager != null)
            comboManager.ResetCombo();

        if (scoreManager != null)
            scoreManager.ResetScore();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CreateTempGround()
    {
        if (player == null || tempGroundPrefab == null)
            return;

        Vector3 playerPos = player.transform.position;
        Vector3 spawnPos = new Vector3(playerPos.x, playerPos.y - 1f, 0f);

        Instantiate(tempGroundPrefab, spawnPos, Quaternion.identity);
    }
    
    public void SetGameStarted(bool isStarted)
    {
        IsGameStarted = isStarted;
    }

    public void SetGamePaused(bool isPaused)
    {
        IsGamePaused = isPaused;
    }
}