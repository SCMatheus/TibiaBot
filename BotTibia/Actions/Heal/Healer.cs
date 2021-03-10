using System.Drawing;
using System.Threading;
using AutoHotkey.Interop;
using BotTibia.Actions.Print;
using System.Diagnostics;
using System.Linq;
using BotTibia.Classes;
using BotTibia.Actions.AHK;

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
                    AhkFunctions.SendKey(vida.LowHeal.Key, processName);
                    Thread.Sleep(50);
                    AhkFunctions.SendKey(vida.MediumHeal.Key, processName);
                }
                else
                {
                    AhkFunctions.SendKey(vida.LowHeal.Key, processName);

                }
            }
            else if (tela.GetPixel(vida.MediumHeal.X, vida.MediumHeal.Y) != vida.pixel)
            {
                AhkFunctions.SendKey(vida.MediumHeal.Key, processName);
            }
            else if (tela.GetPixel(vida.HighHeal.X, vida.HighHeal.Y) != vida.pixel)
            {
                AhkFunctions.SendKey(vida.HighHeal.Key, processName);
            }
        }
        public static void HealDeMana(Bitmap tela, ManaBar mana, VidaBar vida, string processName, bool ehEk)
        {
            if (ehEk)
            {
                if (tela.GetPixel(vida.LowHeal.X, vida.LowHeal.Y) == vida.pixel) {
                    if (tela.GetPixel(mana.ManaHeal.X, mana.ManaHeal.Y) != mana.pixel)
                    {
                        AhkFunctions.SendKey(mana.ManaHeal.Key, processName);
                    }
                }
            }
            else
            {
                if (tela.GetPixel(mana.ManaHeal.X, mana.ManaHeal.Y) != mana.pixel)
                {
                    AhkFunctions.SendKey(mana.ManaHeal.Key, processName);
                }
            }
        }

        public static void HealPara(Bitmap tela, PersonagemStatus status,string processName)
        {
            if (PegaElementosDaTela.PegaParalize(CapturaTela.CortaStatusBar(tela, status)))
            {
                AhkFunctions.SendKey(status.ParaKey, processName);
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
