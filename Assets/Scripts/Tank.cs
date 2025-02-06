using UnityEngine;

public class Tank : MonoBehaviour
{
    public Rigidbody body;
    public float acceleration;
    public float speed;
    public float angularAcceleration;
    public float angularSpeed;
    public BlendShapesController BSC;
    public float TreadAnimSpeed = 1;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

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

    private void FixedUpdate()
    {
        //Right is one, left is negative one
        BSC.rightProgress += -Input.GetAxis("Horizontal") * TreadAnimSpeed;
        BSC.leftProgress += Input.GetAxis("Horizontal") * TreadAnimSpeed;

        if (Input.GetAxis("Horizontal") == 0)
        {
            BSC.rightProgress += Input.GetAxis("Vertical") * TreadAnimSpeed;
            BSC.leftProgress += Input.GetAxis("Vertical") * TreadAnimSpeed;
        }
        
    }
}
