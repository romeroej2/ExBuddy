// ReSharper disable once CheckNamespace
namespace ExBuddy.OrderBotTags.Behaviors
{
    using System.Linq;
    using System.Threading.Tasks;
    using Buddy.Coroutines;
    using Clio.XmlEngine;
    using ff14bot.Managers;
    using ff14bot.Objects;
    using ff14bot.RemoteWindows;
    using ExBuddy.Windows;
    using System.ComponentModel;

    [XmlElement("EtxRetainer")]
    public class EtxRetainer : ExProfileBehavior
    {
        [DefaultValue(2)]
        [XmlAttribute("Retainers")]
        public int RetainerCount { get; set; }

        public new void Log(string text, params object[] args) { Logger.Mew("[EtxRetainer] " + string.Format(text, args)); }

        private byte retainersLeft;

        private string retainerName;

        protected override async Task<bool> Main()
        {
            var retainerList = new RetainerList();

            foreach (var unit in GameObjectManager.GetObjectsOfType<EventObject>().OrderBy(r => r.Distance()))
                if (unit.Name == "Summoning Bell" || unit.Name == "Sonnette" || unit.Name == "Krämerklingel" || unit.Name == "リテイナーベル")
                {
                    unit.Interact();
                    break;
                }

            if (!await Coroutine.Wait(5000, () => RetainerList.IsOpen)) return isDone = true;

            while (retainersLeft < RetainerCount)
            {
                Log("Checking Retainer Nº {0}", retainersLeft + 1);
                // Open specified retainer
                await retainerList.OpenRetainer(retainersLeft);
                retainersLeft++;
                await Coroutine.Wait(5000, () => Talk.DialogOpen);
                // Skip dialog
                Talk.Next();
                await Coroutine.Wait(5000, () => SelectString.IsOpen);
                foreach (var retainer in GameObjectManager.GetObjectsOfType<BattleCharacter>(true).OrderBy(r => r.Distance()))
                    if (retainer.Type.ToString() == "Retainer")
                    {
                        retainerName = retainer.Name;
                        break;
                    }

                switch (await CheckRetainer())
                {
                    case VentureCheck.Completed:
                        Log("Venture complete! Sent {0} out again.", retainerName);
                        break;
                    case VentureCheck.InProgress:
                        Log("Looks like {0} is still out on a venture! Skipping.", retainerName);
                        break;
                    case VentureCheck.None:
                        Log("{0} isn't assigned any ventures. Skipping.", retainerName);
                        break;
                    default:
                        Log("Something went wrong?");
                        break;
                }
                await CloseRetainer();
            }

            Log("Checked all retainers.");
            // Close retainers window.
            if (!retainerList.IsValid) await retainerList.Refresh(1000);
            await retainerList.CloseInstance(300);
            return isDone = true;
        }

        internal async Task<bool> CloseRetainer()
        {
            if (Talk.DialogOpen) Talk.Next();
            await Coroutine.Wait(5000, () => SelectString.IsOpen);
            // Select quit
            SelectString.ClickSlot((uint)SelectString.LineCount - 1);
            await Coroutine.Wait(5000, () => Talk.DialogOpen);
            // Skip Dialog
            Talk.Next();
            await Coroutine.Wait(5000, () => !Talk.DialogOpen);
            return !Talk.DialogOpen;
        }

        internal enum VentureCheck { Completed, InProgress, None }

        internal async Task<VentureCheck> CheckRetainer()
        {
            foreach (var lines in SelectString.Lines())
            {
                if (lines.EndsWith("(In progress)")) return VentureCheck.InProgress;
                if (!lines.EndsWith("(Complete)")) continue;
                // Click on the completed venture
                SelectString.ClickSlot(5);
                await Coroutine.Wait(5000, () => RetainerTaskResult.IsOpen);
                // Assign a new venture
                RetainerTaskResult.Reassign();
                await Coroutine.Wait(5000, () => RetainerTaskAsk.IsOpen);
                // Confirm new venture
                RetainerTaskAsk.Confirm();
                await Coroutine.Wait(5000, () => Talk.DialogOpen);
                // Skip dialog
                Talk.Next();
                return VentureCheck.Completed;
            }
            return VentureCheck.None;
        }
    }
}