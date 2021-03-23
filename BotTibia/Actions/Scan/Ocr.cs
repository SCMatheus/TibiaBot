using System.Drawing;
using IronOcr;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Mat = OpenCvSharp.Mat;

namespace BotTibia.Actions.Scan
{
    public static class Ocr
    {
        public static int IdentificaNumeroEmImg(Bitmap img)
        {
            var Ocr = new IronTesseract();
            IronOcr.Installation.LicenseKey = "IRONOCR.MATHEUSOUZACARDOSO2016.18259-3755AD03FE-HPVE5P-YPGVIBVJUBU5-NZM52ZVSOFQU-RXUJUA27S43K-FFADYK2HER26-KWJAGRZ3H2DT-VKASHY-T7BX5MZHB2B6UA-DEPLOYMENT.TRIAL-UJRH3U.TRIAL.EXPIRES.22.APR.2021";
            Ocr.Configuration.TesseractVersion = TesseractVersion.Tesseract5;
            Ocr.Configuration.WhiteListCharacters = "0123456789O";
            Ocr.Configuration.EngineMode = TesseractEngineMode.TesseractAndLstm;
            Ocr.Configuration.PageSegmentationMode = TesseractPageSegmentationMode.RawLine;
            Ocr.Language = OcrLanguage.English;
            var image = BitmapConverter.ToMat(img);
            var imageGray = new Mat();
            Cv2.CvtColor(image, imageGray, ColorConversionCodes.RGB2GRAY);
            var imageResize = new Mat();
            Cv2.Resize(imageGray, imageResize, new OpenCvSharp.Size(image.Width * 2.5, image.Height * 2.5));
            var imageBinarize = new Mat();
            Cv2.Threshold(imageResize, imageBinarize, 155, 255, ThresholdTypes.BinaryInv);
            using (var Input = new OcrInput(BitmapConverter.ToBitmap(imageBinarize)))
            {

                var Result = Ocr.Read(Input);
                int resultado = 0;
                Result.Text.Replace("O", "0");
                if (int.TryParse(Result.Text, out resultado))
                {
                    return resultado;
                }
                else
                {
                    return -1;
                }
            }
        }
    }
}
