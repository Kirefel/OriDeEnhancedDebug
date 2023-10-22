using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using OriModding.BF.Core;

namespace KFT.OriBF.EnhancedDebug;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInDependency(OriModding.BF.Core.PluginInfo.PLUGIN_GUID)]
[BepInDependency("com.sinai.unityexplorer", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(OriModding.BF.ConfigMenu.PluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
public class Plugin : BaseUnityPlugin
{
    private Harmony harmony;

    public static ConfigEntry<bool> HighAccuracyFrameStep { get; private set; }
    public static ConfigEntry<bool> AutoEnable { get; private set; }
    private ConfigEntry<bool> pauseOnUnityExplorerUI;

    public static new ManualLogSource Logger { get; private set; }

    public static bool UnityExplorerEnabled { get; private set; }

    void Awake()
    {
        Logger = base.Logger;

        HighAccuracyFrameStep = Config.Bind(
            "Debug",
            "High Accuracy Frame Step",
            false,
            "(Experimental) Whether an alternative frame step method should be used. More accurate but not thoroughly tested.");

        AutoEnable = Config.Bind(
            "Debug",
            "Auto Enable Debug Controls",
            true,
            "Whether debug controls should be enabled immediately when starting the game.");

        pauseOnUnityExplorerUI = Config.Bind(
            "Debug",
            "Pause When Using UnityExplorer",
            true,
            "Whether to pause the game while the UnityExplorer UI is visible.");

        On.PlayerInput.FixedUpdate += (orig, self) =>
        {
            if (!FrameStepController.Suspended)
                orig(self);
        };

        harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        if (HighAccuracyFrameStep.Value)
        {
            Logger.LogInfo("Enabling high accuracy frame step");
            FrameStepUpdateHooks.PatchAll(harmony);
        }

        HighAccuracyFrameStep.SettingChanged += (sender, e) =>
        {
            if (HighAccuracyFrameStep.Value)
            {
                Logger.LogInfo("Enabling high accuracy frame step");
                FrameStepUpdateHooks.PatchAll(harmony);
            }
            else
            {
                Logger.LogInfo("Disabling high accuracy frame step");
                harmony.UnpatchSelf();
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
        if (suspended && (!UnityExplorer.UI.UIManager.ShowMenu || !pauseOnUnityExplorerUI.Value))
        {
            SuspensionManager.ResumeAll();
            suspended = false;
        }
        else if (!suspended && UnityExplorer.UI.UIManager.ShowMenu && pauseOnUnityExplorerUI.Value)
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
