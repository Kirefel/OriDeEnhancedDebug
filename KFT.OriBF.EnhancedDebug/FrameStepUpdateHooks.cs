using HarmonyLib;
using OriModding.BF.Core;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace KFT.OriBF.EnhancedDebug;

public static class FrameStepUpdateHooks
{
    private static HashSet<Type> allowList = new HashSet<Type>() { typeof(PlayerInput) };

    public static void PatchAll(Harmony harmony)
    {
        HarmonyMethod prefixMethod = new HarmonyMethod(typeof(FrameStepUpdateHooks), nameof(Prefix));

        Type[] types = typeof(SeinController).Assembly.GetTypes();
        foreach (var type in types)
        {
            try
            {
                if (allowList.Contains(type) || typeof(ISuspendable).IsAssignableFrom(type))
                    continue;

                PatchMethod(harmony, type, "Update", prefixMethod);
                PatchMethod(harmony, type, "FixedUpdate", prefixMethod);
                PatchMethod(harmony, type, "LateUpdate", prefixMethod);
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogWarning("Failed to patch type " + type.Name);
                Plugin.Logger.LogError(ex);
            }
        }
    }

    private static void PatchMethod(Harmony harmony, Type type, string methodName, HarmonyMethod prefixMethod)
    {
        var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (method != null && !method.IsAbstract && method.DeclaringType == type)
            harmony.Patch(method, prefix: prefixMethod);
    }

    private static bool Prefix()
    {
        if (FrameStepController.Suspended)
            return HarmonyHelper.StopExecution;

        return HarmonyHelper.ContinueExecution;
    }
}
