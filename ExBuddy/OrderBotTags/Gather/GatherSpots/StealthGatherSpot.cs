namespace ExBuddy.OrderBotTags.Gather.GatherSpots
{
	using Buddy.Coroutines;
	using Clio.XmlEngine;
	using ExBuddy.Helpers;
	using ff14bot;
	using System.Threading.Tasks;
	using ff14bot.Behavior;
	using ff14bot.Managers;
	using ff14bot.Navigation;

    [XmlElement("StealthGatherSpot")]
	public class StealthGatherSpot : GatherSpot
	{
		[XmlAttribute("UnstealthAfter")]
		public bool UnstealthAfter { get; set; }

		public override async Task<bool> MoveFromSpot(ExGatherTag tag)
		{
			tag.StatusText = "Moving from " + this;

			if (UnstealthAfter && Core.Player.HasAura((int)AbilityAura.Stealth))
			{
				return await tag.CastAura(Ability.Stealth);
			}

			return true;
		}

		public override async Task<bool> MoveToSpot(ExGatherTag tag)
		{
			tag.StatusText = "Moving to " + this;

			var result =
				await
					NodeLocation.MoveTo(
						UseMesh,
						radius: tag.Distance,
						name: tag.Node.EnglishName,
						stopCallback: tag.MovementStopCallback,
						dismountAtDestination: true);

			if (result)
			{
			    var landed = MovementManager.IsDiving || await NewLandingTask();
			    if (landed && Core.Player.IsMounted)
			        ActionManager.Dismount();

			    Navigator.Stop();
                await Coroutine.Yield();
				await tag.CastAura(Ability.Stealth, AbilityAura.Stealth);
			}

			await Coroutine.Yield();

			return result;
        }

        private async Task<bool> NewLandingTask()
        {
            if (!MovementManager.IsFlying) { return true; }

            var _en = "Mounted"; // Works on all Languages.
            //var _jp = "??"; Don't know Mounted name in JP
            //var _fr = "Sur une monture";
            //var _de = "Beritten";
            //var _cn ="??"; Don't know Mounted name in CN

            // statusoff is persistent thru all versions (CN is unknown).
            ChatManager.SendChat("/statusoff \"" + _en + "\"");

            while (MovementManager.IsFlying) { await Coroutine.Yield(); }

            return true;
        }
    }
}