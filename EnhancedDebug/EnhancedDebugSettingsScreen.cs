using OriDeModLoader.UIExtensions;

namespace EnhancedDebug
{
    public class EnhancedDebugSettingsScreen : CustomOptionsScreen
    {
        public override void InitScreen()
        {
            AddToggle(Settings.AutoEnable, "Auto Enable Debug Controls", "Whether debug controls should be enabled immediately when starting the game.");
            AddToggle(Settings.HighAccuracyFrameStep, "High Accuracy Frame Step", "(Experimental) Whether an alternative frame step method should be used. More accurate but not thoroughly tested. Requires restart to change.");
        }
    }
}