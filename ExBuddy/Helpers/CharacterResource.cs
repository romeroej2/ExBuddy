using System;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.NeoProfiles;
using ff14bot.Objects;

namespace ExBuddy.Helpers
{
#if !RB_CN
    public static class CharacterResource
    {
        public static ushort GetGpPerTick()
        {
			ushort gpTick = 5;

            if (CharacterResource.Me.CurrentJob == ClassJobType.Miner && ConditionParser.IsQuestCompleted(68094) ||
                CharacterResource.Me.CurrentJob == ClassJobType.Botanist && ConditionParser.IsQuestCompleted(68160) ||
                CharacterResource.Me.CurrentJob == ClassJobType.Fisher && ConditionParser.IsQuestCompleted(68435))
            { gpTick++; }

            if (Core.Player.ClassLevel >= 80) gpTick++;
			
            return gpTick;
        }

        public static ushort GetEffectiveGp(int ticksTillGather)
        {
            return GetEffectiveGp(ticksTillGather, GetGpPerTick());
        }

        public static ushort GetEffectiveGp(int ticksTillGather, int gpPerTick)
        {
            return ticksTillGather <= 0
                ? CharacterResource.Me.CurrentGP
                : (ushort) Math.Min(CharacterResource.Me.CurrentGP + (ticksTillGather * gpPerTick), CharacterResource.Me.MaxGP);
        }

        public static TimeSpan EstimateExpectedRegenerationTime(ushort gpNeeded)
        {
            return EstimateExpectedRegenerationTime(gpNeeded, CharacterResource.GetGpPerTick());
        }

        public static TimeSpan EstimateExpectedRegenerationTime(ushort gpNeeded, ushort gpPerTick)
        {
            var gpNeededTicks = gpNeeded / gpPerTick;
            var gpNeededSeconds = gpNeededTicks * 3;

            return TimeSpan.FromSeconds(gpNeededSeconds);
        }

        private static LocalPlayer Me
        {
            get { return GameObjectManager.LocalPlayer; }
        }
    }
#endif
}
