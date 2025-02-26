using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Enum;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node.Interface;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Struct;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service.Impl;

public class WallMeshService(ITileService tileService) : IWallMeshService
{
    public void AddWall(IHexFeatureManager feature, EdgeVertices near, Tile nearTile, EdgeVertices far, Tile farTile,
        bool hasRiver, bool hasRoad)
    {
        if (nearTile.Walled == farTile.Walled
            || nearTile.IsUnderwater || farTile.IsUnderwater
            || nearTile.GetEdgeType(farTile) == HexEdgeType.Cliff)
            return;
        AddWallSegment(feature, near.V1, far.V1, near.V2, far.V2);
        if (hasRiver || hasRoad)
        {
            AddWallCap(feature.Walls, near.V2, far.V2);
            AddWallCap(feature.Walls, far.V4, near.V4);
        }
        else
        {
            AddWallSegment(feature, near.V2, far.V2, near.V3, far.V3);
            AddWallSegment(feature, near.V3, far.V3, near.V4, far.V4);
        }

        AddWallSegment(feature, near.V4, far.V4, near.V5, far.V5);
    }

    public void AddWall(IHexFeatureManager feature, Vector3 c1, Tile tile1, Vector3 c2, Tile tile2, Vector3 c3,
        Tile tile3)
    {
        if (tile1.Walled)
        {
            if (tile2.Walled)
            {
                if (!tile3.Walled)
                    AddWallSegment(feature, c3, tile3, c1, tile1, c2, tile2);
            }
            else if (tile3.Walled)
                AddWallSegment(feature, c2, tile2, c3, tile3, c1, tile1);
            else
                AddWallSegment(feature, c1, tile1, c2, tile2, c3, tile3);
        }
        else if (tile2.Walled)
            if (tile3.Walled)
                AddWallSegment(feature, c1, tile1, c2, tile2, c3, tile3);
            else
                AddWallSegment(feature, c2, tile2, c3, tile3, c1, tile1);
        else if (tile3.Walled)
            AddWallSegment(feature, c3, tile3, c1, tile1, c2, tile2);
    }

    private void AddWallSegment(IHexFeatureManager feature, Vector3 nearLeft, Vector3 farLeft,
        Vector3 nearRight, Vector3 farRight, bool addTower = false)
    {
        nearLeft = HexMetrics.Perturb(nearLeft);
        farLeft = HexMetrics.Perturb(farLeft);
        nearRight = HexMetrics.Perturb(nearRight);
        farRight = HexMetrics.Perturb(farRight);
        var height = tileService.GetWallHeight();
        var thickness = tileService.GetWallThickness();
        var left = HexMetrics.WallLerp(nearLeft, farLeft, tileService.UnitHeight);
        var right = HexMetrics.WallLerp(nearRight, farRight, tileService.UnitHeight);
        var leftTop = left.Length() + height;
        var rightTop = right.Length() + height;
        Vector3 v1, v2, v3, v4;
        v1 = v3 = HexMetrics.WallThicknessOffset(nearLeft, farLeft, true, thickness);
        v2 = v4 = HexMetrics.WallThicknessOffset(nearRight, farRight, true, thickness);
        v3 = Math3dUtil.ProjectToSphere(v3, leftTop);
        v4 = Math3dUtil.ProjectToSphere(v4, rightTop);
        feature.Walls.AddQuadUnperturbed([v1, v2, v3, v4]);
        Vector3 t1 = v3, t2 = v4;
        v1 = v3 = HexMetrics.WallThicknessOffset(nearLeft, farLeft, false, thickness);
        v2 = v4 = HexMetrics.WallThicknessOffset(nearRight, farRight, false, thickness);
        v3 = Math3dUtil.ProjectToSphere(v3, leftTop);
        v4 = Math3dUtil.ProjectToSphere(v4, rightTop);
        feature.Walls.AddQuadUnperturbed([v2, v1, v4, v3]);
        feature.Walls.AddQuadUnperturbed([t1, t2, v3, v4]);

        if (addTower)
            feature.AddTower(left, right);
    }

    // pivot 有墙，left\right 没有墙的情况
    private void AddWallSegment(IHexFeatureManager feature, Vector3 pivot, Tile pivotTile,
        Vector3 left, Tile leftTile, Vector3 right, Tile rightTile)
    {
        if (pivotTile.IsUnderwater) return;
        var hasLeftWall = !leftTile.IsUnderwater && pivotTile.GetEdgeType(leftTile) != HexEdgeType.Cliff;
        var hasRightWall = !rightTile.IsUnderwater && pivotTile.GetEdgeType(rightTile) != HexEdgeType.Cliff;
        if (hasLeftWall)
        {
            if (hasRightWall)
            {
                var hasTower = false;
                if (leftTile.Elevation == rightTile.Elevation)
                {
                    var hash = HexMetrics.SampleHashGrid((pivot + left + right) / 3f);
                    hasTower = hash.E < HexMetrics.WallTowerThreshold;
                }
                AddWallSegment(feature, pivot, left, pivot, right, hasTower);
            }
            else if (leftTile.Elevation < rightTile.Elevation)
                AddWallWedge(feature.Walls, pivot, left, right);
            else
                AddWallCap(feature.Walls, pivot, left);
        }
        else if (hasRightWall)
        {
            if (rightTile.Elevation < leftTile.Elevation)
                AddWallWedge(feature.Walls, right, pivot, left);
            else
                AddWallCap(feature.Walls, right, pivot);
        }
    }

    private void AddWallCap(IHexMesh walls, Vector3 near, Vector3 far)
    {
        near = HexMetrics.Perturb(near);
        far = HexMetrics.Perturb(far);
        var center = HexMetrics.WallLerp(near, far, tileService.UnitHeight);
        var thickness = tileService.GetWallThickness();
        var height = tileService.GetWallHeight();
        var centerTop = center.Length() + height;
        Vector3 v1, v2, v3, v4;
        v1 = v3 = HexMetrics.WallThicknessOffset(near, far, true, thickness);
        v2 = v4 = HexMetrics.WallThicknessOffset(near, far, false, thickness);
        v3 = Math3dUtil.ProjectToSphere(v3, centerTop);
        v4 = Math3dUtil.ProjectToSphere(v4, centerTop);
        walls.AddQuadUnperturbed([v1, v2, v3, v4]);
    }

    private void AddWallWedge(IHexMesh walls, Vector3 near, Vector3 far, Vector3 point)
    {
        near = HexMetrics.Perturb(near);
        far = HexMetrics.Perturb(far);
        point = HexMetrics.Perturb(point);
        var center = HexMetrics.WallLerp(near, far, tileService.UnitHeight);
        var thickness = tileService.GetWallThickness();
        var height = tileService.GetWallHeight();
        var centerTop = center.Length() + height;
        point = Math3dUtil.ProjectToSphere(point, center.Length());
        var pointTop = Math3dUtil.ProjectToSphere(point, centerTop);
        Vector3 v1, v2, v3, v4;
        v1 = v3 = HexMetrics.WallThicknessOffset(near, far, true, thickness);
        v2 = v4 = HexMetrics.WallThicknessOffset(near, far, false, thickness);
        v3 = Math3dUtil.ProjectToSphere(v3, centerTop);
        v4 = Math3dUtil.ProjectToSphere(v4, centerTop);
        walls.AddQuadUnperturbed([v1, point, v3, pointTop]);
        walls.AddQuadUnperturbed([point, v2, pointTop, v4]);
        walls.AddTriangleUnperturbed([pointTop, v3, v4]);
    }
}