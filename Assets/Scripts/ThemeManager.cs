using UnityEngine;

[DefaultExecutionOrder(-10)]
public class ThemeManager : MonoBehaviour
{
    public static ThemeManager instance;

    public GameSettingData originalTheme;
    public GameSettingData mandalaTheme;
    public GameSettingData starryNightTheme;

    int _selectedTheme;

    void Awake()
    {
        instance = this;
        _selectedTheme = PlayerPrefs.GetInt("SelectedTheme", 0);
    }

    public GameSettingData GetCurrentTheme()
    {
        switch (_selectedTheme)
        {
            case 1: return mandalaTheme;
            case 2: return starryNightTheme;
            default: return originalTheme;
        }
    }

    public void ToggleTheme()
    {
        _selectedTheme = (_selectedTheme + 1) % 3;
        PlayerPrefs.SetInt("SelectedTheme", _selectedTheme);
        PlayerPrefs.Save();
    }

    public string GetThemeName()
    {
        switch (_selectedTheme)
        {
            case 1: return "MANDALA";
            case 2: return "STARRY";
            default: return "ORIGINAL";
        }
    }
}
