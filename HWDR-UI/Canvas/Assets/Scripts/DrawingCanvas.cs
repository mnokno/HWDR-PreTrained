using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DrawingCanvas : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private RectTransform rectTransform;
    private Image image;
    private Texture2D texture;
    public int brushSize = 15;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        // Create a new texture to use as the canvas
        texture = new Texture2D((int)rectTransform.rect.width, (int)rectTransform.rect.height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        Color32[] pixels = new Color32[texture.width * texture.height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }
        texture.SetPixels32(pixels);
        texture.Apply();
        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

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
    }

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
}
