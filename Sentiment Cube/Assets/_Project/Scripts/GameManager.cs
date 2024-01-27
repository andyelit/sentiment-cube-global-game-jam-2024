using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI logText;
    public Text coinsText;
    public APICallExample m_API;
    public GameObject playButton;
    public GameObject playerAnswerOptions;
    public int score = 0;
    public int bestScore = 0;
    public int scoreModifier = 20;

    public string playerAnswer = string.Empty;
    private bool isPlayerGaveTheAnswer = false;

    [Header("Events")]
    public UnityEvent OnLevelReset;
    public UnityEvent OnCorrectGuess;
    public UnityEvent OnWrongGuess;

    private void Start()
    {
        Application.targetFrameRate = 60;

        score = PlayerPrefs.GetInt("BestScore", 0);
        coinsText.text = $"{score}";
    }

    public void PlayLevel()
    {
        StartCoroutine(LevelRoutine());
    }

    private IEnumerator LevelRoutine()
    {
        playButton.gameObject.SetActive(false);

        isPlayerGaveTheAnswer = false;

        logText.text = "Guessing..?";

        yield return new WaitForSeconds(1);

        string randomWord = string.Empty;
        yield return StartCoroutine(m_API.GetRandomWord((result) =>
        {
            randomWord = JsonUtility.FromJson<APICallExample.RandomWordReponse>(result).word;
        }));

        logText.text = randomWord;

        yield return new WaitForSeconds(1);

        APICallExample.SentimentResponse sentiment = new APICallExample.SentimentResponse();
        yield return StartCoroutine(m_API.GetSentiment(randomWord, (result) =>
        {
            sentiment = JsonUtility.FromJson<APICallExample.SentimentResponse>(result);
        }));

        if (Application.isEditor)
            logText.text = sentiment.sentiment;

        // Take Answer from user
        playerAnswerOptions.gameObject.SetActive(true);
        playerAnswerOptions.GetComponent<DOTweenAnimation>().DORestart();
        yield return new WaitUntil(() => isPlayerGaveTheAnswer);

        // Check positive or negative or neutral
        if (playerAnswer == sentiment.sentiment)
        {
            OnCorrectGuess?.Invoke();
            IncreamentScore();
        }
        else
            OnWrongGuess?.Invoke();

        StartCoroutine(ResetLevelRoutine());
    }

    private IEnumerator ResetLevelRoutine()
    {
        playerAnswerOptions.gameObject.SetActive(false);

        logText.text = $"Next Level in {3}s..";
        yield return new WaitForSeconds(1);
        logText.text = $"Next Level in {2}s..";
        yield return new WaitForSeconds(1);
        logText.text = $"Next Level in {1}s..";
        yield return new WaitForSeconds(1);
        logText.text = $"Next Level in {0}s..";

        isPlayerGaveTheAnswer = false;
        logText.text = "Make Me Happy!";
        playButton.gameObject.SetActive(true);
        playButton.GetComponent<DOTweenAnimation>().DORestart();

        OnLevelReset?.Invoke();
    }

    public void SetPlayerAnswer(string ans)
    {
        playerAnswer = ans;
        isPlayerGaveTheAnswer = true;
    }

    public void IncreamentScore()
    {
        score += scoreModifier;
        coinsText.text = $"{score}";

        if (score > bestScore)
        {
            PlayerPrefs.SetInt("BestScore", score);
        }
    }
}
