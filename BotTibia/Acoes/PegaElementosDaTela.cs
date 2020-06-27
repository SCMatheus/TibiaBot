using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using AForge.Imaging;
using Image = System.Drawing.Image;

namespace BotTibia
{
    public static class PegaElementosDaTela
    {
        public static CoordenadasDeElementos PegaElementos(Bitmap tela, string item)
        {



            //Bitmap tela = (Bitmap)Image.FromFile(@"tela.png");
            //Bitmap telaBit = tela.Clone(new Rectangle(0, 0, tela.Width, tela.Height), PixelFormat.Format24bppRgb);
            Bitmap elemento = (Bitmap)Image.FromFile(@item + ".png");
            Bitmap datagraphic = elemento.Clone(new Rectangle(0, 0, elemento.Width, elemento.Height), PixelFormat.Format24bppRgb);

            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(1);
            // find all matchings with specified above similarity
            TemplateMatch[] matchings = tm.ProcessImage(tela, datagraphic);
            if (matchings.Length == 1)
            {
                var retorno = new CoordenadasDeElementos()
                {
                    X = matchings[0].Rectangle.X + ((Screen.PrimaryScreen.Bounds.Width / 3) * 2),
                    Y = matchings[0].Rectangle.Y,
                    Height = matchings[0].Rectangle.Height,
                    Width = matchings[0].Rectangle.Width,
                };
                return retorno;
            }
            else
            {
                throw new Exception("Não foi possivel capturar algum dos status! \n Por favor Deixar vida e mana full");
            }
        }

        public static bool PegaParalize(Bitmap tela)
        {
            var R = 0;
            var G = 0;
            var B = 0;
            for(int i = 1; i < tela.Width; i++)
            {
                R = tela.GetPixel(i, (int)(tela.Height / 2)).R;
                B = tela.GetPixel(i, (int)(tela.Height / 2)).G;
                G = tela.GetPixel(i, (int)(tela.Height / 2)).B;
                if (R == 255 && G == 0 && B == 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
