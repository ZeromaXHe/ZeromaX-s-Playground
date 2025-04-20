using Domains.Services.Abstractions.Events.Events;
using Domains.Services.Abstractions.Nodes.ChunkManagers;
using Infras.Readers.Abstractions.Nodes.Singletons.ChunkManagers;

namespace Apps.Commands.ChunkManagers;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:34:09
public class FeaturePreviewManagerCommander
{
    private readonly IFeaturePreviewManagerService _featurePreviewManagerService;
    private readonly IFeaturePreviewManagerRepo _featurePreviewManagerRepo;

    public FeaturePreviewManagerCommander(IFeaturePreviewManagerService featurePreviewManagerService,
        IFeaturePreviewManagerRepo featurePreviewManagerRepo)
    {
        _featurePreviewManagerService = featurePreviewManagerService;
        _featurePreviewManagerRepo = featurePreviewManagerRepo;
        _featurePreviewManagerRepo.Ready += OnReady;
        _featurePreviewManagerRepo.TreeExiting += OnTreeExiting;
    }

    private void OnReady()
    {
        FeatureEvent.Instance.PreviewShown += _featurePreviewManagerService.OnShowFeature;
        FeatureEvent.Instance.PreviewHidden += _featurePreviewManagerRepo.OnHideFeature;
    }

    private void OnTreeExiting()
    {
        FeatureEvent.Instance.PreviewShown -= _featurePreviewManagerService.OnShowFeature;
        FeatureEvent.Instance.PreviewHidden -= _featurePreviewManagerRepo.OnHideFeature;
    }
}