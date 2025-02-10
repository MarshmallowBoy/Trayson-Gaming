using UnityEngine;
using Unity.Netcode;
public class Tank : NetworkBehaviour
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
    public float wheelSpeed;
    public BlendShapesController BSC;
    public float TreadAnimSpeed = 1;
    public ParticleSystem MuzzleFlash;
    public float Delay;
    float nextTimeToFire;
    public Transform[] LeftWheels;
    public Transform[] RightWheels;
    private void Start()
    {
        if (IsOwner)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        //Movement Forward
        if (body.linearVelocity.magnitude < speed && Input.GetAxis("Horizontal") == 0)
        {
            body.linearVelocity += body.transform.forward * acceleration * Input.GetAxis("Vertical");
        }
        //Turning
        if (body.angularVelocity.magnitude < angularSpeed)
        {
            body.angularVelocity += new Vector3(0, Input.GetAxis("Horizontal") * angularAcceleration, 0);
        }

        //Defining Target Based On Mouseposition
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit _mouseRay;
        if (Physics.Raycast(ray, out _mouseRay, Mathf.Infinity))
        {
            if (_mouseRay.point != null)
            {
                Target = _mouseRay.point;
            }
        }

        //Firing Mechanics
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
        if (IsOwner)
        {
            TreadMovement();
            TurretMovement();
        }
    }

    void TreadMovement()
    {
        //Right is one, left is negative one
        BSC.rightProgress += -Input.GetAxis("Horizontal") * TreadAnimSpeed;
        BSC.leftProgress += Input.GetAxis("Horizontal") * TreadAnimSpeed;

        //Move Wheels Forward Or Backward
        foreach (Transform g in RightWheels)
        {
            g.Rotate(0, 0, Input.GetAxis("Horizontal") * wheelSpeed);
        }
        foreach (Transform g in LeftWheels)
        {
            g.Rotate(0, 0, -Input.GetAxis("Horizontal") * wheelSpeed);
        }

        //Move Wheels Side
        if (Input.GetAxis("Horizontal") == 0)
        {
            BSC.rightProgress += Input.GetAxis("Vertical") * TreadAnimSpeed;
            BSC.leftProgress += Input.GetAxis("Vertical") * TreadAnimSpeed;

            foreach (Transform g in RightWheels)
            {
                g.Rotate(0, 0, -Input.GetAxis("Vertical") * wheelSpeed);
            }
            foreach (Transform g in LeftWheels)
            {
                g.Rotate(0, 0, -Input.GetAxis("Vertical") * wheelSpeed);
            }
        }
    }

    void TurretMovement()
    {
        //TurretBody Movement
        TurretBody.Rotate(0, TurretBody.InverseTransformPoint(Target).x * turretSpeed, 0);

        //Turret Gun Converting 360 to 180 -180
        Vector3 angles = TurretGun.eulerAngles;
        angles.x = (angles.x > 180) ? angles.x - 360 : angles.x;

        //Clamping Turret Gun movement
        if (angles.x < -40)
        {
            TurretGun.Rotate(0.1f, 0, 0);
            return;
        }
        if (angles.x > 40)
        {
            TurretGun.Rotate(-0.1f, 0, 0);
            return;
        }

        //Moving Turret Gun
        TurretGun.Rotate(TurretGun.InverseTransformPoint(Target).y, 0, 0);
    }
}
