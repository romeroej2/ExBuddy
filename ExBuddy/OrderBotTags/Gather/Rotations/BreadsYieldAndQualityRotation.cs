﻿namespace ExBuddy.OrderBotTags.Gather.Rotations
{
    using Buddy.Coroutines;
    using Attributes;
    using Enumerations;
    using ff14bot;
    using ff14bot.Managers;
    using Helpers;
    using Interfaces;
    using System.Threading.Tasks;
    using System;

    //Name, RequiredTime, RequiredGpBreakpoints
    [GatheringRotation("BreadsYieldAndQuality", 25, 800, 775, 675, 575, 475, 375, 0)]
    public class EndGameYieldAndQualityGatheringRotation : SmartGatheringRotation, IGetOverridePriority
    {
        private readonly ushort level = Core.Player.ClassLevel;

        #region IGetOverridePriority Members

        int IGetOverridePriority.GetOverridePriority(ExGatherTag tag)
        {
            if (tag.CollectableItem != null)
            {
                return -1;
            }

            if (tag.GatherItem != null && tag.GatherItem.HqChance < 1)
            {
                return -1;
            }

            if (level >= 63)
            {
                switch (tag.GatherIncrease)
                {
                    case GatherIncrease.Auto:
                        return 9199;
                    case GatherIncrease.YieldAndQuality:
                        return 9200;
                }
            }

            return -1;
        }

        #endregion IGetOverridePriority Members

        public override async Task<bool> ExecuteRotation(ExGatherTag tag)
        {
            if (!tag.Node.IsUnspoiled())
            {
                if (!(GatheringManager.SwingsRemaining > 4 && Core.Player.CurrentGP >= 300 && level >= 63) || tag.GatherItem.HqChance >= 100) return true;
                if (tag.GatherItem.HqChance >= 90)
                {
                    await tag.Cast(Ability.IncreaseGatherQuality10);
                    await Wait();
                }
                else
                {
                    await tag.Cast(Ability.IncreaseGatherQuality30100);
                    await Wait();
                }
                await IncreaseChance(tag);
                await Wait();
                return true;
            }

            if (Core.Player.CurrentGP >= 300 && level >= 63 && tag.GatherItem.HqChance < 100)
            {
                if (tag.GatherItem.HqChance >= 90)
                {
                    await tag.Cast(Ability.IncreaseGatherQuality10);
                    await Wait();
                }
                else
                {
                    await tag.Cast(Ability.IncreaseGatherQuality30100);
                    await Wait();
                }

                await IncreaseChance(tag);
                await Wait();
            }

            var endlessToil = tag.GatherItem.Level <= 60 ? 0 : 1;

            double YieldsLeft()
            {
                if (GatheringManager.MaxSwings == GatheringManager.SwingsRemaining) return Math.Min(GatheringManager.MaxSwings - endlessToil, Math.Max(0, (Core.Player.CurrentGP + (GatheringManager.MaxSwings - (endlessToil + 1)) * 5) / 100));
                else return Math.Min(GatheringManager.SwingsRemaining, Math.Max(0, (Core.Player.CurrentGP + (GatheringManager.SwingsRemaining - 1) * 5) / 100));
            }

            var bountifulYield = tag.GatherItem.Level <= 70 ? 3 : 2;

            double uncappedYields = Math.Floor((Core.Player.CurrentGP + (GatheringManager.MaxSwings - (endlessToil + 1)) * 5) / 100d);

            if (Core.Player.CurrentGP >= 500 && level >= 40)
            {
                if (((GatheringManager.MaxSwings - endlessToil) * 2) + (uncappedYields - 5) >= bountifulYield * Math.Floor(YieldsLeft()))
                {
                    await tag.Cast(Ability.IncreaseGatherYield2);
                    await Wait();
                }
            }

            bool usedVigorForce = false;

            while (tag.Node.CanGather && GatheringManager.SwingsRemaining > 0 && Behaviors.ShouldContinue)
            {
                await Wait();

                if (!usedVigorForce && GatheringManager.GatheringCombo == 4 && GatheringManager.SwingsRemaining > 0 && ((tag.GatherItem.HqChance > 0 && tag.GatherItem.HqChance < 100) || tag.GatherItem.Chance < 100))
                {
                    await tag.Cast(Ability.IncreaseGatherChanceQuality100);
                    usedVigorForce = true;
                }

                if (Core.Player.CurrentGP >= 100 && level >= 68 && (tag.GatherItem.HqChance == 100 || GatheringManager.SwingsRemaining <= YieldsLeft()) && !Core.Player.HasAura(1286))
                {
                    await tag.Cast(Ability.IncreaseGatherYieldOnce2);
                    await Coroutine.Wait(2000, () => Core.Player.HasAura(1286));
                    await Wait();
                }

                double counsels = Math.Floor(YieldsLeft() + 0.1);

                if (Core.Player.CurrentGP >= 10 && level >= 67 && tag.GatherItem.HqChance > 0 && tag.GatherItem.HqChance < 100 && YieldsLeft() >= counsels && !Core.Player.HasAura(1262))
                {
                    await tag.Cast(Ability.IncreaseGatherQualityRandom);
                    await Coroutine.Wait(2000, () => Core.Player.HasAura(1262));
                    await Wait();
                }

                if (!await tag.ResolveGatherItem()) return false;

                if (!tag.GatherItem.TryGatherItem()) return false;

                await Coroutine.Yield();
            }

            return true;
        }
    }
}