using System;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Transform player;

    public int currScore { get; private set; }
    public int maxScore { get; private set; }

    private float currHeight;
    private float maxHeight;
    private int heightScoreMultiplier = 1;

    private void Start()
    {
        currScore = 0;
        maxScore = PlayerPrefs.GetInt("MaxScore", 0);
    }

    private void Update()
    {
        if (UIManager.Instance != null && !UIManager.Instance.IsGameStarted)
            return;

        UpdateScore();
    }

    void UpdateScore()
    {
        float currHeight = player.position.y;

        if (currHeight > maxHeight)
        {
            maxHeight = currHeight;

            currScore = Mathf.FloorToInt(maxHeight * heightScoreMultiplier);
            currScore = Mathf.Max(0, currScore);

            if (currScore > maxScore)
            {
                maxScore = currScore;
                PlayerPrefs.SetInt("MaxScore", maxScore);
            }
        }
    }

    public void ResetScore()
    {
        currScore = 0;
        maxHeight = 0f;
    }
}