namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Enum;

public enum TileType
{
    // 正二十面体南北极顶点。
    // 索引：0 北极点，1 南极点
    PoleVertices,
    // 正二十面体中间的十个顶点。
    // 索引：0、1 第一组竖向四面的中间右侧从北到南两点，2、3 第二组，以此类推，8、9 第五组（最后一组）
    MidVertices,
    // 正二十面体边点
    // 索引：0 ~ 5 第一组竖向四面的从北到南六边（左侧三边不算），6 ~ 11 第二组，以此类推，24 ~ 29 第五组（最后一组）
    Edges,
    // 正二十面体面点
    // 索引：0 ~ 3 第一组竖向四面从北到南，4 ~ 7 第二组，以此类推，16 ~ 19 第五组（最后一组）
    Faces,
}