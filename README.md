# Enhanced Debug

*Install using the [Mod Manager](https://github.com/Kirefel/bf-mod-manager)*

Currently adds 2 features while debug controls are on:

* Right click on the main map to teleport to the cursor position
* Frame step - press `.` to toggle and `/` to step one frame

## UnityExplorer integration

Integrates with [UnityExplorer](https://github.com/sinai-dev/UnityExplorer) to:

* Pause the game while the UnityExplorer UI is open
* Provide shortcuts to useful objects (`GameController`, `SeinCharacter`)

## Experimental frame step

The "High Accuracy Frame Step" setting changes frame step to suspend more code when paused. This should be more accurate but it hasn't been tested very much so it could result in strange behaviour.

Leave disabled to simulate pausing every frame.
