namespace ExBuddy.Windows
{
    using ExBuddy.Enumerations;
    using ExBuddy.Logging;
    using System.Threading.Tasks;
    using Buddy.Coroutines;
    using ExBuddy.Helpers;
    using ff14bot.RemoteWindows;

    public sealed class RetainerList : Window<RetainerList>
	{
		public RetainerList()
			: base("RetainerList") { }

        public SendActionResult TryOpenRetainer(byte retainersLeft)
        {
            return TrySendAction(2, 3, 2, 4, retainersLeft);
        }

        public async Task<bool> OpenRetainer(byte retainersLeft, byte attempts = 5, int sleep = 70, ushort interval = 2000)
        {
            var result = SendActionResult.None;
            var openAttempts = 0;
            await Behaviors.Wait(interval, () => RetainerList.IsOpen);
            while (result != SendActionResult.Success && !Talk.DialogOpen && openAttempts++ < attempts)
            {
                // Sleep here or else the game crashes. It's probably just that I'm inexperienced, but there seems to be no way to catch the exception.
                if (openAttempts < 2 || result == SendActionResult.InvalidWindow) await Coroutine.Sleep(sleep);
                result = TryOpenRetainer(retainersLeft);
                if (result != SendActionResult.Success) await Refresh(interval);
                // Any sort of injection error will crash the game here. But might as well have the clause.
                if (result == SendActionResult.InjectionError) return false;
            }

            return openAttempts > attempts;
        }
    }
}