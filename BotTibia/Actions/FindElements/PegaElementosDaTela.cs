using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using AForge.Imaging;
using AutoHotkey.Interop;
using BotTibia.Actions.AHK;
using Image = System.Drawing.Image;

namespace BotTibia
{
    public static class PegaElementosDaTela
    {
        public static CoordenadasDeElementos PegaElementos(Bitmap tela, int xSup, int ySup, int width, int height, string item, float tolerance = 1)
        {
            Bitmap elemento = (Bitmap)Image.FromFile(@item + ".png");
            Bitmap datagraphic = tela.Clone(new Rectangle(xSup, ySup, width, height), PixelFormat.Format24bppRgb);
            
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(tolerance);
            // find all matchings with specified above similarity
            TemplateMatch[] matchings = tm.ProcessImage(datagraphic, elemento);
            if (matchings.Length == 1)
            {
                var retorno = new CoordenadasDeElementos()
                {
                    X = matchings[0].Rectangle.X + ((Screen.PrimaryScreen.Bounds.Width / 2)),
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
        public static CoordenadasDeElementos PegaElementosAhk(string process,int x, int y, int width, int height, string elemento)
        {
            var hwnd = Process.GetProcesses().ToList().First(p => p.MainWindowTitle.Equals(process)).MainWindowHandle;
            var _ahkEngine = AutoHotkeyEngine.Instance;
            var retorno = _ahkEngine.ExecFunction("PegaElementosAhk",x.ToString(), y.ToString(),width.ToString(),
                                                  height.ToString(), elemento, hwnd.ToString());
            var parametros = retorno.Split(',');
            _ahkEngine = null;
            if (parametros[0] == "1")
            {
                var elementoX = -1;
                var elementoY = -1;
                var elementoWidth = -1;
                var elementoHeight = -1;
                int.TryParse(parametros[1], out elementoX);
                int.TryParse(parametros[2], out elementoY);
                int.TryParse(parametros[3], out elementoWidth);
                int.TryParse(parametros[4], out elementoHeight);
                return new CoordenadasDeElementos()
                {
                    X = elementoX,
                    Y = elementoY,
                    Height = elementoHeight,
                    Width = elementoWidth,
                };
            }
            else
            {
                throw new Exception($"Não foi possivel capturar o elemento {elemento}");
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
