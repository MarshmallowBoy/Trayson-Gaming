using UnityEngine;

public class Tank : MonoBehaviour
{
    public Rigidbody body;
    public float acceleration;
    public float speed;
    public float angularAcceleration;
    public float angularSpeed;
    void Update()
    {
        if (body.linearVelocity.magnitude < speed && Input.GetAxis("Horizontal") == 0)
        {
            body.linearVelocity += body.transform.forward * acceleration * Input.GetAxis("Vertical");
        }
        if (body.angularVelocity.magnitude < angularSpeed)
        {
            body.angularVelocity += new Vector3(0, Input.GetAxis("Horizontal") * angularAcceleration, 0);
        }
    }
}
