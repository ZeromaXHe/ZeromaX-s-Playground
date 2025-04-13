using Commons.Utils;
using Domains.Models.Entities.Civs;
using Domains.Repos.Civs;
using Godot;
using Infras.Base;

namespace Infras.Repos.Impl.Civs;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-08 16:55:26
public class CivRepo: Repository<Civ>, ICivRepo
{
    protected override void AddHook(Civ entity)
    {
    }

    protected override void DeleteHook(Civ entity)
    {
    }

    protected override void TruncateHook()
    {
    }


    public Civ Add(Color color) => Add(id => new Civ(color, ColorUtil.GetClosestName(color), id));
}