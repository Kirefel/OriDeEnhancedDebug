﻿using BaseModLib;

namespace EnhancedDebug
{
    public class Settings
    {
        public static BoolSetting AutoEnable = new BoolSetting("enhancedDebugAutoEnable", false);
        public static BoolSetting HighAccuracyFrameStep = new BoolSetting("enhancedDebugHighAccuracyFrameStep", false);
    }
}
