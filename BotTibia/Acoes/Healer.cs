using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using AutoHotkey.Interop;
using BotTibia.Elementos;

namespace BotTibia.Acoes
{
    public static class Healer
    {
        public static void HealDeVida(Bitmap tela, VidaBar vida, string processName, bool ehEk)
        {
            if (tela.GetPixel(vida.LowHeal.X, vida.LowHeal.Y) != vida.pixel)
            {
                if (ehEk)
                {
                    AutoHotkeyEngine _ahkEngine = new AutoHotkeyEngine();
                    var script = "ControlSend,, {" + vida.LowHeal.Key + "}," + processName;
                    _ahkEngine.ExecRaw(script);
                    _ahkEngine.ExecRaw("Sleep 50");
                    script = "ControlSend,, {" + vida.MediumHeal.Key + "}," + processName;
                    _ahkEngine.ExecRaw(script);
                }
                else
                {
                    AutoHotkeyEngine _ahkEngine = new AutoHotkeyEngine();
                    var script = "ControlSend,, {" + vida.LowHeal.Key + "}," + processName;
                    _ahkEngine.ExecRaw(script);
                }
            }
            else if (tela.GetPixel(vida.MediumHeal.X, vida.MediumHeal.Y) != vida.pixel)
            {
                AutoHotkeyEngine _ahkEngine = new AutoHotkeyEngine();
                var script = "ControlSend,, {" + vida.MediumHeal.Key + "}," + processName;
                _ahkEngine.ExecRaw(script);
            }
            else if (tela.GetPixel(vida.HighHeal.X, vida.HighHeal.Y) != vida.pixel)
            {
                AutoHotkeyEngine _ahkEngine = new AutoHotkeyEngine();
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
                        AutoHotkeyEngine _ahkEngine = new AutoHotkeyEngine();
                        var script = "ControlSend,, {" + mana.ManaHeal.Key + "}," + processName;
                        _ahkEngine.ExecRaw(script);
                    }
                }
            }
            else
            {
                if (tela.GetPixel(mana.ManaHeal.X, mana.ManaHeal.Y) != mana.pixel)
                {
                    AutoHotkeyEngine _ahkEngine = new AutoHotkeyEngine();
                    var script = "ControlSend,, {" + mana.ManaHeal.Key + "}," + processName;
                    _ahkEngine.ExecRaw(script);
                }
            }
        }

        public static void HealPara(Bitmap tela, PersonagemStatus status,string processName)
        {
            if (PegaElementosDaTela.PegaParalize(CapturaTela.CortaStatusBar(tela, status)))
            {
                AutoHotkeyEngine _ahkEngine = new AutoHotkeyEngine();
                var script = "ControlSend,, {" + status.ParaKey + "}," + processName;
                _ahkEngine.ExecRaw(script);
            }
        }

        public static void Healar(VidaBar vida,  ManaBar mana, PersonagemStatus status, int fireTimer, string processName)
        {
            try
            {
                while (true)
                {
                    var tela = CapturaTela.CaptureWindow(processName);

                    HealDeVida(tela, vida, processName, status.EhEk);

                    HealDeMana(tela, mana, vida, processName, status.EhEk);

                    if (status.ParaStatus)
                    {
                        HealPara(tela, status, processName);
                    }
                    Thread.Sleep(fireTimer);

                }
            }catch{

            }
        }
    }
}
