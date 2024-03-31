using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    [Header("Gameplay Score Text")]
    [SerializeField] private TextMeshProUGUI gameplayScoreText;
    [SerializeField] private TextMeshProUGUI gameplayHighScoreText;

    [Header("Screens")]
    [SerializeField] private GameObject gameplayCanvas;
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject pauseScreen;

    [Header("GameOver Screen")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private TextMeshProUGUI gameoverScoreText;
    [SerializeField] private TextMeshProUGUI gameoverHighScoreText;

    [Header("Scripts Reference")]
    [SerializeField] private GameplayController gameplayController;
    [SerializeField] private ScoreController scoreController;

    private void OnEnable()
    {
        ScoreController.OnScoreUpdate += UpdateScore;
    }

    private void UpdateScore(int currentScore, int highScore)
    {
        gameplayScoreText.text = "Current Score : " + currentScore.ToString();
        gameplayHighScoreText.text = "High Score : " + highScore.ToString();
    }

    public void OnPlayButtonClick()
    {
        gameplayCanvas.SetActive(true);
        mainMenuCanvas.SetActive(false);

        gameplayController.ResetGameplay();
        scoreController.ResetScore();
        gameplayController.StartGame(this, scoreController);
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
    }

    public void OnPauseButtonClick()
    {
        gameplayController.SetGameState(GameState.Paused);
        pauseScreen.SetActive(true);
    }

    public void OnResumeButtonClick()
    {
        gameplayController.SetGameState(GameState.Running);
        pauseScreen.SetActive(false);
    }

    public void OnRestartButtonClick()
    {
        scoreController.ResetScore();
        gameplayController.ResetGameplay();

        gameOverScreen.SetActive(false);
        pauseScreen.SetActive(false);

        gameplayController.StartGame(this, scoreController);
    }

    public void OnHomeButtonClick()
    {
        gameplayController.SetGameState(GameState.Waiting);

        gameOverScreen.SetActive(false);
        pauseScreen.SetActive(false);
        gameplayCanvas.SetActive(false);

        mainMenuCanvas.SetActive(true);
    }

    public void OpenGameOverScreen()
    {
        gameplayController.SetGameState(GameState.GameOver);
        gameoverScoreText.text = "Current Score : " + scoreController.CurrentScore;
        gameoverHighScoreText.text = "High Score : " + scoreController.HighScore;
        gameOverScreen.SetActive(true);
    }

    private void OnDisable()
    {
        ScoreController.OnScoreUpdate -= UpdateScore;
    }
}
