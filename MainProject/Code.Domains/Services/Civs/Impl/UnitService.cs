using Domains.Models.Entities.Civs;
using Domains.Repos.Civs;

namespace Domains.Services.Civs.Impl;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-01 12:45
public class UnitService(IUnitRepo unitRepo): IUnitService
{
    public Unit Add() => unitRepo.Add();
    public Unit? GetById(int id) => unitRepo.GetById(id);
    public IEnumerable<Unit> GetAll() => unitRepo.GetAll();
    public void Delete(int id) => unitRepo.Delete(id);
    public void Truncate() => unitRepo.Truncate();
}