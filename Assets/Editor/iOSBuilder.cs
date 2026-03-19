using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;

/// <summary>
/// Called via Unity CLI: -executeMethod iOSBuilder.Build
/// Generates an Xcode project in the "iOS app/" folder.
/// </summary>
public class iOSBuilder
{
    private static readonly string BuildPath = "iOS app";

    public static void Build()
    {
        Debug.Log("[iOSBuilder] Starting iOS build...");

        // Ensure output directory exists
        if (!Directory.Exists(BuildPath))
            Directory.CreateDirectory(BuildPath);

        // Gather all enabled scenes from Build Settings
        var scenes = GetEnabledScenes();
        if (scenes.Length == 0)
        {
            Debug.LogError("[iOSBuilder] No scenes found in Build Settings!");
            EditorApplication.Exit(1);
            return;
        }

        var options = new BuildPlayerOptions
        {
            scenes           = scenes,
            locationPathName = BuildPath,
            target           = BuildTarget.iOS,
            options          = BuildOptions.None
        };

        BuildReport  report  = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"[iOSBuilder] Build succeeded in {summary.totalTime.TotalSeconds:F1}s");
            EditorApplication.Exit(0);
        }
        else
        {
            Debug.LogError($"[iOSBuilder] Build FAILED: {summary.result}");
            EditorApplication.Exit(1);
        }
    }

    private static string[] GetEnabledScenes()
    {
        var scenes = new System.Collections.Generic.List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
                scenes.Add(scene.path);
        }
        return scenes.ToArray();
    }
}
