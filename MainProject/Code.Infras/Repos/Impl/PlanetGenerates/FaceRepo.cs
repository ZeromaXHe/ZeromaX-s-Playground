using Commons.Utils;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Repos.PlanetGenerates;
using Godot;
using Infras.Base;

namespace Infras.Repos.Impl.PlanetGenerates;

public class FaceRepo : Repository<Face>, IFaceRepo
{
    public Face Add(bool chunky, Vector3[] triVertices) =>
        Add(id =>
        {
            var center = (triVertices[0] + triVertices[1] + triVertices[2]) / 3f;
            return new Face(chunky, center, id) { TriVertices = triVertices };
        });

    public IEnumerable<Face> GetAllByChunky(bool chunky) => GetAll().Where(x => x.Chunky == chunky);

    protected override void AddHook(Face entity)
    {
    }

    protected override void TruncateHook()
    {
    }
    
    public List<Face> GetOrderedFaces(Point center)
    {
        var faces = center.FaceIds.Select(id => GetById(id)!).ToList();
        if (faces.Count == 0) return faces;
        // 将第一个面设置为最接近北方顺时针方向第一个的面
        var first = faces[0];
        var minAngle = Mathf.Tau;
        foreach (var face in faces)
        {
            var angle = center.Position.DirectionTo(face.Center).AngleTo(Vector3.Up);
            if (angle < minAngle)
            {
                minAngle = angle;
                first = face;
            }
        }

        // 第二个面必须保证和第一个面形成顺时针方向，从而保证所有都是顺时针
        var second =
            faces.First(face =>
                face.Id != first.Id
                && face.IsAdjacentTo(first)
                && Math3dUtil.IsRightVSeq(Vector3.Zero, center.Position, first.Center, face.Center));
        var orderedList = new List<Face> { first, second };
        var currentFace = orderedList[1];
        while (orderedList.Count < faces.Count)
        {
            var existingIds = orderedList.Select(face => face.Id).ToList();
            var neighbour = faces.First(face =>
                !existingIds.Contains(face.Id) && face.IsAdjacentTo(currentFace));
            currentFace = neighbour;
            orderedList.Add(currentFace);
        }

        return orderedList;
    }
}