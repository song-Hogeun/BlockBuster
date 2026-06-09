using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private Transform player;

    public int currScore { get; private set; }
    public int maxScore { get; private set; }

    private float maxHeight;
    private int heightScoreMultiplier = 1;

    private int heightScore;
    private int blockScore;

    private void Awake()
    {
        Instance = this;
    }

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

            heightScore = Mathf.FloorToInt(maxHeight * heightScoreMultiplier);
            heightScore = Mathf.Max(0, heightScore);
        }

        currScore = heightScore + blockScore;

        if (currScore > maxScore)
        {
            maxScore = currScore;
            PlayerPrefs.SetInt("MaxScore", maxScore);
        }
    }

    public void AddBlockScore(float score)
    {
        blockScore *= (int)score;

        currScore = heightScore + blockScore;

        if (currScore > maxScore)
        {
            maxScore = currScore;
            PlayerPrefs.SetInt("MaxScore", maxScore);
        }
    }

    public void ResetScore()
    {
        currScore = 0;
        heightScore = 0;
        blockScore = 0;
        maxHeight = 0f;
    }
}