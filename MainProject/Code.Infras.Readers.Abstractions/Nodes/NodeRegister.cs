using GodotNodes.Abstractions;
using Infras.Readers.Abstractions.Nodes.IdInstances;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Readers.Abstractions.Nodes.Singletons.ChunkManagers;
using Infras.Readers.Abstractions.Nodes.Singletons.LandGenerators;
using Infras.Readers.Abstractions.Nodes.Singletons.Planets;
using Nodes.Abstractions;
using Nodes.Abstractions.ChunkManagers;
using Nodes.Abstractions.LandGenerators;
using Nodes.Abstractions.Planets;

namespace Infras.Readers.Abstractions.Nodes;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 11:19:56
public class NodeRegister(
    // 单例
    IChunkLoaderRepo chunkLoaderRepo,
    IFeatureMeshManagerRepo featureMeshManagerRepo,
    IFeaturePreviewManagerRepo featurePreviewManagerRepo,
    IErosionLandGeneratorRepo erosionLandGeneratorRepo,
    IFractalNoiseLandGeneratorRepo fractalNoiseLandGeneratorRepo,
    IRealEarthLandGeneratorRepo realEarthLandGeneratorRepo,
    ICelestialMotionManagerRepo celestialMotionManagerRepo,
    ISelectTileViewerRepo selectTileViewerRepo,
    IUnitManagerRepo unitManagerRepo,
    IChunkManagerRepo chunkManagerRepo,
    IEditPreviewChunkRepo editPreviewChunkRepo,
    IHexMapGeneratorRepo hexMapGeneratorRepo,
    IHexPlanetHudRepo hexPlanetHudRepo,
    IHexPlanetManagerRepo hexPlanetManagerRepo,
    ILongitudeLatitudeRepo longitudeLatitudeRepo,
    IMiniMapManagerRepo miniMapManagerRepo,
    IOrbitCameraRepo orbitCameraRepo,
    // 多例
    IHexGridChunkRepo hexGridChunkRepo)
{
    public bool Register<T>(T node) where T : INode
    {
        return node switch
        {
            // 单例
            IChunkLoader chunkLoader => chunkLoaderRepo.Register(chunkLoader),
            IFeatureMeshManager featureMeshManager => featureMeshManagerRepo.Register(featureMeshManager),
            IFeaturePreviewManager featurePreviewManager => featurePreviewManagerRepo.Register(featurePreviewManager),
            IErosionLandGenerator erosionLandGenerator => erosionLandGeneratorRepo.Register(erosionLandGenerator),
            IFractalNoiseLandGenerator fractalNoiseLandGenerator =>
                fractalNoiseLandGeneratorRepo.Register(fractalNoiseLandGenerator),
            IRealEarthLandGenerator realEarthLandGenerator =>
                realEarthLandGeneratorRepo.Register(realEarthLandGenerator),
            ICelestialMotionManager celestialMotionManager =>
                celestialMotionManagerRepo.Register(celestialMotionManager),
            ISelectTileViewer selectTileViewer => selectTileViewerRepo.Register(selectTileViewer),
            IUnitManager unitManager => unitManagerRepo.Register(unitManager),
            IChunkManager chunkManager => chunkManagerRepo.Register(chunkManager),
            IEditPreviewChunk editPreviewChunk => editPreviewChunkRepo.Register(editPreviewChunk),
            IHexMapGenerator hexMapGenerator => hexMapGeneratorRepo.Register(hexMapGenerator),
            IHexPlanetHud hexPlanetHud => hexPlanetHudRepo.Register(hexPlanetHud),
            IHexPlanetManager hexPlanetManager => hexPlanetManagerRepo.Register(hexPlanetManager),
            ILongitudeLatitude longitudeLatitude => longitudeLatitudeRepo.Register(longitudeLatitude),
            IMiniMapManager miniMapManager => miniMapManagerRepo.Register(miniMapManager),
            IOrbitCamera orbitCamera => orbitCameraRepo.Register(orbitCamera),
            // 多例
            IHexGridChunk hexGridChunk => RegisterIdInstance(hexGridChunk, hexGridChunkRepo.Register),
            _ => throw new ArgumentException($"暂不支持的单例节点：{typeof(T).Name}")
        };
    }

    private static bool RegisterIdInstance<T>(T instance, Action<T> register) where T : INode
    {
        register.Invoke(instance);
        return false; // 多例永远不会发生覆盖
    }
}