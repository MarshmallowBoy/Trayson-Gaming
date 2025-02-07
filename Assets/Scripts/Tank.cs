using UnityEngine;

public class Tank : MonoBehaviour
{
    public Rigidbody body;
    public Transform TurretBody;
    public Transform TurretGun;
    public Vector2 ClampThresholdTurretGun;
    Vector3 Target;
    public float acceleration;
    public float speed;
    public float angularAcceleration;
    public float angularSpeed;
    public float turretSpeed;
    public BlendShapesController BSC;
    public float TreadAnimSpeed = 1;
    public ParticleSystem MuzzleFlash;
    public float Delay;
    float nextTimeToFire;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
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

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit _mouseRay;
        if (Physics.Raycast(ray, out _mouseRay, Mathf.Infinity))
        {
            if (_mouseRay.point != null)
            {
                Target = _mouseRay.point;
            }
        }

        if (Input.GetButton("Fire1") && nextTimeToFire < Time.time)
        {
            nextTimeToFire = Time.time + Delay;
            RaycastHit hit;
            if (Physics.Raycast(TurretGun.position, -TurretGun.forward, out hit))
            {
                MuzzleFlash.Play();
            }
        }

        Debug.DrawRay(TurretGun.position, -TurretGun.forward * 1000, Color.red);
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


        
        TurretBody.Rotate(0, TurretBody.InverseTransformPoint(Target).x * turretSpeed, 0);
        TurretGun.Rotate(TurretGun.InverseTransformPoint(Target).y, 0, 0);
        Debug.Log(TurretGun.InverseTransformPoint(Target).y);
    }
}
