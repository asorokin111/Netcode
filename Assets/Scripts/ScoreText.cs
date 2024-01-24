using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    private TextMeshProUGUI _scoreText;
    private void Awake()
    {
        _scoreText = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        ScoreManager.OnScoresUpdated += UpdateScoreText;
    }

    private void OnDisable()
    {
        ScoreManager.OnScoresUpdated -= UpdateScoreText;
    }

    private void UpdateScoreText()
    {
        _scoreText.text = "";
        foreach (KeyValuePair<string, int> entry in ScoreManager.instance.scores)
        {
            _scoreText.text += "Player " + entry.Key + " - " + entry.Value + '\n';
        }
    }
}
