using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenNGS.Extension
{
    public static class ImageExtension
    {

        public static bool IsBlack(this Color color)
        {
            return (color.r + color.g + color.b) == 0;
        }

        public static bool IsBlack(this Color32 color)
        {
            return (color.r + color.g + color.b) == 0;
        }


        /// <summary>
        ///  Save this texture into JPG format file.
        /// </summary>
        /// <param name="tex">Text texture to convert.</param>
        /// <param name="file">The filename to save</param>
        /// <param name="flags"></param>
        public static void SaveToEXR(this Texture2D tex, string file, Texture2D.EXRFlags flags)
        {
            File.WriteAllBytes(file, tex.EncodeToEXR(flags));
        }

        /// <summary>
        ///  Encodes this texture into JPG format.
        /// </summary>
        /// <param name="tex">Text texture to convert.</param>
        /// <param name="file">The filename to save</param>
        /// <param name="quality">JPG quality to encode with, 1..100 (default 75).</param>
        /// <returns></returns>
        public static void SaveToJPG(this Texture2D tex, string file, int quality)
        {
            File.WriteAllBytes(file, tex.EncodeToJPG(quality));
        }

        /// <summary>
        /// Encodes this texture into PNG format.
        /// </summary>
        /// <param name="tex">The texture to convert.</param>
        /// <param name="file">The filename to save</param>
        public static void SaveToPNG(this Texture2D tex, string file)
        {

            File.WriteAllBytes(file, tex.EncodeToPNG());
        }

        /// <summary>
        /// Encodes the specified texture in TGA format.
        /// </summary>
        /// <param name="tex">The texture to encode.</param>
        /// <param name="file">The filename to save</param>
        /// <returns></returns>
        public static void SaveToTGA(this Texture2D tex, string file)
        {
            File.WriteAllBytes(file, tex.EncodeToTGA());
        }


        public static void LoadTGA(this Texture2D tex, string file)
        {
            using (var TGAStream = File.OpenRead(file))
            {
                using (BinaryReader r = new BinaryReader(TGAStream))
                {
                    // Skip some header info we don't care about.
                    // Even if we did care, we have to move the stream seek point to the beginning,
                    // as the previous method in the workflow left it at the end.
                    r.BaseStream.Seek(12, SeekOrigin.Begin);

                    short width = r.ReadInt16();
                    short height = r.ReadInt16();
                    int bitDepth = r.ReadByte();

                    // Skip a byte of header information we don't care about.
                    r.BaseStream.Seek(1, SeekOrigin.Current);
                    if (tex.width != width || tex.height != height)
                    {
#if UNITY_2021_1_OR_NEWER
                        tex.Reinitialize(width, height);
#else
                        tex.Resize(width, height);
#endif
                    }
                    Color32[] pulledColors = new Color32[width * height];

                    if (bitDepth == 32)
                    {
                        for (int i = 0; i < width * height; i++)
                        {
                            byte red = r.ReadByte();
                            byte green = r.ReadByte();
                            byte blue = r.ReadByte();
                            byte alpha = r.ReadByte();

                            pulledColors[i] = new Color32(blue, green, red, alpha);
                        }
                    }
                    else if (bitDepth == 24)
                    {
                        for (int i = 0; i < width * height; i++)
                        {
                            byte red = r.ReadByte();
                            byte green = r.ReadByte();
                            byte blue = r.ReadByte();

                            pulledColors[i] = new Color32(blue, green, red, 1);
                        }
                    }
                    else
                    {
                        throw new Exception("TGA texture had non 32/24 bit depth.");
                    }

                    tex.SetPixels32(pulledColors);
                    tex.Apply();
                }
            }
        }
    }
}
