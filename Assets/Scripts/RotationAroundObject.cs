using UnityEngine;
using UnityEngine.UI;
public class RotationAroundObject : MonoBehaviour
{
    public Transform Target;
    [Range(0, 360)]
    public float Rotation;
    public Vector3 Offset;
    public Slider PreviewSlider;
    float defaultY;
    private void Start()
    {
        defaultY = Offset.y;
    }

    void Update()
    {
        Rotation = PreviewSlider.value;
        Target.localRotation = Quaternion.Euler(0, Rotation, 0);
        transform.localPosition = Offset;
        if (Input.GetKey(KeyCode.LeftControl))
        {
            Offset.y = defaultY -1.35f;
        }
        else
        {
            Offset.y = defaultY;
        }
    }
}
