using System;
using UnityEngine;

namespace ImgPro
{
    /// <summary>
    /// Utilities for image processing
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Centers the given image based on the black pixel density
        /// </summary>
        /// <param name="img">Original image to be centered</param>
        /// <returns>Centered copy of the original image</returns>
        public static float[,] Centre(float[,] img)
        {
            int size = (int)Math.Sqrt(img.Length);
            float xC = 0;
            float yC = 0;
            float t = 0;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    t += img[x, y];
                    xC += x * img[x, y];
                    yC += y * img[x, y];
                }
            }
            xC = (xC / t) - (size / 2);
            yC = (yC / t) - (size / 2);

            float[,] translatedData = new float[size, size];
            for (int x = Math.Max(0, (int)xC); x < Math.Min(size, size + (int)xC); x++)
            {
                for (int y = Math.Max(0, (int)yC); y < Math.Min(size, size + (int)yC); y++)
                {
                    translatedData[x - (int)xC, y - (int)yC] = img[x, y];
                }
            }

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    translatedData[x, y] = -(translatedData[x, y] - 1);
                }
            }

            return translatedData;
        }

        public static float[,] Blure(float[,] img)
        {
            return null;
        }

        public static float[,] ResizeDigit(float[,] img)
        {
            return null;
        }

        public static Texture2D Resize(Texture2D texture, int res)
        {
            return null;
        }

        /// <summary>
        /// Converts a Unity Texture2D to a raw data array 
        /// </summary>
        /// <param name="texture">Unity Texture2D to be converted</param>
        /// <returns>Unity Texture2D represented as a raw data array</returns>
        public static float[,] TextureToRaw(Texture2D texture)
        {
            Color[] bytes = texture.GetPixels();

            int size = (int)Mathf.Sqrt(bytes.Length);
            float[,] data = new float[size, size];
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    data[x, y] = -(bytes[x * size + y].grayscale - 1);
                }
            }

            return data;
        }

        /// <summary>
        /// Converts raw data array to a Unity Texture2D
        /// </summary>
        /// <param name="img">Raw array image data</param>
        /// <returns>Unity Texture2D with the raw array image data loaded</returns>
        public static Texture2D RawToTexture(float[,] img)
        {
            int size = (int)Math.Sqrt(img.Length);
            Color[] data = new Color[img.Length];

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    data[x * size + y] = new Color(img[x, y], img[x, y], img[x, y]);
                }
            }

            Texture2D texture = new Texture2D(size, size, TextureFormat.ARGB32, false);
            texture.SetPixels(data);
            return texture;
        }
    }
}