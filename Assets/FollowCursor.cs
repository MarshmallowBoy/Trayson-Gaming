using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    public Vector2 MaxRotation = new Vector2(1.5f, 1f);
    public Vector2 Offset = new Vector2(0, 0);
    void Update()
    {
        float mouseXOffset = Input.mousePosition.x - Screen.width / 2;
        float mouseYOffset = Input.mousePosition.y - Screen.height / 2;
        transform.eulerAngles = new Vector3(((mouseYOffset / Screen.height) * 2 * MaxRotation.y) + Offset.x, ((-(mouseXOffset/Screen.width)*2)*MaxRotation.x) + Offset.y, 0);
    }
}
