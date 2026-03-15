using UnityEngine;

[DefaultExecutionOrder(-10)]
public class ThemeManager : MonoBehaviour
{
    public static ThemeManager instance;

    public GameSettingData originalTheme;
    public GameSettingData mandalaTheme;
    public GameSettingData starryNightTheme;

    int _selectedTheme;

    // Read-only access to current index for UIManager / UIScore display
    public int currentThemeIndex => _selectedTheme;

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

    // Direct selection — called when a theme button is tapped
    public void SelectTheme(int index)
    {
        _selectedTheme = Mathf.Clamp(index, 0, 2);
        PlayerPrefs.SetInt("SelectedTheme", _selectedTheme);
        PlayerPrefs.Save();
    }

    // Kept for any legacy callers
    public void ToggleTheme() => SelectTheme((_selectedTheme + 1) % 3);

    public string GetThemeName()  => GetThemeName(_selectedTheme);

    public static string GetThemeName(int index)
    {
        switch (index)
        {
            case 1: return "MANDALA";
            case 2: return "STARRY";
            default: return "ORIGINAL";
        }
    }
}
