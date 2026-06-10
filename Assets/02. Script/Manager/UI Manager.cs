using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private SoundManager soundManager;
    private ComboManager comboManager;
    private PlayerCtrl player;
    private Ground ground;

    [SerializeField] private GameObject tempGroundPrefab;

    [SerializeField] private TextMeshProUGUI inGame_noticeText;
    [SerializeField] private TextMeshProUGUI inGame_currScoreText;
    [SerializeField] private TextMeshProUGUI inGame_maxScoreText;


    [SerializeField] private TextMeshProUGUI Die_currScoreText;
    [SerializeField] private TextMeshProUGUI Die_maxScoreText;

    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI multiplierText;

    [Header("Combo Text Animation")]
    [SerializeField] private float comboPopScale = 1.35f;
    [SerializeField] private float comboPopDuration = 0.12f;
    [SerializeField] private float comboReturnDuration = 0.12f;

    [SerializeField] private GameObject startSet;
    [SerializeField] private GameObject inGameSet;
    [SerializeField] private GameObject DieSet;
    [SerializeField] private GameObject joyStickUI;
    [SerializeField] private GameObject extraLife;

    private bool hasDieUIShown = false;
    private float startTime;

    public bool IsGameReady { get; private set; }
    public bool IsGameStarted { get; private set; }
    public bool IsGamePaused { get; set; }
    public bool IsGameOver { get; set; }

    private Coroutine comboAnimCoroutine;
    private Coroutine extraLifeCoroutine;
    private Vector3 comboTextOriginScale;


    private void Awake()
    {
        Instance = this;
        IsGameReady = false;
        IsGameStarted = false;
        IsGamePaused = false;
        IsGameOver = false;
    }

    void Start()
{
    player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>();
    ground = FindFirstObjectByType<Ground>();
    startTime = 3f;

    if (comboText != null)
        comboTextOriginScale = comboText.transform.localScale;

    comboManager = ComboManager.Instance;

    if (comboManager != null)
    {
        comboManager.OnComboChanged += UpdateComboText;
        comboManager.OnMultiplierChanged += UpdateMultiplierText;

        UpdateComboText(comboManager.CurrentCombo);
        UpdateMultiplierText(comboManager.CurrentMultiplier);
    }
    else
    {
        UpdateComboText(0);
        UpdateMultiplierText(1f);
    }

    Init(false, true, false, false, false, false);
}

    private void Update()
    {
        if (!IsGameReady || player == null || hasDieUIShown)
            return;
        
        UpdateText();

        if (player.ISDead)
        {
            ground.StopAllCoroutines();
            ground.gameObject.SetActive(false);
            ShowDieUI();
        }
    }

    private void OnDestroy()
    {
        if (comboManager == null)
            return;

        comboManager.OnComboChanged -= UpdateComboText;
        comboManager.OnMultiplierChanged -= UpdateMultiplierText;
    }

    void Init(bool _gameStart, bool _start, bool _inGame, bool _die, bool _joyStick, bool _extraLife)
    {
        IsGameStarted = _gameStart;
        startSet.SetActive(_start);
        inGameSet.SetActive(_inGame);
        DieSet.SetActive(_die);
        joyStickUI.SetActive(_joyStick);
        extraLife.SetActive(_extraLife);
    }

    public void StartPhase()
    {
        if (IsGameStarted)
            return;

        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        Init(false, false, true, false, false, false);
        IsGameReady = true;
        
        for (int i = 0; i < startTime; i++)
        {
            inGame_noticeText.text = Mathf.Ceil(startTime - i).ToString();
            soundManager.EffectSoundPlay("Number");
            yield return new WaitForSeconds(1f);
        }
        
        joyStickUI.SetActive(true);

        inGame_noticeText.text = "";
    }
    
    public void SetGameStarted(bool GameStarted)
    {
        IsGameStarted = GameStarted;
    }

    private void UpdateText()
    {
        inGame_currScoreText.text = scoreManager.currScore.ToString() + " m";
        inGame_maxScoreText.text = scoreManager.maxScore.ToString() + " m";
    }

    private void UpdateComboText(int combo)
    {
        if (comboText == null)
            return;

        if (combo <= 0)
        {
            comboText.gameObject.SetActive(false);
            return;
        }

        comboText.gameObject.SetActive(true);

        string comboNumberText = GetColoredComboNumber(combo);
        comboText.text = $"COMBO {comboNumberText}";

        PlayComboTextAnimation();
    }

    private void UpdateMultiplierText(float multiplier)
    {
        if (multiplierText == null)
            return;

        if (multiplier <= 1f)
        {
            multiplierText.gameObject.SetActive(false);
            return;
        }

        multiplierText.gameObject.SetActive(true);
        multiplierText.text = $"x{multiplier:0.0}";
    }

    private void ShowDieUI()
    {
        Init(false, false, false, true, false, false);
        Die_currScoreText.text = scoreManager.currScore.ToString() + " m";
        Die_maxScoreText.text = scoreManager.maxScore.ToString() + " m";

        hasDieUIShown = true;
    }

    public void RestartGame()
    {
        if (comboManager != null)
            comboManager.ResetCombo();

        scoreManager.ResetScore();
        ground.gameObject.SetActive(true);
        ground.StopAllCoroutines();
        SceneManager.LoadScene(0);
    }

    public void ActiveExtraLife(bool isActive)
    {
        extraLife.SetActive(isActive);
        
        if(isActive)
            PlayExtraLifeTextAnimation();
    }

    private void PlayExtraLifeTextAnimation()
    {
        if (extraLife == null)
            return;

        if (extraLifeCoroutine != null)
            StopCoroutine(extraLifeCoroutine);
        
        extraLifeCoroutine = StartCoroutine(ExtraLifeAnimCoroutine());
    }

    IEnumerator ExtraLifeAnimCoroutine()
    {
        extraLife.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        extraLife.SetActive(false);
    }

    public void GroundItem()
    {
        Vector3 pos = player.transform.position;
        Vector3 targetPos = new Vector3(pos.x, pos.y - 1f, 0f);
        GameObject tempGround = Instantiate(tempGroundPrefab, targetPos, Quaternion.identity);
    }

    private void PlayComboTextAnimation()
    {
        if (comboText == null)
            return;

        if (comboAnimCoroutine != null)
            StopCoroutine(comboAnimCoroutine);

        comboAnimCoroutine = StartCoroutine(ComboTextPopCoroutine());
    }

    private IEnumerator ComboTextPopCoroutine()
    {
        Transform textTransform = comboText.transform;

        Vector3 targetScale = comboTextOriginScale * comboPopScale;

        float timer = 0f;

        while (timer < comboPopDuration)
        {
            timer += Time.deltaTime;
            float t = timer / comboPopDuration;

            textTransform.localScale = Vector3.Lerp(comboTextOriginScale, targetScale, t);

            yield return null;
        }

        timer = 0f;

        while (timer < comboReturnDuration)
        {
            timer += Time.deltaTime;
            float t = timer / comboReturnDuration;

            textTransform.localScale = Vector3.Lerp(targetScale, comboTextOriginScale, t);

            yield return null;
        }

        textTransform.localScale = comboTextOriginScale;
    }

    private string GetColoredComboNumber(int combo)
    {
        string colorCode = "";

        if (combo >= 1000)
            colorCode = "#A020F0"; // 보라색
        else if (combo >= 500)
            colorCode = "#FF0000"; // 빨간색
        else if (combo >= 100)
            colorCode = "#FF8C00"; // 주황색
        else if (combo >= 50)
            colorCode = "#FFD700"; // 노란색

        if (string.IsNullOrEmpty(colorCode))
            return combo.ToString();

        return $"<color={colorCode}>{combo}</color>";
    }
}