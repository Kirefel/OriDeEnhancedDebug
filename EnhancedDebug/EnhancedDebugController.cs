using Game;
using UnityEngine;

namespace EnhancedDebug
{
    public class EnhancedDebugController : MonoBehaviour
    {
        private void Awake()
        {
            // Enable debug controls on game launch
            DebugMenuB.MakeDebugMenuExist();
            DebugMenuB.DebugControlsEnabled = true;
        }

        private void FixedUpdate()
        {
            RightClickMapTeleport();
        }

        private void OnGUI()
        {
            if (DebugMenuB.DebugControlsEnabled)
            {
                // TODO test this looks ok. May need label instead of box content.
                GUI.Box(new Rect(Screen.width - 200, 0, 200, 40), "DEBUG");
            }
        }

        private static void RightClickMapTeleport()
        {
            if (!DebugMenuB.DebugControlsEnabled || !AreaMapUI.Instance || !AreaMapUI.Instance.gameObject.activeInHierarchy)
                return;

            if (Core.Input.RightClick.OnPressed)
            {
                Vector2 cursorPosition = Core.Input.CursorPositionUI;
                Vector2 worldPosition = AreaMapUI.Instance.Navigation.MapToWorldPosition(cursorPosition);
                if (Characters.Sein != null)
                {
                    Characters.Sein.Position = worldPosition;
                    Characters.Sein.Position = worldPosition + new Vector2(0f, 0.5f);
                    UI.Cameras.Current.MoveCameraToTargetInstantly(true);
                    UI.Menu.HideMenuScreen(true);
                    return;
                }
            }
        }
    }
}
