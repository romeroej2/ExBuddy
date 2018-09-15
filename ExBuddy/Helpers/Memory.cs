namespace ExBuddy.Helpers
{
    using System;
    using System.Linq;
    using ff14bot;
    using ff14bot.Managers;
    using GreyMagic;
    using Offsets;
    using OrderBotTags.Behaviors.Objects;

    public static class Memory
    {
        public static class Request
        {
            public static uint ItemId1 => GetItemByIndex(0);

            public static uint ItemId2 => GetItemByIndex(1);

            public static uint ItemId3 => GetItemByIndex(2);

            public static uint[] ItemsToTurnIn { get { return new[] {Request.ItemId1, Request.ItemId2, Request.ItemId3}.Where(i => i > 0).ToArray(); } }

            public static uint GetItemByIndex(int index)
            {
                var ptr = RequestOffsets.ItemBasePtr + MarshalCache<IntPtr>.Size;
                return Core.Memory.NoCacheRead<uint>(ptr + RequestOffsets.ItemSize * index + MarshalCache<IntPtr>.Size);
            }
        }

        public static class Scrips
        {
            public static int RedCrafter => (int) SpecialCurrencyManager.GetCurrencyCount(SpecialCurrency.RedCraftersScrips);

            public static int RedGatherer => (int) SpecialCurrencyManager.GetCurrencyCount(SpecialCurrency.RedGatherersScrips);

            public static int YellowCrafter => (int) SpecialCurrencyManager.GetCurrencyCount(SpecialCurrency.YellowCraftersScrips);

            public static int YellowGatherer => (int) SpecialCurrencyManager.GetCurrencyCount(SpecialCurrency.YellowGatherersScrips);

            public static int CenturioSeals => (int) SpecialCurrencyManager.GetCurrencyCount(SpecialCurrency.CenturioSeals);

            public static int GetRemainingScripsByShopType(ShopType shopType)
            {
                switch (shopType)
                {
                    case ShopType.RedCrafter50:
                        return Scrips.RedCrafter;                        
                    case ShopType.RedCrafter58:
                        return Scrips.RedCrafter;
                    case ShopType.RedCrafter61:
                        return Scrips.RedCrafter;
                    case ShopType.YellowCrafterItems:
                        return Scrips.YellowCrafter;
                    case ShopType.RedGatherer50:
                        return Scrips.RedGatherer;
                    case ShopType.RedGatherer58:
                        return Scrips.RedGatherer;
                    case ShopType.RedGatherer61:
                        return Scrips.RedGatherer;
                    case ShopType.YellowGathererItems:
                        return Scrips.YellowGatherer;
                }

                return 0;
            }
        }
    }
}