using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private ComboManager comboManager;

    [Header("InGame Text")]
    [SerializeField] private TextMeshProUGUI inGame_noticeText;
    [SerializeField] private TextMeshProUGUI inGame_currScoreText;
    [SerializeField] private TextMeshProUGUI inGame_maxScoreText;

    [Header("Die Text")]
    [SerializeField] private TextMeshProUGUI die_currScoreText;
    [SerializeField] private TextMeshProUGUI die_maxScoreText;

    [Header("Combo Text")]
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI multiplierText;

    [Header("Combo Text Animation")]
    [SerializeField] private float comboPopScale = 1.35f;
    [SerializeField] private float comboPopDuration = 0.12f;
    [SerializeField] private float comboReturnDuration = 0.12f;

    [Header("UI Sets")]
    [SerializeField] private GameObject startSet;
    [SerializeField] private GameObject inGameSet;
    [SerializeField] private GameObject dieSet;
    [SerializeField] private GameObject joyStickUI;
    [SerializeField] private GameObject extraLife;

    private Coroutine comboAnimCoroutine;
    private Coroutine extraLifeCoroutine;
    private Vector3 comboTextOriginScale;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
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
    }

    private void OnDestroy()
    {
        if (comboManager == null)
            return;

        comboManager.OnComboChanged -= UpdateComboText;
        comboManager.OnMultiplierChanged -= UpdateMultiplierText;
    }

    public void ShowStartUI()
    {
        SetUI(
            start: true,
            inGame: false,
            die: false,
            joystick: false,
            extraLifeActive: false
        );
    }

    public void ShowInGameUI(bool joystickActive)
    {
        SetUI(
            start: false,
            inGame: true,
            die: false,
            joystick: joystickActive,
            extraLifeActive: false
        );
    }

    public void ShowDieUI(int currScore, int maxScore)
    {
        SetUI(
            start: false,
            inGame: false,
            die: true,
            joystick: false,
            extraLifeActive: false
        );

        if (die_currScoreText != null)
            die_currScoreText.text = $"{currScore} m";

        if (die_maxScoreText != null)
            die_maxScoreText.text = $"{maxScore} m";
    }

    private void SetUI(bool start, bool inGame, bool die, bool joystick, bool extraLifeActive)
    {
        if (startSet != null)
            startSet.SetActive(start);

        if (inGameSet != null)
            inGameSet.SetActive(inGame);

        if (dieSet != null)
            dieSet.SetActive(die);

        if (joyStickUI != null)
            joyStickUI.SetActive(joystick);

        if (extraLife != null)
            extraLife.SetActive(extraLifeActive);
    }

    public void SetNoticeText(string text)
    {
        if (inGame_noticeText != null)
            inGame_noticeText.text = text;
    }

    public void SetJoystickActive(bool isActive)
    {
        if (joyStickUI != null)
            joyStickUI.SetActive(isActive);
    }

    public void UpdateScoreText(int currScore, int maxScore)
    {
        if (inGame_currScoreText != null)
            inGame_currScoreText.text = $"{currScore} m";

        if (inGame_maxScoreText != null)
            inGame_maxScoreText.text = $"{maxScore} m";
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

    public void ActiveExtraLife(bool isActive)
    {
        if (extraLife == null)
            return;

        extraLife.SetActive(isActive);

        if (isActive)
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

    private IEnumerator ExtraLifeAnimCoroutine()
    {
        extraLife.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        extraLife.SetActive(false);
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
            colorCode = "#A020F0";
        else if (combo >= 500)
            colorCode = "#FF0000";
        else if (combo >= 100)
            colorCode = "#FF8C00";
        else if (combo >= 50)
            colorCode = "#FFD700";

        if (string.IsNullOrEmpty(colorCode))
            return combo.ToString();

        return $"<color={colorCode}>{combo}</color>";
    }
}