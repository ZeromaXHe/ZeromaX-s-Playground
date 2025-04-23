using Domains.Services.Abstractions.Events.Events;
using Infras.Readers.Abstractions.Nodes.Singletons.ChunkManagers;

namespace Apps.Commands.Nodes.Singletons.ChunkManagers;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:33:34
public class FeatureMeshManagerCommander
{
    private readonly IFeatureMeshManagerRepo _featureMeshManagerRepo;
    
    public FeatureMeshManagerCommander(IFeatureMeshManagerRepo featureMeshManagerRepo)
    {
        _featureMeshManagerRepo = featureMeshManagerRepo;
        _featureMeshManagerRepo.Ready += OnReady;
        _featureMeshManagerRepo.TreeExiting += OnTreeExiting;
    }

    public void ReleaseEvents()
    {
        _featureMeshManagerRepo.Ready -= OnReady;
        _featureMeshManagerRepo.TreeExiting -= OnTreeExiting;
    }

    private void OnReady()
    {
        _featureMeshManagerRepo.Singleton!.InitMultiMeshInstances();
        FeatureEvent.Instance.MeshShown += _featureMeshManagerRepo.OnShowFeature;
        FeatureEvent.Instance.MeshHidden += _featureMeshManagerRepo.OnHideFeature;
    }

    private void OnTreeExiting()
    {
        FeatureEvent.Instance.MeshShown -= _featureMeshManagerRepo.OnShowFeature;
        FeatureEvent.Instance.MeshHidden -= _featureMeshManagerRepo.OnHideFeature;
    }
}