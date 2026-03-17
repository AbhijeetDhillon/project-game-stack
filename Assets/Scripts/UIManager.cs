using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    static public UIManager instance;

    public Text scoreText;
    public Text highScoreText;
    public GameObject Title;
    public Button playButton;
    public GameObject themePanel;          // whole panel — hidden when game starts

    // One entry per theme (ORIGINAL=0, MANDALA=1, STARRY=2)
    public Button[] themeButtons;          // length 3 — Button components
    public Text[]   themeScoreTexts;       // length 3 — "BEST N" under each button

    int currentScore = 0;
    int[] themeHighScores = new int[3];
    static readonly string[] HighScoreKeys = { "HighScore_0", "HighScore_1", "HighScore_2" };

    private void Awake()
    {
        instance = this;

        // Load per-theme high scores
        for (int i = 0; i < 3; i++)
            themeHighScores[i] = PlayerPrefs.GetInt(HighScoreKeys[i], 0);

        playButton.onClick.AddListener(OnStartPlay);

        // Wire theme buttons (listener captures index by value)
        if (themeButtons != null)
        {
            for (int i = 0; i < themeButtons.Length; i++)
            {
                int idx = i;
                if (themeButtons[i] != null)
                    themeButtons[i].onClick.AddListener(() => OnSelectTheme(idx));
            }
        }

        RefreshThemeUI();
        UpdateCurrentHighScore();
    }

    // ── Play flow ──────────────────────────────────────────────────────────────

    public void OnStartPlay()
    {
        Title.SetActive(false);
        if (themePanel != null) themePanel.SetActive(false);
        playButton.onClick.RemoveAllListeners();
        playButton.gameObject.SetActive(false);
        SetScoreText("0");
        GameManager.instance.isGameStarted = true;
    }

    // ── Theme selection ────────────────────────────────────────────────────────

    void OnSelectTheme(int index)
    {
        if (ThemeManager.instance == null) return;
        if (ThemeManager.instance.currentThemeIndex == index) return; // already active

        ThemeManager.instance.SelectTheme(index);
        // Reload scene → GameManager.Awake picks up the new theme
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Highlight the active theme button; update per-theme score labels
    void RefreshThemeUI()
    {
        if (themeButtons == null || ThemeManager.instance == null) return;

        int selected = ThemeManager.instance.currentThemeIndex;
        for (int i = 0; i < themeButtons.Length; i++)
        {
            if (themeButtons[i] == null) continue;

            // Bright = selected, dim = unselected
            var img = themeButtons[i].GetComponent<Image>();
            if (img != null)
                img.color = (i == selected)
                    ? new Color(1f, 1f, 1f, 0.90f)
                    : new Color(1f, 1f, 1f, 0.55f);

            // Score label under this button
            if (themeScoreTexts != null && i < themeScoreTexts.Length && themeScoreTexts[i] != null)
                themeScoreTexts[i].text = "BEST " + themeHighScores[i];
        }
    }

    void UpdateCurrentHighScore()
    {
        int t = ThemeManager.instance != null ? ThemeManager.instance.currentThemeIndex : 0;
        SetHighScoreText(themeHighScores[t].ToString());
    }

    // ── Score helpers ──────────────────────────────────────────────────────────

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

    // ── Game over ──────────────────────────────────────────────────────────────

    public void OnGameOver()
    {
        int t = ThemeManager.instance != null ? ThemeManager.instance.currentThemeIndex : 0;

        if (currentScore > themeHighScores[t])
        {
            themeHighScores[t] = currentScore;
            PlayerPrefs.SetInt(HighScoreKeys[t], themeHighScores[t]);
            PlayerPrefs.Save();
        }

        SetHighScoreText(themeHighScores[t].ToString());

        if (AdsManager.instance != null)
            AdsManager.instance.OnTurnComplete();

        playButton.gameObject.SetActive(true);
        playButton.onClick.AddListener(GameManager.instance.Replay);
    }
}
