using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shows a preview of what the neural network will review as an input
/// </summary>
public class ImagePreview : MonoBehaviour
{
    /// <summary>
    /// Image used to display the texture
    /// </summary>
    private Image image;
    /// <summary>
    /// Texture displayed on the image
    /// </summary>
    private Texture2D texture;


    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    private void Start()
    {
        image = GetComponent<Image>();
        texture = FindObjectOfType<DrawingCanvas>().GetTexture();
        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}
