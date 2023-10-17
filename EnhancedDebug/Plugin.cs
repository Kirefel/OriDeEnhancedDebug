using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using OriModding.BF.Core;

namespace KFT.OriBF.EnhancedDebug;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    private Harmony harmony;

    public static ConfigEntry<bool> HighAccuracyFrameStep { get; set; }
    public static ConfigEntry<bool> AutoEnable { get; set; }

    public static new ManualLogSource Logger { get; private set; }

    void Awake()
    {
        Logger = base.Logger;

        HighAccuracyFrameStep = Config.Bind(
            "Debug",
            "High Accuracy Frame Step",
            false,
            "(Experimental) Whether an alternative frame step method should be used. More accurate but not thoroughly tested. Requires restart to change.");

        AutoEnable = Config.Bind(
            "Debug",
            "Auto Enable Debug Controls",
            true,
            "Whether debug controls should be enabled immediately when starting the game.");


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

        Controllers.Add<EnhancedDebugController>(group: "EnhancedDebug");
        Controllers.Add<FrameStepController>(group: "EnhancedDebug");
    }

    public void Unload()
    {
        harmony.UnpatchSelf();
    }
}
