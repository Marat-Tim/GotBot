using GotBot.Properties;

namespace GotBot.GameClasses;

public static class Decks
{
    public static readonly IReadOnlyList<IReadOnlyList<EventCard>> Westeros = new[]
    {
        new[]
        {
            Cards.WinterIsComing,
            Cards.LastDaysOfSummer,
            Cards.GatheringOfTroops,
            Cards.GatheringOfTroops,
            Cards.GatheringOfTroops,
            Cards.Supply,
            Cards.Supply,
            Cards.Supply,
            Cards.ThroneOfBlades,
            Cards.ThroneOfBlades
        },
        new[]
        {
            Cards.WinterIsComing,
            Cards.LastDaysOfSummer,
            Cards.BattleOfKings,
            Cards.BattleOfKings,
            Cards.BattleOfKings,
            Cards.GameOfThrones,
            Cards.GameOfThrones,
            Cards.GameOfThrones,
            Cards.BlackWingsBlackWords,
            Cards.BlackWingsBlackWords
        },
        new[]
        {
            Cards.LoyalToSword,
            Cards.LoyalToSword,
            Cards.WildlingsInvasion,
            Cards.WildlingsInvasion,
            Cards.WildlingsInvasion,
            Cards.StormOfSwords,
            Cards.AutumnRains,
            Cards.SeaOfStorms,
            Cards.WebOfLies,
            Cards.FeastForCrows
        }
    };

    public static readonly IReadOnlyList<EventCard> Wildlings = new[]
    {
        new EventCard("Тишина у стены", nameof(Resources.SilenceByTheWall)),
        new EventCard("Разбойники гремучей рубашки", nameof(Resources.RattlesnakeRobbers)),
        new EventCard("Сбор на молоководной", nameof(Resources.CollectionAtTheDairy)),
        new EventCard("Разведчик оборотень", nameof(Resources.WerewolfScout)),
        new EventCard("Передовой отряд", nameof(Resources.Vanguard)),
        new EventCard("Убийцы ворон", nameof(Resources.RavenKillers)),
        new EventCard("Наездники на мамонтах", nameof(Resources.MammothRiders)),
        new EventCard("Наступление орды", nameof(Resources.HordeOffensive)),
        new EventCard("Король за стеной", nameof(Resources.KingBehindWall)),
    };
}