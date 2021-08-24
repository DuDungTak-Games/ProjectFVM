using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using UnityEditor.Build.Reporting;

public partial class AutoBuild
{
    static string[] SCENES = FindEnabledEditorScenes();

    static readonly string APP_NAME = PlayerSettings.productName;
    static readonly string TARGET_DIR = "target";
    static string projectPath { get { return Application.dataPath.Replace("/Assets", "") + "/"; } }
    static string buildinfoPath { get { return Application.dataPath + "/Resources/buildinfo.txt"; } }
    static readonly string android_key_store_pwd_ = "ta130805";
    static readonly string android_key_alias_pwd_ = "ta130805";


    static void UpdateAndroidManifest(string name)
    {
        string path = Application.dataPath + string.Format("/Plugins/Android/AndroidManifest.{0}.xml", name);
        Debug.Log(path);
        string orignal = Application.dataPath + "/Plugins/Android/AndroidManifest.xml";
        try
        {
            System.IO.File.Copy(orignal, orignal + ".tmp", true);
        }
        catch (System.Exception e)
        {
        }
        System.IO.File.Copy(path, orignal, true);
        AssetDatabase.Refresh();
    }
    
    static void ToggleDefine(BuildTargetGroup target, string defineName, bool on)
    {
        string define = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
        bool defined = define.Contains(defineName);
        if (defined == false && on)
            PlayerSettings.SetScriptingDefineSymbolsForGroup(target, define + defineName);
        else if (defined == true && on == false)
            PlayerSettings.SetScriptingDefineSymbolsForGroup(target, define.Replace(defineName, ""));
    }

    static void UpdateBuildInfo()
    {
        string info = string.Format("V.{0}", PlayerSettings.bundleVersion);
        string path = buildinfoPath;
        System.IO.File.WriteAllText(path, info);

        AssetDatabase.Refresh();
    }

    [MenuItem("Custom/CI/Build Android")]
    static void UpdateAndPerformAndroidBuild()
    {
        string[] arguments = Environment.GetCommandLineArgs();
        if (arguments.Length > 8)
        {
            PlayerSettings.bundleVersion = arguments[7];
            PlayerSettings.Android.bundleVersionCode = int.Parse(arguments[8]);
        }
        UpdateBuildInfo();

        PerformAndroidBuild();
    }

    [MenuItem("Custom/CI/Build AOS App bundle(aab) ")]
    static void PerformAndroidAppBundle()
    {        
        string[] arguments = Environment.GetCommandLineArgs();
        if(arguments.Length > 8)
        {
            PlayerSettings.bundleVersion = arguments[7];
            PlayerSettings.Android.bundleVersionCode = int.Parse(arguments[8]);
        }
        UpdateBuildInfo();

        string targetName = APP_NAME + ".aab";
        string targetPath = TARGET_DIR + "/" + targetName;
        EditorUserBuildSettings.buildAppBundle = true;
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
        EditorUserBuildSettings.androidBuildType = AndroidBuildType.Release;

        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 |AndroidArchitecture.ARMv7;

        RemoveFiles(projectPath + TARGET_DIR, "*.aab");

        Console.WriteLine("Starts to build AAB :" + targetPath);
        GenericBuild(SCENES, targetPath, BuildTarget.Android, BuildOptions.None);
    }
    
    static void PerformAndroidBuild()
    {
        string targetName = APP_NAME + ".apk";
        string targetPath = TARGET_DIR + "/" + targetName;
        EditorUserBuildSettings.buildAppBundle = false;
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
        EditorUserBuildSettings.androidBuildType = AndroidBuildType.Release;

        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 |AndroidArchitecture.ARMv7;

        RemoveFiles(projectPath + TARGET_DIR, "*.apk");

        Console.WriteLine("Starts to build APK :" + targetPath);
        GenericBuild(SCENES, targetPath, BuildTarget.Android, BuildOptions.None);
    }

    static void RemoveFiles(string dir, string name)
    {
        string gpgFolder = dir;
        foreach (System.IO.FileInfo f in new System.IO.DirectoryInfo(gpgFolder).GetFiles(name))
        {
            f.Delete();
        }
        AssetDatabase.Refresh();
    }

    static void RemoveiOSGooglePlayGame()
    {
        #region GPGS
        RemoveFiles(Application.dataPath, "GPGS_Constants.cs");

        RemoveFiles(Application.dataPath + "/Plugins/iOS", "CustomWebViewApplication.*");
        RemoveFiles(Application.dataPath + "/Plugins/iOS", "GPGS*.*");

        try
        {
            System.IO.Directory.Delete(Application.dataPath + "/GooglePlayGames", true);
        }
        catch (Exception e)
        {
            Debug.Log("Autobuilder.cs removeUnnecessaryFiles() Exception : " + e.Message);
        }
        #endregion

        AssetDatabase.Refresh();
    }


    [MenuItem("Custom/CI/Build iOS")]
    static void PerformiOSBuild()
    {
        string[] arguments = Environment.GetCommandLineArgs();
        if (arguments.Length > 8)
        {
            PlayerSettings.bundleVersion = arguments[7];
            PlayerSettings.iOS.buildNumber = arguments[8];
        }

        UpdateBuildInfo();
        AssetDatabase.Refresh();

        string target_dir = "XcodeProject";

        string path = projectPath;

        BuildOptions opt = BuildOptions.None;

        GenericBuild(SCENES, path + target_dir, BuildTarget.iOS, opt);
    }

    private static string[] FindEnabledEditorScenes()
    {
        List<string> EditorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled) continue;
            EditorScenes.Add(scene.path);
        }
        return EditorScenes.ToArray();
    }

    static void GenericBuild(string[] scenes, string target_dir, BuildTarget build_target, BuildOptions build_options)
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        if(build_target == BuildTarget.Android )
        {
            PlayerSettings.Android.keystorePass = android_key_store_pwd_;
            PlayerSettings.Android.keyaliasPass = android_key_alias_pwd_;

            //buildPlayerOptions.targetGroup = BuildTargetGroup.Android;
        }
        else if ( build_target == BuildTarget.iOS)
        {
            //buildPlayerOptions.targetGroup = BuildTargetGroup.iOS;
        }

        buildPlayerOptions.scenes = scenes;
        buildPlayerOptions.locationPathName = target_dir;
        buildPlayerOptions.target = build_target;
        buildPlayerOptions.options = build_options;

        var report = BuildPipeline.BuildPlayer(buildPlayerOptions);


        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log("GenericBuild returns Succeeded!");
        }
        else if (report.summary.result == BuildResult.Failed)
        {
            throw new Exception("BuildPlayer failure totalSize : " + report.summary.totalSize);
        }
        else if (report.summary.result == BuildResult.Cancelled)
        {
            Debug.Log("GenericBuild returns Cancelled!");
        }
        else
        {
            Debug.LogError("Unknown Erros");
        }
        Console.WriteLine("Completed building :" + target_dir );
    }

    [MenuItem("PlayerData/Clear")]
    static void PlayerDataClear()
    {
        PlayerPrefs.DeleteAll();
    }
}
