using UnityEngine;
public class ReportTransform : MonoBehaviour
{
    public bool[] Position;
    public bool[] Rotation;
    public bool[] Scale;
    public bool[] Direction;
    void Update()
    {
        if (Position.Length > 0)
        {
            ReportPosition();
        }
        if (Direction.Length > 0)
        {
            ReportDirection();
        }
    }

    public void ReportPosition()
    {
        if (Position[0])
        {
            Debug.Log("X: " + transform.position.x);
        }
        if (Position[1])
        {
            Debug.Log("Y: " + transform.position.y);
        }
        if (Position[2])
        {
            Debug.Log("Z: " + transform.position.z);
        }
    }

    public void ReportDirection()
    {
        if (Direction[0])
        {
            Debug.Log(transform.forward);
        }
        if (Direction[1])
        {
            Debug.Log(transform.right);
        }
        if (Direction[2])
        {
            Debug.Log(transform.up);
        }
    }
}
