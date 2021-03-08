﻿using System.IO;
using AutoHotkey.Interop;
using BotTibia.Classes;

namespace BotTibia.Actions.AHK
{
    public class AhkFunctions
    {
        public void LoadScripts()
        {
            GdipLib();
            PegaElementosDaTela();
            PegaCoordenadasDoPersonagem();
        }
        private void GdipLib()
        {
            var _ahkEngine = AutoHotkeyEngine.Instance;
            _ahkEngine.LoadFile(Global._caminho + "\\Actions\\AHK\\Gdip_All_2.ahk");
        }
        private void PegaElementosDaTela()
        {
            var _ahkEngine = AutoHotkeyEngine.Instance;
            string Function = "PegaElementosAhk(x, y, largura, altura,item, hwnd){"
                            + "\r\n If !pToken:= Gdip_Startup()                                                                          "
                            + "\r\n {                                                                                                   "
                            + "\r\n   MsgBox, 48, gdiplus error!, Gdiplus failed to start. Please ensure you have gdiplus on your system"
                            + "\r\n   ExitApp                                                                                           "
                            + "\r\n }                                                                "
                            + "\r\n bmpHaystack := Gdip_BitmapFromHWND(hwnd)                                                     "
                            + "\r\n bmpNeedle:= Gdip_CreateBitmapFromFile(item)                                                     "
                            + "\r\n width:= Gdip_GetImageWidth(bmpNeedle), height:= Gdip_GetImageHeight(bmpNeedle)                      "
                            + "\r\n RET:= Gdip_ImageSearch(bmpHaystack, bmpNeedle, OutputList, x, y, largura, altura, 0, 0xFFFFFF)                 "
                            + "\r\n Gdip_DisposeImage(bmpHaystack)                                                                      "
                            + "\r\n Gdip_DisposeImage(bmpNeedle)                                                                        "
                            + "\r\n Gdip_Shutdown(pToken)                                                                        "
                            + "\r\n if (RET)                                                                                            "
                            + "\r\n {                                                                                       "
                            + "\r\n   StringSplit, LISTArray, OutputList, `,                                              "
                            + "\r\n   vx:= LISTArray1                                                                                   "
                            + "\r\n   vy:= LISTArray2                                                                                   "
                            + "\r\n } else {"
                            + "\r\n return 0"
                            + "\r\n }"
                            + "\r\n virgula := \",\" "
                            + "\r\n retorno = %RET%%virgula%%vx%%virgula%%vy%%virgula%%width%%virgula%%height%"
                            + "\r\n return %retorno%"
                            + "\r\n }";
            _ahkEngine.ExecRaw(Function);
        }
        private void PegaCoordenadasDoPersonagem()
        {
            var _ahkEngine = AutoHotkeyEngine.Instance;
            string Function = "PegaCoordenadasDoPersonagemAhk(x, y, largura, altura,map, hwnd){"
                            + "\r\n If !pToken:= Gdip_Startup()                                                                          "
                            + "\r\n {                                                                                                   "
                            + "\r\n   MsgBox, 48, gdiplus error!, Gdiplus failed to start. Please ensure you have gdiplus on your system"
                            + "\r\n   ExitApp                                                                                           "
                            + "\r\n }                                                                "
                            + "\r\n Loop 2 {                                                   "
                            + "\r\n     tela := Gdip_BitmapFromHWND(hwnd)                                                     "
                            + "\r\n     bmpNeedle := Gdip_CropImage(tela, x+3, y+3, largura-7, altura-7)                                                     "
                            + "\r\n     bmpHaystack:= Gdip_CreateBitmapFromFile(map)                                                     "
                            + "\r\n     Gdip_SaveBitmapToFile(bmpNeedle, \"teste1.png\")                                                     "
                            + "\r\n     Gdip_SaveBitmapToFile(bmpHaystack, \"teste2.png\")                                                     "
                            + "\r\n     Width:= Gdip_GetImageWidth( bmpNeedle ), Height:= Gdip_GetImageHeight( bmpNeedle )                    "
                            + "\r\n     Gdip_SetBitmapTransColor(bmpNeedle, 0xFFFFFF)                   "
                            + "\r\n     if(A_Index > 1){                  "
                            + "\r\n         Gdip_SetBitmapTransColor(bmpNeedle, 0x999999)                 "
                            + "\r\n     }                 "
                            + "\r\n     RET := Gdip_ImageSearch(bmpHaystack,bmpNeedle, OutputList, 0, 0, 0, 0, 0, 0xff3300)               "
                            + "\r\n     Gdip_DisposeImage(bmpHaystack)                                                                      "
                            + "\r\n     Gdip_DisposeImage(tela)                                                                      "
                            + "\r\n     Gdip_DisposeImage(bmpNeedle)                                                                        "
                            + "\r\n     if(RET){                                                                       "
                            + "\r\n         break                                                                       "
                            + "\r\n     }                                                                       "
                            + "\r\n }                                                                       "
                            + "\r\n Gdip_Shutdown(pToken)                                                                        "
                            + "\r\n if (RET)                                                                                            "
                            + "\r\n {                                                                                       "
                            + "\r\n   StringSplit, LISTArray, OutputList, `,                                              "
                            + "\r\n   vx:=(LISTArray1 + (Width//2)) + 31744                                                                                 "
                            + "\r\n   vy:=(LISTArray2 + (Height//2)) + 30976                                                                                 "
                            + "\r\n } else {"
                            + "\r\n return 0"
                            + "\r\n }"
                            + "\r\n virgula := \",\" "
                            + "\r\n retorno = %RET%%virgula%%vx%%virgula%%vy% "
                            + "\r\n return %retorno%"
                            + "\r\n }";
            _ahkEngine.ExecRaw(Function);
        }
    }
}