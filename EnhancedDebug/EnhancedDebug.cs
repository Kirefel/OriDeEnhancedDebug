using BaseModLib;
using HarmonyLib;
using OriDeModLoader;

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

            Controllers.Add<EnhancedDebugController>(group: "EnhancedDebug");
            Controllers.Add<FrameStepController>(group: "EnhancedDebug");
        }

        public void Unload()
        {
            harmony.UnpatchAll("enhanced_debug");
        }
    }
}
