namespace Domains.Models.Bases;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-24 13:35
public abstract class Entity(int id)
{
    public int Id { get; } = id;
}