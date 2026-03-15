using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    static public UIManager instance;

    public Text scoreText;
    public Text highScoreText;
    public Text themeButtonText;
    public GameObject Title;
    public Button playButton;
    public Button themeButton;

    int currentScore = 0;
    int highScore;

    private void Awake()
    {
        instance = this;
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        playButton.onClick.AddListener(OnStartPlay);

        if (themeButton != null)
            themeButton.onClick.AddListener(OnToggleTheme);

        UpdateThemeButtonText();
        SetHighScoreText(highScore.ToString());
    }

    public void OnStartPlay()
    {
        Title.SetActive(false);
        playButton.onClick.RemoveAllListeners();
        playButton.gameObject.SetActive(false);
        SetScoreText("0");
        GameManager.instance.isGameStarted = true;
    }

    void OnToggleTheme()
    {
        ThemeManager.instance.ToggleTheme();
        UpdateThemeButtonText();
    }

    void UpdateThemeButtonText()
    {
        if (themeButtonText != null && ThemeManager.instance != null)
            themeButtonText.text = ThemeManager.instance.GetThemeName();
    }

    public void SetScoreText(string score)
    {
        scoreText.text = score;
        currentScore = int.Parse(score);
    }

    public void SetHighScoreText(string score)
    {
        if (highScoreText != null)
            highScoreText.text = "BEST\n" + score;
    }

    public void OnGameOver()
    {
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        SetHighScoreText(highScore.ToString());

        // Count the completed turn and show interstitial every 2 turns
        if (AdsManager.instance != null)
            AdsManager.instance.OnTurnComplete();

        playButton.gameObject.SetActive(true);
        playButton.onClick.AddListener(GameManager.instance.Replay);
    }
}
