using System.Collections.Generic;
using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

public class Chunk(Vector3 pos, int id = -1)
{
    public int Id { get; } = id;
    public Vector3 Pos { get; } = pos;
    public List<int> TileIds { get; } = [];

    #region 数据查询

    private static readonly Dictionary<int, Chunk> Repo = new();
    private static readonly Dictionary<Vector3, int> PosIndex = new();

    public static void Truncate()
    {
        Repo.Clear();
        PosIndex.Clear();
    }

    public static Chunk Add(Vector3 pos)
    {
        var chunk = new Chunk(pos, Repo.Count);
        Repo.Add(chunk.Id, chunk);
        PosIndex.Add(pos, chunk.Id);
        return chunk;
    }

    public static Chunk GetById(int id) => Repo.GetValueOrDefault(id);

    public static Chunk GetByPos(Vector3 position) =>
        PosIndex.TryGetValue(position, out var id) ? GetById(id) : null;

    public static IEnumerable<Chunk> GetAll() => Repo.Values;
    public static int GetCount() => Repo.Count;

    #endregion
}