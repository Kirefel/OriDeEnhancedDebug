using BaseModLib;
using HarmonyLib;
using OriDeModLoader;
using OriDeModLoader.UIExtensions;

namespace EnhancedDebug
{
    public class EnhancedDebug : IMod
    {
        public string Name => "Enhanced Debug";

        private Harmony harmony;

        public void Init()
        {
            harmony = new Harmony("enhanced_debug");
            harmony.PatchAll();

            CustomMenuManager.RegisterOptionsScreen<EnhancedDebugSettingsScreen>("DEBUG", 100);

            Controllers.Add<EnhancedDebugController>(group: "EnhancedDebug");
            Controllers.Add<FrameStepController>(group: "EnhancedDebug");
        }

        public void Unload()
        {
            harmony.UnpatchAll("enhanced_debug");
        }
    }
}
