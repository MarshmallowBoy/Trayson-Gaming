using UnityEngine;
using UnityEngine.UI;
public class CopySprite : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Image image;
    void Update()
    {
        image.sprite = spriteRenderer.sprite;
    }
}
