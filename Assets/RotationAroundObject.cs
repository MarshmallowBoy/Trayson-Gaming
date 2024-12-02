using UnityEngine;
using UnityEngine.UI;
public class RotationAroundObject : MonoBehaviour
{
    public Transform Target;
    [Range(0, 360)]
    public float Rotation;
    public Vector3 Offset;
    public Slider PreviewSlider;
    void Update()
    {
        Rotation = PreviewSlider.value;
        Target.localRotation = Quaternion.Euler(0, Rotation, 0);
        transform.localPosition = Offset;
    }
}
