using UnityEngine;

public class ScoreController : MonoBehaviour
{
    private int currentScore;
    private int highestScore;

    public delegate void ScoreUpdate(int currentScore, int highScore);
    public static ScoreUpdate OnScoreUpdate;

    public int CurrentScore { get { return currentScore; } }
    public int HighScore { get { return highestScore; } }

    private void Start()
    {
        currentScore = 0;
        highestScore = SavingSystem.Instance.Load().highScore;

        if (OnScoreUpdate != null)
        {
            OnScoreUpdate.Invoke(currentScore, highestScore);
        }
    }

    public void AddScore(int value)
    {
        if(value >= 0)
        {
            currentScore += value;

            if (currentScore > highestScore)
            {
                highestScore = currentScore;
                SaveHighScore();
            }

            if (OnScoreUpdate != null)
            {
                OnScoreUpdate.Invoke(currentScore, highestScore);
            }
        }
    }

    private void SaveHighScore()
    {
        SaveData data = SavingSystem.Instance.Load();
        data.highScore = highestScore;

        SavingSystem.Instance.Save(data);
    }

    public void ResetScore()
    {
        currentScore = 0;
        if (OnScoreUpdate != null)
        {
            OnScoreUpdate.Invoke(currentScore, highestScore);
        }
    }
}
