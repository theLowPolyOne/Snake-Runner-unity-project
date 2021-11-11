using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private ImpactGUI humanPointsIcon;
    [SerializeField] private ImpactGUI gemPointsIcon;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI humanPointsText;
    [SerializeField] private TextMeshProUGUI gemPointsText;
    [SerializeField] private TextMeshProUGUI highscoreText;
    private int currentScore;

    void Awake()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.pauseGameEvent += OnPauseGame;
        gameManager.unpauseGameEvent += OnUnpauseGame;
        FindObjectOfType<PlayerController>().deathEvent += OnPlayerDeath;
        gameManager.newHumanPointEvent += OnHumanPointsChange;
        gameManager.newGemPointEvent += OnGemPointsChange;
        gameManager.newHighscoreEvent += OnHighscoreChange;
    }

    private void Start()
    {
        OnHumanPointsChange(0);
        OnGemPointsChange(0);
    }

    void OnPauseGame()
    {
        pauseScreen.SetActive(true);
    }

    void OnUnpauseGame()
    {
        pauseScreen.SetActive(false);
    }

    private void OnPlayerDeath()
    {
        gameOverScreen.SetActive(true);
        finalScoreText.text = "Your score: " + currentScore;
    }

    private void OnHumanPointsChange(int points)
    {
        currentScore = points;
        humanPointsText.text = points.ToString();
    }

    private void OnGemPointsChange(int points)
    {
        currentScore = points;
        gemPointsText.text = points.ToString();
    }

    private void OnHighscoreChange(int highscore)
    {
        highscoreText.text = "Highscore: " + highscore;
    }
}
