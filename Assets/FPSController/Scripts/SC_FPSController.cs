using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Networking;
using Dissonance.Audio.Capture;
using Dissonance;
using System;
[RequireComponent(typeof(CharacterController))]

public class SC_FPSController : NetworkBehaviour
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    public float resistance;
    public float forceLimit;
    public bool YourOnThinIcePal;

    public GameObject PenguinBody;

    CharacterController characterController;
    public Vector3 moveDirection = Vector3.zero;
    public Vector3 ExternalVector = Vector3.zero;
    float rotationX = 0;
    bool MovingLastFrame = false;
    public Vector3 moveLastDirection = Vector3.zero;

    [HideInInspector]
    public bool canMove = true;

    public Renderer[] PlayerModel;
    public GameObject HatMenu;
    public GameObject SuitMenu;
    public GameObject TrailsMenu;
    public GameObject HealthBar;
    public GameObject HealthCamera;
    public GameObject DamageVignette;
    public Renderer[] Outline;
    public Animator animator;
    public Animator animator2;
    public GameObject Weapons;
    public GameObject Preview;
    public GameObject PreviewCamera;
    public bool MenuEnabled = true;
    public GameObject Console;
    public bool BellyMode;
    public float bellyspeed;
    public GameObject NameTag;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (IsOwner)
        {
            // Lock cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        playerCamera.gameObject.SetActive(IsOwner);
        HealthBar.SetActive(IsOwner);
        HealthCamera.SetActive(IsOwner);
        DamageVignette.SetActive(IsOwner);
        PreviewCamera.SetActive(IsOwner);
        NameTag.SetActive(!IsOwner);
        foreach(var O in Outline)
        {
            O.enabled = true;
        }
        if (IsOwner)
        {
            foreach (var m in PlayerModel)
            {
                //m.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                m.gameObject.layer = 6;
            }
        }
    }

    public void SetPosition(Vector3 position)
    {
        characterController.enabled = false;
        transform.position = position;
        characterController.enabled = true;
    }
    void Update()
    {
        if (IsOwner)
        {
            YourOnThinIcePal = false;
            RaycastHit _hit;
            /*
            if (Physics.Raycast(transform.position, Vector3.down, out _hit, 0.1f))
            {
                if (_hit.transform.CompareTag("Ice"))
                {
                    YourOnThinIcePal = true;
                }
            }*/

            if (Physics.Raycast(transform.position, Vector3.down, out _hit, 0.5f))
            {
                if (_hit.transform.CompareTag("Ice"))
                {
                    YourOnThinIcePal = true;
                }
            }

            if (MenuEnabled)
            {
                if (Input.GetKeyDown(KeyCode.H))
                {
                    HatMenu.SetActive(!HatMenu.activeInHierarchy);
                    //AddHatSendRpc();
                }
                if (Input.GetKeyDown(KeyCode.G))
                {
                    SuitMenu.SetActive(!SuitMenu.activeInHierarchy);
                }
                if (Input.GetKeyDown(KeyCode.T))
                {
                    TrailsMenu.SetActive(!TrailsMenu.activeInHierarchy);
                }
            }

            BellyMode = Input.GetAxis("Slide") > 0;

            if (BellyMode)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f))
                {
                    //ExternalVector += Vector3.forward * gravity * bellyspeed * -(1 - hit.normal.y);
                    ExternalVector += new Vector3(hit.normal.x, -(hit.normal.y), hit.normal.z);
                    PenguinBody.transform.up = new Vector3(hit.normal.x, 0, hit.normal.z + 2f);
                    PenguinBody.transform.localEulerAngles = new Vector3(PenguinBody.transform.localEulerAngles.x, 0, 0);
                }
                
            }
            else
            {
                PenguinBody.transform.localEulerAngles = new Vector3(0, 0, 0);
            }

            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                Console.SetActive(!Console.activeInHierarchy);
                Console.GetComponent<Console>().InputField.ActivateInputField();
                MenuEnabled = !Console.activeInHierarchy;
                HatMenu.SetActive(false);
                SuitMenu.SetActive(false);
                TrailsMenu.SetActive(false);
                if (Console.activeInHierarchy)
                {
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
                Cursor.visible = Console.activeInHierarchy;
                canMove = !Cursor.visible;
                Weapons.SetActive(!Cursor.visible);
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Cursor.visible = !Cursor.visible;
                if (Cursor.visible)
                {
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
                canMove = !Cursor.visible;
                Weapons.SetActive(!Cursor.visible);
                Preview.SetActive(Cursor.visible);
            }

            // We are grounded, so recalculate move direction based on axes
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            // Press Left Shift to run
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
            float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
            float movementDirectionY = moveDirection.y;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);
            Vector3 TrueMoveDirection = new Vector3(Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal"));
            //int Moving = Mathf.RoundToInt(moveDirection.normalized.magnitude);
            bool Moving = Convert.ToBoolean(Mathf.RoundToInt(Input.GetAxis("Vertical"))) || Convert.ToBoolean(Mathf.RoundToInt(Input.GetAxis("Horizontal")));

            animator.SetBool("Running", moveDirection.magnitude > 0);
            animator.SetBool("IsRecordingVoice", GameObject.Find("---DissonanceComms---").GetComponent<VoiceProximityBroadcastTrigger>().IsTransmitting);
            animator2.SetBool("Running", moveDirection.magnitude > 0);

            if (YourOnThinIcePal && Moving)
            {
                ExternalVector = moveDirection;
            }

            if (YourOnThinIcePal) { resistance = 0; }
            else if (BellyMode) { resistance = 0.5f; }
            else { resistance = 3; }

            if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
            {
                moveDirection.y = jumpSpeed;
            }
            else
            {
                moveDirection.y = movementDirectionY;
            }

            // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
            // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
            // as an acceleration (ms^-2)
            if (!characterController.isGrounded)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }

            ExternalVector += -ExternalVector * Time.deltaTime * resistance;
            if (ExternalVector.magnitude <= 0) { ExternalVector = Vector3.zero; }

            // Move the controller
            characterController.Move((moveDirection + ExternalVector) * Time.deltaTime);

            // Player and Camera rotation
            if (canMove)
            {
                rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
                rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
                playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
            }
            MovingLastFrame = Moving;
        }
    }


}