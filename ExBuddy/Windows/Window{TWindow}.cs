﻿namespace ExBuddy.Windows
{
	using Buddy.Coroutines;
	using ExBuddy.Enumerations;
	using ExBuddy.Helpers;
	using ExBuddy.Logging;
	using ff14bot;
	using ff14bot.AClasses;
	using ff14bot.Behavior;
	using ff14bot.Managers;
	using System;
	using System.Threading.Tasks;
    using ff14bot.RemoteWindows;

    public abstract class Window<TWindow>
		where TWindow : Window<TWindow>, new()
	{
		// ReSharper disable once StaticMemberInGenericType
		private static Action updateWindows;

		private AtkAddonControl control;

		static Window()
		{
			updateWindows = RaptureAtkUnitManager.Update;
			TreeRoot.OnStart += TreeRootOnStart;
			TreeRoot.OnStop += TreeRootOnStop;
		}

		protected Window(string name)
		{
			Name = name;
			control = RaptureAtkUnitManager.GetWindowByName(name);
		}

		public static AtkAddonControl AtkAddonControl
		{
			get { return new TWindow().Control; }
		}

		public virtual AtkAddonControl Control
		{
			get { return control ?? Refresh().control; }
		}

		public static bool IsOpen
		{
			get { return new TWindow().Control != null; }
		}

		public bool IsValid
		{
			get { return Control != null && Control.IsValid; }
		}

		public string Name { get; set; }

		public static void Close()
		{
			new TWindow().Control.TrySendAction(1, 3, uint.MaxValue);
		}

		public static async Task<bool> CloseGently(byte maxTicks = 10, ushort interval = 200)
		{
			return await new TWindow().CloseInstanceGently(maxTicks, interval);
		}

		public virtual async Task<SendActionResult> CloseInstance(ushort interval = 250)
		{
			await Behaviors.Sleep(interval);

			Logger.Instance.Verbose(Localization.Localization.Window_Attempting, Name);

			var result = TrySendAction(1, 3, uint.MaxValue);

			await Refresh(interval / 2, false);

			if (result == SendActionResult.Success)
			{
				if (!IsValid)
				{
					Logger.Instance.Verbose(Localization.Localization.Window_Closed, Name);
#if !RB_CN
                    await CloseSelectString();
#endif
                    return result;
				}

				Logger.Instance.Verbose(Localization.Localization.Window_WaitToClose, interval * 2, Name);
				await Refresh(interval * 2, false);

				if (!IsValid)
				{
					Logger.Instance.Verbose(Localization.Localization.Window_Closed, Name);
#if !RB_CN
                    await CloseSelectString();
#endif
                    return result;
				}

				Logger.Instance.Verbose(Localization.Localization.Window_UnexpectedResult, Name);
				return SendActionResult.UnexpectedResult;
			}

			if (result == SendActionResult.InvalidWindow)
			{
				Logger.Instance.Verbose(Localization.Localization.Window_invalid, Name);
			}

			return result;
        }

        private async Task<bool> CloseSelectString(ushort interval = 250)
        {
            await Coroutine.Wait(1000, () => SelectString.IsOpen);

            if (SelectString.IsOpen)
            {
                SelectString.ClickSlot((uint) SelectString.LineCount - 1);
            }

            await Coroutine.Wait(1000, () => !SelectString.IsOpen);

            return !SelectString.IsOpen;
        }


        public virtual async Task<bool> CloseInstanceGently(byte maxTicks = 10, ushort interval = 200)
		{
			if (!IsValid)
			{
				return true;
			}

			if (await CloseInstance(interval) == SendActionResult.Success)
			{
				if (!IsValid)
				{
					return true;
				}
			}

			await Behaviors.Sleep(interval);

			var result = SendActionResult.None;
			var ticks = 0;
			while (result != SendActionResult.Success && ticks++ < maxTicks && IsValid && Behaviors.ShouldContinue)
			{
				if (result == SendActionResult.InvalidWindow)
				{
					return true;
				}

				result = await CloseInstance(interval);
			}

			return result > SendActionResult.UnexpectedResult && !IsValid;
		}

		public TWindow Refresh()
		{
			updateWindows();
			control = RaptureAtkUnitManager.GetWindowByName(Name);
			return (TWindow)this;
		}

		public async Task<bool> Refresh(int timeoutMs, bool valid = true)
		{
			return await Coroutine.Wait(timeoutMs, () => Refresh().IsValid == valid);
		}
        
        public virtual SendActionResult TrySendAction(int pairCount, params ulong[] param)
		{
			return Control.TrySendAction(pairCount, param);
		}

		private static void TreeRootOnStart(BotBase bot)
		{
			if (bot.PulseFlags.HasFlag(PulseFlags.Windows))
			{
				updateWindows = () => { };
			}
		}

		private static void TreeRootOnStop(BotBase bot)
		{
			updateWindows = RaptureAtkUnitManager.Update;
		}
	}
}