﻿using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using OriModding.BF.Core;
using UnityEngine;
using UnityExplorer.UI;

namespace KFT.OriBF.EnhancedDebug;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInDependency(OriModding.BF.Core.PluginInfo.PLUGIN_GUID)]
[BepInDependency("com.sinai.unityexplorer", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(OriModding.BF.ConfigMenu.PluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
public class Plugin : BaseUnityPlugin
{
    private Harmony harmony;
    private Harmony harmonyFs;

    public static ConfigEntry<bool> HighAccuracyFrameStep { get; private set; }
    public static ConfigEntry<bool> AutoEnable { get; private set; }
    public static ConfigEntry<bool> DrawTriggerAreas { get; private set; }
    public static ConfigEntry<bool> DrawMapCompletionAreas { get; private set; }
    private ConfigEntry<bool> pauseOnUnityExplorerUI;
    
    public static ConfigEntry<Color> Colour_MapFace { get; private set; }
    public static ConfigEntry<Color> Colour_MapEdge { get; private set; }
    public static ConfigEntry<Color> Colour_Trigger { get; private set; }
    public static ConfigEntry<Color> Colour_StayTrigger { get; private set; }

    public static new ManualLogSource Logger { get; private set; }

    public static bool UnityExplorerEnabled { get; private set; }

    void Awake()
    {
        Logger = base.Logger;

        HighAccuracyFrameStep = Config.Bind("Debug", "High Accuracy Frame Step", false, "(Experimental) Whether an alternative frame step method should be used. More accurate but not thoroughly tested.");
        AutoEnable = Config.Bind("Debug", "Auto Enable Debug Controls", true, "Whether debug controls should be enabled immediately when starting the game.");
        DrawTriggerAreas = Config.Bind("Debug", "Draw trigger areas", false, "Whether to draw boxes around trigger areas such as spawn triggers.");
        DrawMapCompletionAreas = Config.Bind("Debug", "Draw map completion areas", false, "Whether to draw the zones that grant map completion when visited.");

        pauseOnUnityExplorerUI = Config.Bind("Debug", "Pause When Using UnityExplorer", true, "Whether to pause the game while the UnityExplorer UI is visible.");

        Colour_MapFace = Config.Bind("Debug", "Map Face Colour", new Color(Color.yellow.r,Color.yellow.g,Color.yellow.b, 0.15f), "The colour to fill cells in for the map completion visualisation.");
        Colour_MapEdge = Config.Bind("Debug", "Map Edge Colour", Color.yellow, "The colour to draw edges for the map completion visualisation.");
        Colour_Trigger = Config.Bind("Debug", "Trigger Colour", Color.green, "The colour to draw PlayerCollisionTrigger.");
        Colour_StayTrigger = Config.Bind("Debug", "Stay Trigger Colour", Color.magenta, "The colour to draw PlayerCollisionStayTrigger.");

        On.PlayerInput.FixedUpdate += (orig, self) =>
        {
            if (!FrameStepController.Suspended)
                orig(self);
        };

        harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        harmony.PatchAll();
        
        harmonyFs = new Harmony(PluginInfo.PLUGIN_GUID + "fs");
        if (HighAccuracyFrameStep.Value)
        {
            Logger.LogInfo("Enabling high accuracy frame step");
            FrameStepUpdateHooks.PatchAll(harmonyFs);
        }

        HighAccuracyFrameStep.SettingChanged += (sender, e) =>
        {
            if (HighAccuracyFrameStep.Value)
            {
                Logger.LogInfo("Enabling high accuracy frame step");
                FrameStepUpdateHooks.PatchAll(harmonyFs);
            }
            else
            {
                Logger.LogInfo("Disabling high accuracy frame step");
                harmonyFs.UnpatchSelf();
            }
        };

        Controllers.Add<EnhancedDebugController>(group: "EnhancedDebug");
        Controllers.Add<FrameStepController>(group: "EnhancedDebug");

        UnityExplorerEnabled = this.IsPluginLoaded("com.sinai.unityexplorer");

        if (!UnityExplorerEnabled && this.TryGetPlugin(OriModding.BF.ConfigMenu.PluginInfo.PLUGIN_GUID, out var configMenuPlugin))
        {
            var configMenu = configMenuPlugin as OriModding.BF.ConfigMenu.Plugin;
            configMenu.Hide(pauseOnUnityExplorerUI);
        }
    }

    bool suspended = false;

    void FixedUpdate()
    {
        if (!UnityExplorerEnabled)
            return;

        HandleUnityExplorer();
    }

    private void HandleUnityExplorer()
    {
        // This needs to be called from a different method otherwise it will get compiled and error if the plugin isn't loaded
        if (suspended && (!UIManager.ShowMenu || !pauseOnUnityExplorerUI.Value))
        {
            SuspensionManager.ResumeAll();
            suspended = false;
        }
        else if (!suspended && UIManager.ShowMenu && pauseOnUnityExplorerUI.Value)
        {
            SuspensionManager.SuspendAll();
            suspended = true;
        }
    }

    void OnDestroy()
    {
        harmony?.UnpatchSelf();
    }
}