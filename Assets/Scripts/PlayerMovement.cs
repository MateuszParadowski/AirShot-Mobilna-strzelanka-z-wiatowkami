using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpForce = 3.5f;
    public float gravity = -9.81f;

    [Header("Stance")]
    public float standHeight = 1.8f;
    public float crouchHeight = 1.1f;
    public float proneHeight = 0.6f;
    public float standCamY = 1.6f;
    public float crouchCamY = 0.9f;
    public float proneCamY = 0.4f;

    [Header("References")]
    public CharacterController controller;
    public Transform playerCamera;
    public Joystick joystick;
    public LookArea lookArea;

    [Header("Audio")]
    public AudioSource oneShotSource;
    public AudioSource footstepSource;
    public AudioClip jumpSound;
    public AudioClip footstepLoop;

    [Header("Ladder")]
    public float ladderSpeed = 3f;
    public LayerMask ladderLayer;

    Vector3 velocity;
    bool isSprinting;
    bool isOnLadder;
    bool groundedLastFrame;

    float xRotation;
    float moveX;
    float moveZ;

    Vector3 lastPosition;
    bool wasMovingLastFrame;

    bool moved;
    bool jumped;
    bool usedLadder;
    bool climbedWall;

    public bool HasMoved() => moved;
    public bool HasJumped() => jumped;
    public bool HasUsedLadder() => usedLadder;
    public bool HasClimbedWall() => climbedWall;
    public bool IsCrouching() => currentStance == Stance.Crouch;
    public bool IsProne() => currentStance == Stance.Prone;
    public bool IsSprinting() => isSprinting;

    // ===== STANCE =====
    public enum Stance { Stand, Crouch, Prone }
    Stance currentStance = Stance.Stand;
    public float doubleTapTime = 0.3f;
    float lastCrouchTap;

    // ===== CLIMB LEDGE =====
    bool canClimbLedge;
    bool isClimbingLedge;
    Transform currentClimbTrigger;

    void Update()
    {
        lastPosition = transform.position;

        if (isClimbingLedge) return;

        ReadMovementInput();
        Look();
        Move();
        ApplyGravity();
        HandleFootsteps();
        PCShortcuts();
    }

    // ================= INPUT =================
    void ReadMovementInput()
    {
        moveX = Application.isMobilePlatform ? joystick.Horizontal : Input.GetAxis("Horizontal");
        moveZ = Application.isMobilePlatform ? joystick.Vertical : Input.GetAxis("Vertical");
    }

    // ================= MOVE =================
    void Move()
    {
        if (isOnLadder)
        {
            float climb = Application.isMobilePlatform ? joystick.Vertical : Input.GetAxis("Vertical");
            controller.Move(Vector3.up * climb * ladderSpeed * Time.deltaTime);
            return;
        }

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        float speed = isSprinting ? sprintSpeed : walkSpeed;
        controller.Move(move * speed * Time.deltaTime);
    }

    // ================= LOOK =================
    void Look()
    {
        float lookX = Application.isMobilePlatform
            ? lookArea.lookDelta.x * 0.2f
            : Input.GetAxis("Mouse X") * 150f * Time.deltaTime;

        float lookY = Application.isMobilePlatform
            ? lookArea.lookDelta.y * 0.2f
            : Input.GetAxis("Mouse Y") * 150f * Time.deltaTime;

        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * lookX);
    }

    // ================= GRAVITY =================
    void ApplyGravity()
    {
        if (isOnLadder || isClimbingLedge) return;

        bool groundedNow = controller.isGrounded;

        if (groundedNow && !groundedLastFrame)
            velocity.y = -2f;

        groundedLastFrame = groundedNow;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // ================= JUMP / CLIMB =================
    public void JumpButton()
    {
        if (canClimbLedge && !isClimbingLedge)
        {
            StartCoroutine(ClimbLedge());
            return;
        }

        if (isOnLadder)
        {
            isOnLadder = false;
            velocity.y = jumpForce;
            return;
        }

        if (!controller.isGrounded || currentStance == Stance.Prone) return;

        velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);

        if (jumpSound && oneShotSource)
            oneShotSource.PlayOneShot(jumpSound);
    }

    IEnumerator ClimbLedge()
    {
        if (!currentClimbTrigger) yield break;

        isClimbingLedge = true;
        controller.enabled = false;

        Vector3 target = currentClimbTrigger.position;
        target.y += controller.height * 0.5f;

        transform.position = target;

        yield return new WaitForSeconds(0.1f);

        controller.enabled = true;

        // 🔥 reset krawędzi
        canClimbLedge = false;
        currentClimbTrigger = null;

        isClimbingLedge = false;
    }

    // ================= FOOTSTEPS =================
    void HandleFootsteps()
    {
        float realMovement = (transform.position - lastPosition).magnitude;

        bool isMoving =
            controller.isGrounded &&
            !isOnLadder &&
            !isClimbingLedge &&
            realMovement > 0.01f;

        if (isMoving && !wasMovingLastFrame)
        {
            footstepSource.clip = footstepLoop;
            footstepSource.loop = true;
            footstepSource.pitch = isSprinting ? 1.25f : 1f;
            footstepSource.Play();
        }
        else if (!isMoving && wasMovingLastFrame)
        {
            footstepSource.Stop();
        }

        wasMovingLastFrame = isMoving;
    }

    // ================= STANCE =================
    public void CrouchButton()
    {
        if (Time.time - lastCrouchTap <= doubleTapTime)
            SetStance(currentStance == Stance.Prone ? Stance.Stand : Stance.Prone);
        else
            SetStance(currentStance == Stance.Stand ? Stance.Crouch : Stance.Stand);

        lastCrouchTap = Time.time;
    }

    void SetStance(Stance stance)
    {
        currentStance = stance;

        float h = standHeight;
        float camY = standCamY;

        if (stance == Stance.Crouch) { h = crouchHeight; camY = crouchCamY; }
        if (stance == Stance.Prone) { h = proneHeight; camY = proneCamY; }

        controller.height = h;
        controller.center = new Vector3(0, h / 2f, 0);

        Vector3 pos = playerCamera.localPosition;
        pos.y = camY;
        playerCamera.localPosition = pos;
    }

    // ================= TRIGGERS =================
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("ClimbTrigger"))
        {
            canClimbLedge = true;
            currentClimbTrigger = other.transform;
        }

        if (((1 << other.gameObject.layer) & ladderLayer) != 0)
        {
            isOnLadder = true;
            velocity.y = 0;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("ClimbTrigger"))
        {
            canClimbLedge = false;
            currentClimbTrigger = null;
        }

        if (((1 << other.gameObject.layer) & ladderLayer) != 0)
            isOnLadder = false;
    }

    // ================= PC =================
    void PCShortcuts()
    {
        if (!Application.isMobilePlatform)
        {
            if (Input.GetKeyDown(KeyCode.Space)) JumpButton();
            if (Input.GetKeyDown(KeyCode.C)) CrouchButton();
            isSprinting = Input.GetKey(KeyCode.LeftShift);
        }
    }

    public void StartSprint() => isSprinting = true;
    public void StopSprint() => isSprinting = false;
}
