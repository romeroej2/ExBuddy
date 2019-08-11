// ReSharper disable once CheckNamespace

namespace ExBuddy.OrderBotTags.Behaviors
{
#if RB_CN
    using System.Linq;
    using System.Threading.Tasks;
    using Buddy.Coroutines;
    using Clio.XmlEngine;
    using ff14bot.Managers;
    using ff14bot.RemoteWindows;

    [XmlElement("EtxRetainer")]
    public class EtxRetainer : ExProfileBehavior
    {
        public new void Log(string text, params object[] args) { Logger.Mew("[EtxRetainer] " + string.Format(text, args)); }

        protected override async Task<bool> Main()
        {
            foreach (var unit in GameObjectManager.GameObjects.OrderBy(r => r.Distance()))
                if (unit.NpcId == 2000401 || unit.NpcId == 2000441)
                {
                    unit.Interact();
                    break;
                }
            if (!await Coroutine.Wait(5000, () => SelectString.IsOpen)) return isDone = true;
            {
                uint count = 0;
                var lineC = SelectString.LineCount;
                var countLine = (uint)lineC;
                foreach (var retainer in SelectString.Lines())
                    if (retainer.EndsWith("]") || retainer.EndsWith(")"))
                    {
                        Log("Checking Retainer n° " + (count + 1));
                        // If Venture Completed
                        if (retainer.EndsWith("[探险归来]") || retainer.EndsWith("[Tâche terminée]") || retainer.EndsWith("(Venture complete)"))
                        {
                            Log("Venture Completed !");
                            // Select the retainer
                            SelectString.ClickSlot(count);
                            if (!await Coroutine.Wait(5000, () => Talk.DialogOpen)) continue;
                            // Skip Dialog
                            Talk.Next();
                            if (!await Coroutine.Wait(5000, () => SelectString.IsOpen)) continue;
                            // Click on the completed venture
                            SelectString.ClickSlot(5);
                            if (!await Coroutine.Wait(5000, () => RetainerTaskResult.IsOpen)) continue;
                            // Assign a new venture
                            RetainerTaskResult.Reassign();
                            if (!await Coroutine.Wait(5000, () => RetainerTaskAsk.IsOpen)) continue;
                            // Confirm new venture
                            RetainerTaskAsk.Confirm();
                            if (!await Coroutine.Wait(5000, () => Talk.DialogOpen)) continue;
                            // Skip Dialog
                            Talk.Next();
                            if (!await Coroutine.Wait(5000, () => SelectString.IsOpen)) continue;
                            SelectString.ClickSlot((uint)SelectString.LineCount - 1);
                            if (!await Coroutine.Wait(5000, () => Talk.DialogOpen)) continue;
                            // Skip Dialog
                            Talk.Next();
                            await Coroutine.Sleep(3000);
                            foreach (var unit in GameObjectManager.GameObjects.OrderBy(r => r.Distance()))
                                if (unit.NpcId == 2000401 || unit.NpcId == 2000441)
                                {
                                    unit.Interact();
                                    break;
                                }
                            if (await Coroutine.Wait(5000, () => SelectString.IsOpen))
                                count++;
                        }
                        else
                        {
                            Log("Venture not Completed !");
                            count++;
                        }
                    }
                    else
                    {
                        Log("No more Retainer to check");
                        SelectString.ClickSlot(countLine - 1);
                    }
                return isDone = true;
            }
        }
    }
#endif
}