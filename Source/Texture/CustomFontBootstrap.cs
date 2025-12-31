using System;
using System.IO;
using UnityEngine;
using ImGuiNET;

public sealed class CustomFontBootstrap : MonoBehaviour
{
    [SerializeField] private CustomFontByteData fontData;
    [SerializeField, Range(16, 96)] private int fontSizePx = 48;
    [SerializeField] private int desiredAtlasWidth = 4096;

    private unsafe ImFontConfig* imFontConfig;
    
    public unsafe void OnFontInit(ImGuiIOPtr io)
    {
        if (fontData == null || fontData.FontData.Length == 0)
        {
            io.Fonts.Clear();
            io.Fonts.AddFontDefault();
            return;
        }

        io.Fonts.Clear();
        
        io.Fonts.TexDesiredWidth = desiredAtlasWidth;
        io.Fonts.TexGlyphPadding *= 2; //Double padding to preserve mipmapping problems

        
        imFontConfig = ImGuiNative.ImFontConfig_ImFontConfig();
        imFontConfig->RasterizerMultiply = 0.9f;
        imFontConfig->FontDataOwnedByAtlas = 0;

        fixed (byte* pFont = fontData.FontData)
        {
            io.Fonts.AddFontFromMemoryTTF(
                (IntPtr)pFont,
                fontData.FontData.Length,
                fontSizePx,
                imFontConfig,
                io.Fonts.GetGlyphRangesDefault()
            );
        }
    }

    private unsafe void OnDestroy()
    {
        ImGuiNative.ImFontConfig_destroy(imFontConfig);
    }
    
}