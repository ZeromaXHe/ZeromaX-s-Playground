using BackEnd4IdleStrategy.Common.Constants;
using BackEnd4IdleStrategy.Game.Domain.Entities;

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
    internal static MarchingArmyDto From(MarchingArmy marchingArmy, Tile fromTile, Tile toTile)
        => new MarchingArmyDto(
            marchingArmy.Id,
            marchingArmy.Population,
            marchingArmy.PlayerId,
            InitTileDto.From(fromTile),
            InitTileDto.From(toTile)
        );
}