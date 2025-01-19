using HarmonyLib;
using UnityEngine;

namespace KFT.OriBF.EnhancedDebug;

[HarmonyPatch(typeof(GameWorld))]
internal class GameWorldAreaPatch
{
    [HarmonyPostfix, HarmonyPatch(nameof(GameWorld.Awake))]
    public static void AttachMapCageRenderer(GameWorld __instance)
    {
        foreach (var area in __instance.Areas)
        {
            var drawer = GameController.Instance.gameObject.AddComponent<MapCageRenderer>();
            drawer.Area = area;
            drawer.RuntimeArea = __instance.FindRuntimeArea(area);
            drawer.isHollowGrove = drawer.Area.AreaIdentifier == "hollowGrove";
        }
    }
}

public class MapCageRenderer : MonoBehaviour
{
    private static Material lineMaterial;
    public GameWorldArea Area;
    public RuntimeGameWorldArea RuntimeArea;
    public bool isHollowGrove;

    private void Awake()
    {
        if (!lineMaterial)
        {
            lineMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
            lineMaterial.SetInt("_ZTest", 0);
            lineMaterial.SetInt("_Cull", 0);
        }
    }

    public void OnRenderObject()
    {
        if (!Plugin.DrawTriggerAreas.Value)
            return;

        // This is pretty inefficient so don't render cages for areas that are far away
        if (GameWorld.Instance.CurrentArea != RuntimeArea)
            return;

        GL.PushMatrix();
        GL.LoadProjectionMatrix(Camera.main.projectionMatrix);

        lineMaterial.SetPass(0);

        // GL.
        GL.Begin(GL.TRIANGLES);
        var c = Color.yellow;
        c.a = 0.5f;
        GL.Color(c);

        foreach (var face in Area.CageStructureTool.Faces)
        {
            if (isHollowGrove && face.ID == 1259)
            {
                // For some reason this face is massive and gets in the way of everything else,
                // so don't draw it
                continue;
            }

            if (RuntimeArea.GetFaceState(face.ID) == WorldMapAreaState.Visited)
                continue;


            for (int i = 0; i < face.Triangles.Count; i++)
            {
                var v = Area.CageStructureTool.Vertices[face.Vertices[face.Triangles[i]]];
                GL.Vertex3(v.Position.x, v.Position.y, v.Position.z);
            }
        }

        GL.End();
        GL.PopMatrix();
    }
}