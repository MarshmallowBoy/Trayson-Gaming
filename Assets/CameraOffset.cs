using UnityEngine;

public class CameraOffset : MonoBehaviour
{
    public Vector3 Offset;
    public float defaultY;
    void Start()
    {
        defaultY = transform.localPosition.y;
        Offset = transform.localPosition;
    }
    void Update()
    {
        transform.localPosition = Offset;
        if (Input.GetKey(KeyCode.LeftControl))
        {
            Offset.y = defaultY - 1;
        }
        else
        {
            Offset.y = defaultY;
        }
    }
}
