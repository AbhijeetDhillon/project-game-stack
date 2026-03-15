// This Editor-only script runs automatically after an iOS build.
// It writes the AdMob App ID into the generated Xcode project's Info.plist
// so the Google Mobile Ads SDK can initialise on iOS.

#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public static class iOSPostBuild
{
    const string AdMobAppId = "ca-app-pub-8860686787222189~2077310758";

    [PostProcessBuild(100)]
    public static void OnPostProcessBuild(BuildTarget target, string buildPath)
    {
        if (target != BuildTarget.iOS) return;

        string plistPath = Path.Combine(buildPath, "Info.plist");
        var plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        // Required by Google Mobile Ads SDK
        plist.root.SetString("GADApplicationIdentifier", AdMobAppId);

        // Recommended: suppress partial SKAdNetwork warnings on iOS 15+
        plist.root.SetBoolean("SKAdNetworkItems", false);

        plist.WriteToFile(plistPath);
        UnityEngine.Debug.Log("[iOSPostBuild] GADApplicationIdentifier written to Info.plist.");
    }
}
#endif
