using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node.Interface;

public interface IHexMesh
{
    bool UseCollider { get; set; }
    bool UseColor { get; set; }
    bool UseUvCoordinates { get; set; }
    SurfaceTool SurfaceTool { get; set; }
    int VIdx { get; set; }
    void AddTriangle(Vector3[] vs, Color[] cs = null, Vector2[] uvs = null, Vector2[] uvs2 = null);
    void AddTriangleUnperturbed(Vector3[] vs, Color[] cs = null, Vector2[] uvs = null, Vector2[] uvs2 = null);
    void AddQuad(Vector3[] vs, Color[] cs = null, Vector2[] uvs = null, Vector2[] uvs2 = null);
    void AddQuadUnperturbed(Vector3[] vs, Color[] cs = null, Vector2[] uvs = null, Vector2[] uvs2 = null);
}