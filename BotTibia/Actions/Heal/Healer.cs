using System.Drawing;
using System.Threading;
using AutoHotkey.Interop;
using BotTibia.Actions.Print;
using System.Diagnostics;
using System.Linq;
using BotTibia.Classes;

namespace BotTibia.Actions.Heal
{
    public static class Healer
    {
        public static void HealDeVida(Bitmap tela, VidaBar vida, string processName, bool ehEk)
        {
            if (tela.GetPixel(vida.LowHeal.X, vida.LowHeal.Y) != vida.pixel)
            {
                if (ehEk)
                {
                    var _ahkEngine = AutoHotkeyEngine.Instance;
                    var script = "ControlSend,, {" + vida.LowHeal.Key + "}," + processName;
                    _ahkEngine.ExecRaw(script);
                    _ahkEngine.ExecRaw("Sleep 50");
                    script = "ControlSend,, {" + vida.MediumHeal.Key + "}," + processName;
                    _ahkEngine.ExecRaw(script);
                }
                else
                {
                    var _ahkEngine = AutoHotkeyEngine.Instance;
                    var script = "ControlSend,, {" + vida.LowHeal.Key + "}," + processName;
                    _ahkEngine.ExecRaw(script);
                }
            }
            else if (tela.GetPixel(vida.MediumHeal.X, vida.MediumHeal.Y) != vida.pixel)
            {
                var _ahkEngine = AutoHotkeyEngine.Instance;
                var script = "ControlSend,, {" + vida.MediumHeal.Key + "}," + processName;
                _ahkEngine.ExecRaw(script);
            }
            else if (tela.GetPixel(vida.HighHeal.X, vida.HighHeal.Y) != vida.pixel)
            {
                var _ahkEngine = AutoHotkeyEngine.Instance;
                var script = "ControlSend,, {" + vida.HighHeal.Key + "}," + processName;
                _ahkEngine.ExecRaw(script);
            }
        }
        public static void HealDeMana(Bitmap tela, ManaBar mana, VidaBar vida, string processName, bool ehEk)
        {
            if (ehEk)
            {
                if (tela.GetPixel(vida.LowHeal.X, vida.LowHeal.Y) == vida.pixel) {
                    if (tela.GetPixel(mana.ManaHeal.X, mana.ManaHeal.Y) != mana.pixel)
                    {
                        var _ahkEngine = AutoHotkeyEngine.Instance;
                        var script = "ControlSend,, {" + mana.ManaHeal.Key + "}," + processName;
                        _ahkEngine.ExecRaw(script);
                    }
                }
            }
            else
            {
                if (tela.GetPixel(mana.ManaHeal.X, mana.ManaHeal.Y) != mana.pixel)
                {
                    var _ahkEngine = AutoHotkeyEngine.Instance;
                    var script = "ControlSend,, {" + mana.ManaHeal.Key + "}," + processName;
                    _ahkEngine.ExecRaw(script);
                }
            }
        }

        public static void HealPara(Bitmap tela, PersonagemStatus status,string processName)
        {
            if (PegaElementosDaTela.PegaParalize(CapturaTela.CortaStatusBar(tela, status)))
            {
                var _ahkEngine = AutoHotkeyEngine.Instance;
                var script = "ControlSend,, {" + status.ParaKey + "}," + processName;
                _ahkEngine.ExecRaw(script);
            }
        }

        public static void Healar(VidaBar vida,  ManaBar mana, PersonagemStatus status, int fireTimer, string processName)
        {

            while (true)
            {
                if(!Process.GetProcesses().ToList().Any(p => p.MainWindowTitle.Equals(processName)))
                {
                    Thread.Sleep(1000);
                    continue;
                }
                var tela = CapturaTela.CaptureWindow(processName);

                HealDeVida(tela, vida, processName, status.EhEk);

                HealDeMana(tela, mana, vida, processName, status.EhEk);

                if (status.ParaStatus)
                {
                    HealPara(tela, status, processName);
                }
                Thread.Sleep(fireTimer);

            }
        }
    }
}
