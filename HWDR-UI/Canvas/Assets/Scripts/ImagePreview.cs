using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImagePreview : MonoBehaviour
{
    private RectTransform rectTransform;
    private Image image;
    private Texture2D texture;
    private Texture2D previewOf;

    public void Setup()
    {
        // Create a new texture to use as the canvas
        texture = new Texture2D(28, 28, TextureFormat.RGBA32, false);
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

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        previewOf = FindObjectOfType<DrawingCanvas>().GetTexture();
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        float[,] data = ImgPro.Util.ResizeDigit(ImgPro.Util.Centre(ImgPro.Util.TextureToRaw(previewOf)));
     
        if (data.Length > 0)
        {
            Texture2D newTexture = ImgPro.Util.RawToTexture(data);
            ImgPro.Util.Resize(newTexture, 28);
            texture.SetPixels(newTexture.GetPixels());
            texture.Apply();
        }
    }
}
