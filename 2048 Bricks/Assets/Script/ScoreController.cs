using UnityEngine;

public class ScoreController : MonoBehaviour
{
    private int currentScore;
    private int highestScore;

    public delegate void ScoreUpdate(int currentScore, int highScore);
    public static ScoreUpdate OnScoreUpdate;

    private void OnEnable()
    {
        GameplayController.OnBlockMerge += AddScore;
    }

    private void Start()
    {
        currentScore = 0;
        highestScore = SavingSystem.Instance.Load().highScore;

        if (OnScoreUpdate != null)
        {
            OnScoreUpdate.Invoke(currentScore, highestScore);
        }
    }

    private void AddScore(int value)
    {
        currentScore += value;

        if(currentScore > highestScore)
        {
            highestScore = currentScore;

            SaveData data = SavingSystem.Instance.Load();
            data.highScore = highestScore;

            SavingSystem.Instance.Save(data);
        }

        if(OnScoreUpdate != null)
        {
            OnScoreUpdate.Invoke(currentScore, highestScore);
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
        if (OnScoreUpdate != null)
        {
            OnScoreUpdate.Invoke(currentScore, highestScore);
        }
    }

    private void OnDisable()
    {
        GameplayController.OnBlockMerge -= AddScore;
    }
}
