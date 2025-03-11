using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

public interface IPlanetSettingService
{
    float Radius { get; set; }
    int ChunkDivisions { get; set; }
    int Divisions { get; set; }
    float UnitHeight { get; }
    float MaxHeight { get; }
    float MaxHeightRatio { get; }
    int ElevationStep { get; set; }
    float StandardScale { get; }

    #region 河流和水体

    float GetStreamBedHeight(int elevation);
    float GetWaterSurfaceHeight(int level);

    #endregion

    #region 特征

    float[] GetFeatureThreshold(int level);
    float GetWallHeight();
    float GetWallThickness();
    Vector3 WallThicknessOffset(Vector3 near, Vector3 far, bool toNear, float thickness);
    Vector3 WallLerp(Vector3 near, Vector3 far);

    #endregion
}