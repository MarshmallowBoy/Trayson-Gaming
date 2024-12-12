using UnityEngine;
using UnityEngine.UI;

public class DamageNotif : MonoBehaviour
{
    public Transform Image;
    public Transform Target;
    public Camera Camera;
    void Update()
    {
        if (Image != null && Target != null)
        {
            Image.transform.position = Camera.WorldToScreenPoint(Target.position);
        }
    }
}
