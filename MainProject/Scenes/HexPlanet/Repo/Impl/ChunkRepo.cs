using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repo.Impl;

public class ChunkRepo : Repository<Chunk>, IChunkRepo
{
    private readonly Dictionary<Vector3, int> _posIndex = new();
    public Chunk Add(Vector3 pos) => Add(id => new Chunk(pos, id));
    protected override void AddHook(Chunk entity) => _posIndex.Add(entity.Pos, entity.Id);
    protected override void TruncateHook() => _posIndex.Clear();
    public Chunk GetByPos(Vector3 position) => _posIndex.TryGetValue(position, out var id) ? GetById(id) : null;
}