namespace ExBuddy.OrderBotTags.Behaviors.Objects
{
    public enum ShopType
    {
#if RB_CN
        RedCrafter50,

        RedCrafter58,

        RedCrafter61,

        RedCrafterMasterRecipes,

        YellowCrafter,

        YellowCrafterII,

        YellowCrafterAugmentation,

        YellowCrafterItems,

        RedGatherer50,

        RedGatherer58,

        RedGatherer61,

        YellowGatherer,

        YellowGathererII,

        YellowGathererAugmentation,

        YellowGathererItems
#else
        Yellow50,

        Yellow58,

        Yellow61,

        MasterRecipes,

        YellowGear,

        YellowGearII,

        YellowAugmentation,

        Yellow70,

        WhiteGear,

        White80,
#endif
    }
}