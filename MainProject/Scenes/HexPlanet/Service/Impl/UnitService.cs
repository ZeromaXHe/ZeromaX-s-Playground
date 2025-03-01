using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repo;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service.Impl;

public class UnitService(IUnitRepo unitRepo): IUnitService
{
    public Unit Add() => unitRepo.Add();
    public void Delete(int id) => unitRepo.Delete(id);
    public void Truncate() => unitRepo.Truncate();
}