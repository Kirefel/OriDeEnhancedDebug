# Enhanced Debug

*Download the full package and extract into the `Ori DE` folder. This includes the mod loader and is intended work with an unmodified game.*

Currently adds 2 features while debug controls are on:

* Right click on the main map to teleport to the cursor position
* Frame step - press `.` to toggle and `/` to step one frame

Trigger area and map completion areas can be enabled in the settings.

## UnityExplorer integration

Integrates with [UnityExplorer](https://github.com/sinai-dev/UnityExplorer) to:

* Pause the game while the UnityExplorer UI is open

Download the BepInEx 5 version (`UnityExplorer.BepInEx5.Mono.zip`) and extract to `Ori DE/BepInEx/plugins/sinai-dev-UnityExplorer`

## Experimental frame step

The "High Accuracy Frame Step" setting changes frame step to suspend more code when paused. This should be more accurate but it hasn't been tested very much so it could result in strange behaviour.

Leave disabled to simulate pausing every frame.
