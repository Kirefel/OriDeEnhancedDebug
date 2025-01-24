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
        if (!Plugin.DrawMapCompletionAreas.Value)
            return;

        // This is pretty inefficient so don't render cages for areas that are far away
        if (GameWorld.Instance.CurrentArea != RuntimeArea)
            return;

        GL.PushMatrix();
        GL.LoadProjectionMatrix(Camera.main.projectionMatrix);

        lineMaterial.SetPass(0);
        
        GL.Begin(GL.TRIANGLES);
        GL.Color(Plugin.Colour_MapFace.Value);

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
        GL.Begin(GL.LINES);
        GL.Color(Plugin.Colour_MapEdge.Value);

        foreach (var edge in Area.CageStructureTool.Edges)
        {
            var v1 = Area.CageStructureTool.Vertices[edge.VertexA].Position;
            var v2 = Area.CageStructureTool.Vertices[edge.VertexB].Position;
            GL.Vertex3(v1.x, v1.y, v1.z);
            GL.Vertex3(v2.x, v2.y, v2.z);
        }

        GL.End();
        GL.PopMatrix();
    }
}