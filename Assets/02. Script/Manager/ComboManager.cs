using System;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    public static ComboManager Instance { get; private set; }

    [Header("Combo Settings")]
    [SerializeField] private float comboResetTime = 2f;

    [Header("Score Settings")]
    [SerializeField] private int baseBlockScore = 10;

    public int CurrentCombo { get; private set; }
    public float CurrentMultiplier { get; private set; } = 1f;

    private float lastComboTime;
    private bool isComboActive;

    public event Action<int> OnComboChanged;
    public event Action<float> OnMultiplierChanged;
    public event Action OnComboReset;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        ResetCombo();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        if (!isComboActive)
            return;

        if (Time.time - lastComboTime >= comboResetTime)
        {
            ResetCombo();
        }
    }

    public float AddCombo()
    {
        CurrentCombo++;
        lastComboTime = Time.time;
        isComboActive = true;

        UpdateMultiplier();

        OnComboChanged?.Invoke(CurrentCombo);
        OnMultiplierChanged?.Invoke(CurrentMultiplier);

        return CurrentMultiplier;
    }

    public void ResetCombo()
    {
        CurrentCombo = 0;
        CurrentMultiplier = 1f;
        isComboActive = false;

        OnComboChanged?.Invoke(CurrentCombo);
        OnMultiplierChanged?.Invoke(CurrentMultiplier);
        OnComboReset?.Invoke();
    }

    private void UpdateMultiplier()
    {
        if (CurrentCombo >= 1000)
            CurrentMultiplier = 5f;
        else if (CurrentCombo >= 500)
            CurrentMultiplier = 4f;
        else if (CurrentCombo >= 100)
            CurrentMultiplier = 3f;
        else if (CurrentCombo >= 100)
            CurrentMultiplier = 2f;
        else if (CurrentCombo >= 50)
            CurrentMultiplier = 1.5f;
        else if (CurrentCombo >= 20)
            CurrentMultiplier = 1.2f;
        else
            CurrentMultiplier = 1f;
    }
}