using OriDeModLoader.UIExtensions;

namespace EnhancedDebug
{
    public class EnhancedDebugSettingsScreen : CustomOptionsScreen
    {
        public override void InitScreen()
        {
            AddToggle(Settings.AutoEnable, "Auto Enable Debug Controls", "Whether debug controls should be enabled immediately when starting the game");
        }
    }
}