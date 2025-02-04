using UnityEngine;

public class BlendShapesController : MonoBehaviour
{
    public SkinnedMeshRenderer RightTread;
    public SkinnedMeshRenderer LeftTread;
    public float rightProgress = 0;
    public float leftProgress = 0;
    void FixedUpdate()
    {
        RightTread.SetBlendShapeWeight(0, Mathf.Clamp(rightProgress, 0, 100));
        RightTread.SetBlendShapeWeight(1, Mathf.Clamp(rightProgress - 100, 0, 100));
        RightTread.SetBlendShapeWeight(2, Mathf.Clamp(rightProgress - 200, 0, 100));
        RightTread.SetBlendShapeWeight(3, Mathf.Clamp(rightProgress - 300, 0, 100));

        if (rightProgress > 400){ rightProgress = 1; }

        if (rightProgress <= 0){ rightProgress = 400; }

        LeftTread.SetBlendShapeWeight(0, Mathf.Clamp(leftProgress, 0, 100));
        LeftTread.SetBlendShapeWeight(1, Mathf.Clamp(leftProgress - 100, 0, 100));
        LeftTread.SetBlendShapeWeight(2, Mathf.Clamp(leftProgress - 200, 0, 100));
        LeftTread.SetBlendShapeWeight(3, Mathf.Clamp(leftProgress - 300, 0, 100));

        if (leftProgress > 400){ leftProgress = 1; }

        if(leftProgress <= 0){ leftProgress = 400; }
    }
}
