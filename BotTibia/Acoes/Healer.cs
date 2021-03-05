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
        public static void HealDeVida(Bitmap tela, VidaBar vida, string processName)
        {
            if (tela.GetPixel(vida.LowHeal.X, vida.LowHeal.Y) != vida.pixel)
            {
                AutoHotkeyEngine _ahkEngine = new AutoHotkeyEngine();
                var script = "ControlSend,, {" +vida.LowHeal.Key + "}," + processName;
                _ahkEngine.ExecRaw(script);
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
        public static void HealDeMana(Bitmap tela, ManaBar mana)
        {
            if (tela.GetPixel(mana.ManaHeal.X, mana.ManaHeal.Y) != mana.pixel)
            {
                SendKeys.SendWait("{" + mana.ManaHeal.Key + "}");
            }
        }

        public static void HealPara(Bitmap tela, PersonagemStatus status)
        {
            if (PegaElementosDaTela.PegaParalize(CapturaTela.CortaStatusBar(tela, status)))
            {
                SendKeys.SendWait("{" + status.ParaKey + "}");
            }
        }

        public static void Healar(VidaBar vida,  ManaBar mana, PersonagemStatus status, int fireTimer, string processName)
        {
            try
            {
                while (true)
                {
                    var tela = CapturaTela.CaptureWindow(processName);

                    HealDeVida(tela, vida, processName);

                    HealDeMana(tela, mana);

                    if (status.ParaStatus)
                    {
                        HealPara(tela, status);
                    }
                    Thread.Sleep(fireTimer);

                }
            }catch{

            }
        }
    }
}
