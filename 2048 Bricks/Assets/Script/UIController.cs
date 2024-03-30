using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    private void OnEnable()
    {
        ScoreController.OnScoreUpdate += UpdateScore;
    }

    private void UpdateScore(int currentScore, int highScore)
    {
        scoreText.text = "Score : " + currentScore.ToString();
        highScoreText.text = "High Score : " + highScore.ToString();
    }

    private void OnDisable()
    {
        ScoreController.OnScoreUpdate -= UpdateScore;
    }
}
