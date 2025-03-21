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
    public ParticleSystem BulletParticle;
    public GameObject SandHitEffect;
    public float Delay;
    public int Damage;
    float nextTimeToFire;
    public Transform[] LeftWheels;
    public Transform[] RightWheels;
    public Camera TankCamera;
    public VehiclePart MainBody;
    public VehiclePart Turret;
    public VehiclePart RightWheel;
    public VehiclePart LeftWheel;
    private void Start()
    {
        if (IsOwner)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        TankCamera.gameObject.SetActive(IsOwner);
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

        //Firing Mechanics
        if (Input.GetButton("Fire1") || Input.GetKey(KeyCode.Space))
        {
            if (nextTimeToFire < Time.time)
            {
                nextTimeToFire = Time.time + Delay;
                FireEffectsServerRpc();
                RaycastHit hit;
                if (Physics.Raycast(TurretGun.position, -TurretGun.forward, out hit))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        hit.transform.GetComponent<Heath>().DoDamage(Damage);
                    }
                    if (hit.transform.CompareTag("Vehicle"))
                    {
                        hit.transform.GetComponent<VehiclePart>().DoDamage(Damage);
                    }
                    if (hit.transform.CompareTag("Terrain"))
                    {
                        GameObject temp = Instantiate(SandHitEffect, hit.point, Quaternion.identity);
                        temp.transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal);
                    }
                }
            }
        }
        /*
        //Defining Target Based On Mouseposition
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit _mouseRay;
        if (Physics.Raycast(ray, out _mouseRay, Mathf.Infinity))
        {
            if (_mouseRay.point != null)
            {
                Target = _mouseRay.point;
            }
        }*/
        Debug.DrawRay(TurretGun.position, -TurretGun.forward * 1000, Color.red);
    }

    [Rpc(SendTo.Server)]
    public void FireEffectsServerRpc()
    {
        FireEffectsClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void FireEffectsClientRpc()
    {
        MuzzleFlash.Play();
        BulletParticle.Play();
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
        MoveTreadPingRpc(true, -Input.GetAxis("Horizontal") * TreadAnimSpeed); //right
        MoveTreadPingRpc(false, Input.GetAxis("Horizontal") * TreadAnimSpeed); //left

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
            MoveTreadPingRpc(true, Input.GetAxis("Vertical") * TreadAnimSpeed); //right
            MoveTreadPingRpc(false, Input.GetAxis("Vertical") * TreadAnimSpeed); //left

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

    [Rpc(SendTo.Server)]
    void MoveTreadPingRpc(bool Right, float Increment)
    {
        MoveTreadPongRpc(Right, Increment);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void MoveTreadPongRpc(bool Right, float Increment)
    {
        if (Right)
        {
            BSC.rightProgress += Increment;
        }
        else
        {
            BSC.leftProgress += Increment;
        }
    }

    void TurretMovement()
    {
        //Turret Gun Converting 360 to 180 -180
        Vector3 angles = TurretGun.localEulerAngles;
        angles.x = (angles.x > 180) ? angles.x - 360 : angles.x;

        if (Input.GetKey(KeyCode.UpArrow) && angles.x < ClampThresholdTurretGun.y)
        {
            TurretGun.localEulerAngles += Vector3.right;
        }
        if (Input.GetKey(KeyCode.DownArrow) && angles.x > ClampThresholdTurretGun.x)
        {
            TurretGun.localEulerAngles -= Vector3.right;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            TurretBody.localEulerAngles += Vector3.up;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            TurretBody.localEulerAngles -= Vector3.up;
        }


        /*
        //TurretBody Movement
        TurretBody.Rotate(0, TurretBody.InverseTransformPoint(Target).x * turretSpeed, 0);

        //Turret Gun Converting 360 to 180 -180
        Vector3 angles = TurretGun.localEulerAngles;
        angles.x = (angles.x > 180) ? angles.x - 360 : angles.x;

        //Clamping Turret Gun movement
        if (angles.x < -40)
        {
            //TurretGun.Rotate(0.1f, 0, 0);
            TurretGun.localEulerAngles = new Vector3(-40, 90, 0);
            //return;
        }
        if (angles.x > 40)
        {
            //TurretGun.Rotate(-0.1f, 0, 0);
            TurretGun.localEulerAngles = new Vector3(40, 90, 0);
            //return;
        }

        //Moving Turret Gun
        TurretGun.Rotate(TurretGun.InverseTransformPoint(Target).y, 0, 0);
        */
    }
}
