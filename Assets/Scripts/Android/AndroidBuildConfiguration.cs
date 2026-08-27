#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;

namespace SpaceShooter.Android
{
    public static class AndroidBuildConfiguration
    {
        public const string PACKAGE_ID = "com.novastorm.spaceshooter";
        public const string APP_NAME = "Space Shooter: Nova Storm";
        public const string VERSION_NAME = "1.0.0";
        public const int BUNDLE_VERSION_CODE = 100;

        [MenuItem("SpaceShooter/Android/Configure Android Production Settings")]
        public static void ConfigurePlayerSettings()
        {
            PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Android, PACKAGE_ID);
            PlayerSettings.productName = APP_NAME;
            PlayerSettings.bundleVersion = VERSION_NAME;
            PlayerSettings.Android.bundleVersionCode = BUNDLE_VERSION_CODE;

            // Target API & Architecture
            PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26; // Android 8.0 Oreo
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto; // Target API 34+

            // Orientation: Landscape Primary
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;

            // Rendering & Performance
            PlayerSettings.colorSpace = ColorSpace.Linear;
            PlayerSettings.Android.blitType = AndroidBlitType.Always;
            PlayerSettings.Android.renderOutsideSafeArea = true;

            Debug.Log($"[AndroidBuildConfiguration] Successfully configured {PACKAGE_ID} v{VERSION_NAME} (Code: {BUNDLE_VERSION_CODE}) for Android IL2CPP ARM64.");
        }

        [MenuItem("SpaceShooter/Android/Build Development APK")]
        public static void BuildDevelopmentApk()
        {
            ConfigurePlayerSettings();
            EditorUserBuildSettings.buildAppBundle = false;
            EditorUserBuildSettings.development = true;
            EditorUserBuildSettings.connectProfiler = false;

            string outDir = Path.Combine(Application.dataPath, "../Builds/Android");
            Directory.CreateDirectory(outDir);
            string buildPath = Path.Combine(outDir, $"SpaceShooter_Dev_v{VERSION_NAME}.apk");

            string[] scenes = { "Assets/Scenes/Prototype_SpaceCombat.unity" };
            BuildPlayerOptions opt = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = buildPath,
                target = BuildTarget.Android,
                options = BuildOptions.Development | BuildOptions.AllowDebugging
            };

            Debug.Log($"[AndroidBuildConfiguration] Starting Development APK build to: {buildPath}");
            BuildReport report = BuildPipeline.BuildPlayer(opt);
            Debug.Log($"[AndroidBuildConfiguration] Build result: {report.summary.result} (Total Size: {report.summary.totalSize} bytes)");
        }

        [MenuItem("SpaceShooter/Android/Build Release AAB (Google Play)")]
        public static void BuildReleaseAab()
        {
            ConfigurePlayerSettings();
            EditorUserBuildSettings.buildAppBundle = true;
            EditorUserBuildSettings.development = false;

            string outDir = Path.Combine(Application.dataPath, "../Builds/Android");
            Directory.CreateDirectory(outDir);
            string buildPath = Path.Combine(outDir, $"SpaceShooter_Release_v{VERSION_NAME}_{BUNDLE_VERSION_CODE}.aab");

            string[] scenes = { "Assets/Scenes/Prototype_SpaceCombat.unity" };
            BuildPlayerOptions opt = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = buildPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            Debug.Log($"[AndroidBuildConfiguration] Starting Release AAB build to: {buildPath}");
            BuildReport report = BuildPipeline.BuildPlayer(opt);
            Debug.Log($"[AndroidBuildConfiguration] Release AAB Build result: {report.summary.result}");
        }
    }
}
#endif
