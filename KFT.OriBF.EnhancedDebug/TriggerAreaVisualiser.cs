using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace KFT.OriBF.EnhancedDebug;

[HarmonyPatch(typeof(PlayerCollisionTrigger))]
internal class PlayerCollisionTriggerAreaPatch
{
    [HarmonyPostfix, HarmonyPatch(nameof(PlayerCollisionTrigger.Awake))]
    public static void AddVisualiserComponent(PlayerCollisionTrigger __instance, Rect ___m_bounds)
    {
        var visualiser = __instance.gameObject.AddComponent<TriggerAreaVisualiser>();
        visualiser.SetBounds(___m_bounds);
        visualiser.colour = Color.green;
    }
}

[HarmonyPatch(typeof(PlayerCollisionStayTrigger))]
internal class PlayerCollisionStayTriggerAreaPatch
{
    [HarmonyPostfix, HarmonyPatch(nameof(PlayerCollisionStayTrigger.Awake))]
    public static void AddVisualiserComponent(PlayerCollisionStayTrigger __instance, Rect ___m_bounds)
    {
        var visualiser = __instance.gameObject.AddComponent<TriggerAreaVisualiser>();
        visualiser.SetBounds(___m_bounds);
        visualiser.colour = Color.magenta;
    }
}

public class TriggerAreaVisualiser : MonoBehaviour
{
    private static Material lineMaterial;
    private Vector3[] corners;  
    private string fullName;
    public Color colour;

    public void SetBounds(Rect bounds)
    {
        corners =
        [
            new Vector3(bounds.xMin, bounds.yMin),
            new Vector3(bounds.xMax, bounds.yMin),
            new Vector3(bounds.xMax, bounds.yMax),
            new Vector3(bounds.xMin, bounds.yMax)
        ];
    }

    private void Awake()
    {
        if (!lineMaterial)
        {
            lineMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
            lineMaterial.SetInt("_ZTest", 0);
        }

        fullName = GetName();
    }

    public void OnGUI()
    {
        if (!Plugin.DrawTriggerAreas.Value)
            return;
        
        var point = Camera.main.WorldToScreenPoint(corners[0]);
        point.y = Screen.height - point.y;
        var c = GUI.color;
        GUI.color = colour;
        GUI.Label(new Rect(point, new Vector2(400, 50)), fullName);
        GUI.color = c;
    }

    private string GetName()
    {
        List<string> names = new List<string>();
        var o = transform;
        while (o != null)
        {
            names.Add(o.name);
            o = o.parent;
        }

        names.Reverse();
        return string.Join("/", names.ToArray());
    }

    public void OnRenderObject()
    {
        if (!Plugin.DrawTriggerAreas.Value)
            return;
        
        GL.PushMatrix();
        GL.LoadProjectionMatrix(Camera.main.projectionMatrix);
        
        lineMaterial.SetPass(0);

        GL.Begin(GL.LINES);
        GL.Color(colour);

        for (int i = 0; i < corners.Length - 1; i++)
        {
            GL.Vertex3(corners[i].x, corners[i].y, corners[i].z);
            GL.Vertex3(corners[i + 1].x, corners[i + 1].y, corners[i + 1].z);
        }

        GL.Vertex3(corners[corners.Length - 1].x, corners[corners.Length - 1].y, corners[corners.Length - 1].z);
        GL.Vertex3(corners[0].x, corners[0].y, corners[0].z);

        GL.End();
        GL.PopMatrix();
    }
}