using Infras.Readers.Abstractions.Nodes.Singletons.LandGenerators;
using Infras.Readers.Bases;
using Nodes.Abstractions.LandGenerators;

namespace Infras.Readers.Nodes.Singletons.LandGenerators;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 19:37:25
public class ErosionLandGeneratorRepo : SingletonNodeRepo<IErosionLandGenerator>, IErosionLandGeneratorRepo
{
}