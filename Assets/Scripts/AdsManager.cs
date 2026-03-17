using UnityEngine;

#if UNITY_ANDROID || UNITY_IOS
using GoogleMobileAds.Api;
#endif

public class AdsManager : MonoBehaviour
{
    public static AdsManager instance;

    // ── Ad IDs ────────────────────────────────────────────────────────────────
    // Set USE_TEST_ADS to false only when submitting a production build
    const bool USE_TEST_ADS = false;

#if UNITY_ANDROID
    const string realAdUnitId  = "ca-app-pub-8860686787222189/8642719108";
    const string testAdUnitId  = "ca-app-pub-3940256099942544/1033173712"; // Google official test
#elif UNITY_IOS
    const string realAdUnitId  = "ca-app-pub-8860686787222189/8306478073";
    const string testAdUnitId  = "ca-app-pub-3940256099942544/4411468910"; // Google official test
#else
    const string realAdUnitId  = "unused";
    const string testAdUnitId  = "unused";
#endif

    string interstitialAdUnitId => USE_TEST_ADS ? testAdUnitId : realAdUnitId;

    // Show an ad every N completed turns  (N = 2 means after turns 2, 4, 6 …)
    const int AdEveryNTurns = 2;

    // Persists across scene reloads because it is static
    static int turnCount = 0;

#if UNITY_ANDROID || UNITY_IOS
    InterstitialAd interstitialAd;
#endif

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    void Awake()
    {
        // Singleton — keep the first instance alive across scene reloads
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

#if UNITY_ANDROID || UNITY_IOS
        MobileAds.Initialize(status =>
        {
            Debug.Log("[Ads] SDK initialised.");
            LoadInterstitial();
        });
#endif
    }

    // ── Loading ───────────────────────────────────────────────────────────────

#if UNITY_ANDROID || UNITY_IOS
    void LoadInterstitial()
    {
        // Clean up previous instance first
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        InterstitialAd.Load(interstitialAdUnitId, new AdRequest(),
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogWarning("[Ads] Interstitial failed to load: " + error);
                    return;
                }

                interstitialAd = ad;
                RegisterAdEvents(interstitialAd);
                Debug.Log("[Ads] Interstitial loaded.");
            });
    }

    void RegisterAdEvents(InterstitialAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("[Ads] Interstitial closed — loading next ad.");
            LoadInterstitial();
        };

        ad.OnAdFullScreenContentFailed += adError =>
        {
            Debug.LogWarning("[Ads] Interstitial failed to show: " + adError);
            LoadInterstitial();
        };
    }
#endif

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Call this once every time a game-over occurs.
    /// Shows an interstitial every AdEveryNTurns turns.
    /// </summary>
    public void OnTurnComplete()
    {
        turnCount++;
        Debug.Log($"[Ads] Turn {turnCount} complete.");

        if (turnCount % AdEveryNTurns != 0) return;

#if UNITY_ANDROID || UNITY_IOS
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            Debug.Log("[Ads] Showing interstitial.");
            interstitialAd.Show();
        }
        else
        {
            Debug.LogWarning("[Ads] Interstitial not ready — reloading.");
            LoadInterstitial();
        }
#else
        Debug.Log("[Ads] Skipping ad (Editor / unsupported platform).");
#endif
    }
}
