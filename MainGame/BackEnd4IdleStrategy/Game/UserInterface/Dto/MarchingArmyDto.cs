using BackEnd4IdleStrategy.Common.Constants;
using BackEnd4IdleStrategyFS.Game;

namespace BackEnd4IdleStrategy.Game.UserInterface.Dto;

public record MarchingArmyDto
(
    int Id = Constant.NullId,
    int Population = 0,
    int PlayerId = Constant.NullId,
    InitTileDto? FromTile = default,
    InitTileDto? ToTile = default
)
{
    // TODO：为了先跑通代码写的恶心构造，后续优化
    internal static MarchingArmyDto From(DomainT.MarchingArmy marchingArmy, InitTileDto fromTile, InitTileDto toTile) =>
        new (
            marchingArmy.id.Item,
            marchingArmy.population,
            marchingArmy.playerId.Item,
            fromTile,
            toTile
        );
    
    
    
}