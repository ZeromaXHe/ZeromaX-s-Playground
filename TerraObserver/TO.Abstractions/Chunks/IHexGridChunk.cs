namespace TO.Abstractions.Chunks;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-08 13:41:08
public interface IHexGridChunk : IChunk
{
    event Action<IHexGridChunk> Processed;
    int Id { get; set; }
    void HideOutOfSight();
    void ApplyNewData();
    void ClearOldData();
}