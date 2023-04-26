using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;

/// <summary>
/// Simple drawing canvas with automatic image processing for handwrite digit classification model
/// </summary>
public class DrawingCanvas : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    /// <summary>
    /// The rect transform of the canvas
    /// </summary>
    private RectTransform rectTransform;
    /// <summary>
    /// The image component of the canvas
    /// </summary>
    private Image image;
    /// <summary>
    /// The texture of the canvas
    /// </summary>
    private Texture2D texture;
    /// <summary>
    /// The texture of the canvas after processing
    /// </summary>
    private Texture2D textureAfterProcessing;
    /// <summary>
    /// The brush size
    /// </summary>
    public int brushSize = 15;

    /// <summary>
    /// Called before Start
    /// </summary>
    private void Awake()
    {
        textureAfterProcessing = new Texture2D(28, 28, TextureFormat.RGBA32, false);
        textureAfterProcessing.filterMode = FilterMode.Point;
        textureAfterProcessing.wrapMode = TextureWrapMode.Clamp;
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        Setup();
    }

    /// <summary>
    /// Sets up the canvas
    /// </summary>
    private void Setup()
    {
        // Create a new texture to use as the canvas
        texture = new Texture2D((int)rectTransform.rect.width, (int)rectTransform.rect.height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        Color[] pixels = new Color[texture.width * texture.height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }
        texture.SetPixels(pixels);
        texture.Apply();
        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        pixels = textureAfterProcessing.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }
        textureAfterProcessing.SetPixels(pixels);
        textureAfterProcessing.Apply();
    }

    /// <summary>
    /// Clears the canvas
    /// </summary>
    public void Clear()
    {
        Color[] pixels;
        Color clearColor = new Color(1, 1, 1, 1);

        pixels = texture.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = clearColor;
        }
        texture.SetPixels(pixels);
        texture.Apply();

        pixels = textureAfterProcessing.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = clearColor;
        }
        textureAfterProcessing.SetPixels(pixels);
        textureAfterProcessing.Apply();
    }

    /// <summary>
    /// Handles on mouse dawn
    /// </summary>
    /// <param name="eventData">Event data</param>
    public void OnPointerDown(PointerEventData eventData)
    {
        // Get the position of the user's pointer on the canvas
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            // Draw a line between the previous and current pointer positions
            Vector2 previousPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position - eventData.delta, eventData.pressEventCamera, out previousPoint))
            {
                int startX = Mathf.RoundToInt(previousPoint.x + texture.width / 2f);
                int startY = Mathf.RoundToInt(previousPoint.y + texture.height / 2f);
                int endX = Mathf.RoundToInt(localPoint.x + texture.width / 2f);
                int endY = Mathf.RoundToInt(localPoint.y + texture.height / 2f);
                DrawLine(startX, startY, endX, endY, Color.black);
            }
        }
    }

    /// <summary>
    /// Handles on drag event
    /// </summary>
    /// <param name="eventData">Event data</param>
    public void OnDrag(PointerEventData eventData)
    {
        // Get the position of the user's pointer on the canvas
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            // Draw a line between the previous and current pointer positions
            Vector2 previousPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position - eventData.delta, eventData.pressEventCamera, out previousPoint))
            {
                int startX = Mathf.RoundToInt(previousPoint.x + texture.width / 2f);
                int startY = Mathf.RoundToInt(previousPoint.y + texture.height / 2f);
                int endX = Mathf.RoundToInt(localPoint.x + texture.width / 2f);
                int endY = Mathf.RoundToInt(localPoint.y + texture.height / 2f);
                DrawLine(startX, startY, endX, endY, Color.black);               
            }
        }
    }

    /// <summary>
    /// Draws a line using a brush
    /// </summary>
    /// <param name="x0">X coordinate of starting position</param>
    /// <param name="y0">Y coordinate of starting position</param>
    /// <param name="x1">X coordinate of end position</param>
    /// <param name="y1">Y coordinate of end position</param>
    /// <param name="color">Color for the line</param>
    private void DrawLine(int x0, int y0, int x1, int y1, Color color)
    {
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            DrawBrush(x0, y0, brushSize, color);

            if (x0 == x1 && y0 == y1)
            {
                break;
            }

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }

        texture.Apply();
        Processtexture();
    }

    /// <summary>
    /// Draws a brush, instead of a single pixel
    /// </summary>
    /// <param name="x">X of the brush center</param>
    /// <param name="y">Y of the brush center</param>
    /// <param name="size">Size of the brush</param>
    /// <param name="color">Color of the brush</param>
    private void DrawBrush(int x, int y, int size, Color color)
    {
        int radius = size / 2;
        int diameter = radius * 2;

        for (int drawX = x - radius; drawX <= x + radius; drawX++)
        {
            for (int drawY = y - radius; drawY <= y + radius; drawY++)
            {
                int distanceX = Mathf.Abs(x - drawX);
                int distanceY = Mathf.Abs(y - drawY);
                float distance = Mathf.Sqrt(Mathf.Pow(distanceX, 2) + Mathf.Pow(distanceY, 2));
                if (distance <= radius)
                {
                    if (drawX >= 0 && drawX < texture.width && drawY >= 0 && drawY < texture.height)
                    {
                        texture.SetPixel(drawX, drawY, color);
                    }
                }
            }
        }

        texture.Apply();
    }

    /// <summary>
    /// Saves preprocessed and process image data (mostly for debugging purposes)
    /// </summary>
    /// <param name="fileName"></param>
    public void SaveCanvas(string fileName)
    {
        File.WriteAllBytes(Application.dataPath + "/" + fileName + "b.png", texture.EncodeToPNG());
        File.WriteAllBytes(Application.dataPath + "/" + fileName + "a.png", textureAfterProcessing.EncodeToPNG());
    }

    /// <summary>
    /// Process the input texture to one that the neural network will take as an input
    /// </summary>
    private void Processtexture()
    {
        float[,] data = 
            ImgPro.Util.ResizeDigit(
                ImgPro.Util.ReversPolarity(
                    ImgPro.Util.Centre(
                        ImgPro.Util.ReversPolarity(
                            ImgPro.Util.TextureToRaw(texture)))));
        if (data.Length > 0)
        {
            Texture2D newTexture = ImgPro.Util.RawToTexture(data);
            ImgPro.Util.Resize(newTexture, 28);
            //newTexture = ImgPro.Util.RawToTexture(ImgPro.Util.BoxBlure(ImgPro.Util.TextureToRaw(newTexture)));
            textureAfterProcessing.SetPixels(newTexture.GetPixels());
            textureAfterProcessing.Apply();
        }
    }

    /// <summary>
    /// Getter for texture after processing
    /// </summary>
    /// <returns>Texture2D after processing</returns>
    public Texture2D GetTexture()
    {
        return textureAfterProcessing;
    }

    
    /// <summary>
    /// Quites the application
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }
}
