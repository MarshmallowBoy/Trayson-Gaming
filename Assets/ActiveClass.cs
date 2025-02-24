using UnityEngine;
using UnityEngine.UI;
public class ActiveClass : MonoBehaviour
{
    public Image IconImage;
    public Transform IconTransform;

    public Sprite[] ClassSprites;
    public Vector3[] Rotation;
    public Vector3[] Scale;

    public void LoadIcon(int Index)
    {
        IconImage.sprite = ClassSprites[Index];
        IconTransform.localEulerAngles = Rotation[Index];
        IconTransform.localScale = Scale[Index];
    }
}
