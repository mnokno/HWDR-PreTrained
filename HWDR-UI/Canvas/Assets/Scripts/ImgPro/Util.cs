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

        /// <summary>
        /// Applies box bluer filter
        /// </summary>
        /// <param name="img">Image to bluer</param>
        /// <param name="blureSize">Size of the box, has to be odd</param>
        /// <param name="centerWeight">Weight that the center of the filer will have relative to </param>
        /// <returns>New bluer copy of the image</returns>
        public static float[,] BoxBlure(float[,] img, int blureSize = 3, int centerWeight = 3)
        {
            Debug.Assert(blureSize % 2 == 1, $"blureSize has to be odd, {blureSize} was given which is even =!");
            int size = (int)Math.Sqrt(img.Length);
            int padd = blureSize / 2;
            int filterArea = blureSize * blureSize;
            float[,] blured = new float[size, size];
            centerWeight = centerWeight - 1;
            for (int x = padd; x < size - padd; x++)
            {
                for (int y = padd; y < size - padd; y++)
                {
                    float total = 0;
                    for (int xf = -padd; xf < blureSize - padd; xf++)
                    {
                        for (int yf = -padd; yf < blureSize - padd; yf++)
                        {
                            total += img[x + xf, y + yf];
                        }
                    }
                    blured[x, y] = (total + img[x, y] * centerWeight) / (float)(filterArea + centerWeight);
                }
            }
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    blured[x, y] = -(blured[x, y] - 1);
                }
            }
            return blured;
        }

        /// <summary>
        /// Resizes the image so that digits max dimensions spans 65% of the image restitution
        /// </summary>
        /// <param name="img">Image data to size</param>
        /// <returns>Sized copy of the original image data</returns>
        public static float[,] ResizeDigit(float[,] img)
        {
            int size = (int)Math.Sqrt(img.Length);
            int xC = size / 2;
            int yC = size / 2;

            int xUpper = 0;
            for (int x = xC; x >= 0;)
            {
                bool interception = false;
                for (int y = 0; y < size; y++)
                {
                    if (img[x, y] != 1)
                    {
                        interception = true;
                        break;
                    } 
                }
                if (interception)
                {
                    x--;
                    continue;
                }
                xUpper = x;
                break;
            }

            int xLower = size - 1;
            for (int x = xC; x < size;)
            {
                bool interception = false;
                for (int y = 0; y < size; y++)
                {
                    if (img[x, y] != 1)
                    {
                        interception = true;
                        break;
                    }
                }
                if (interception)
                {
                    x++;
                    continue;
                }
                xLower = x;
                break;
            }

            int yUpper = 0;
            for (int y = yC; y >= 0;)
            {
                bool interception = false;
                for (int x = 0; x < size; x++)
                {
                    if (img[x, y] != 1)
                    {
                        interception = true;
                        break;
                    }
                }
                if (interception)
                {
                    y--;
                    continue;
                }
                yUpper = y;
                break;
            }

            int yLower = size - 1;
            for (int y = yC; y < 300;)
            {
                bool interception = false;
                for (int x = 0; x < size; x++)
                {
                    if (img[x, y] != 1)
                    {
                        interception = true;
                        break;
                    }
                }
                if (interception)
                {
                    y++;
                    continue;
                }
                yLower = y;
                break;
            }

            int maxDim = Math.Max(xLower - xUpper, yLower - yUpper);
            int desiredSize = (int)((maxDim / 11f) * 17);

            int xDc = (xLower + xUpper) / 2;
            int yDc = (yLower + yUpper) / 2;
            float[,] data = new float[desiredSize, desiredSize];
            for (int x = 0; x < desiredSize; x++)
            {
                for (int y = 0; y < desiredSize; y++)
                {
                    int xT = x + xDc - desiredSize / 2;
                    int yT = y + yDc - desiredSize / 2;
                    if (xT < 0 || yT < 0 || xT > size - 1 || yT > size - 1)
                    {
                        data[x, y] = 1;
                    }
                    else
                    {
                        data[x, y] = img[xT, yT];
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Resize the given Texture2D in place to given restitution
        /// </summary>
        /// <param name="texture">Texture2D to be resized</param>
        /// <param name="resolution">New resolution for the texture</param>
        public static void Resize(Texture2D texture, int resolution)
        {
            TextureScale.Scale(texture, resolution, resolution);
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