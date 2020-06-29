using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using BotTibia.Elementos;

namespace BotTibia.Acoes
{
    public static class Healer
    {
        public static void HealDeVida(Bitmap tela, VidaBar vida)
        {
            if (tela.GetPixel(vida.LowHeal.X, vida.LowHeal.Y) != vida.pixel)
            {
                SendKeys.SendWait("{" + vida.LowHeal.Key + "}");
            }
            else if (tela.GetPixel(vida.MediumHeal.X, vida.MediumHeal.Y) != vida.pixel)
            {
                SendKeys.SendWait("{" + vida.MediumHeal.Key + "}");
            }
            else if (tela.GetPixel(vida.HighHeal.X, vida.HighHeal.Y) != vida.pixel)
            {
                SendKeys.SendWait("{" + vida.HighHeal.Key + "}");
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

        public static void Healar(VidaBar vida,  ManaBar mana, PersonagemStatus status, int fireTimer, string personagem)
        {
            while (true)
            {
                var telaTibia = false;
                var telaEmPrimeiroPlano = PegaTelaPrincipal.GetActiveWindowTitle();
                if (telaEmPrimeiroPlano != null) {
                    telaTibia = telaEmPrimeiroPlano.Contains(personagem);
                }
                if (telaTibia)
                {
                    var tela = CapturaTela.CapturaDeTela();

                    HealDeVida(tela, vida);

                    HealDeMana(tela, mana);

                    if (status.ParaStatus)
                    {
                        HealPara(tela, status);
                    }

                    Thread.Sleep(fireTimer);
                }
                else
                {
                    Thread.Sleep(2000);
                }
            }
        }
    }
}
