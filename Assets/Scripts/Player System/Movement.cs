using TMPro;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Collisions")]
    public bool onRunnableGround = false;
    public bool onRunnableWall = false;
    public bool stuckTowWall = false;
    public Collision collisionData;

    [Space(25)]

    [Header("Server-wide constants")]
    public float gravAccl = -19.6f;                         // Constant g = 9.8 m/s^2
    public float wallrunGravityFactor = 0.1f;               // 25% Gravity during wallrunning
    public float terminalVelocity = 56;                     // Terminal velocity of a falling human

    [Header("Physics")]
    [Header("Movement")]

    public ResetableVar<float> peakMomentum;                // Highest achievable momentum for this player
    public ResetableVar<float> runForce;                    // Amount of force applied during acceleration
    public ResetableVar<float> haltForce;                   // Amount of force applied during retardation

    public ResetableVar<float> wallrunStuckForce;           // Maximum amount of force that may be used to remain stuck to wall

    [Header("Jumping")]
    public ResetableVar<float> jumpMomentum;                // Momentum output while jumping
    public ResetableVar<float> jumpTime;                    // Cyote time
    public ResetableVar<int> jumpCount;                     // Number of availabel jumps

    [Header("Variances")]
    public ResetableVar<float> sprintBoost;                 // Multiplier while sprinting
    public ResetableVar<float> sneakSpeed;                  // Speed while sneaking
    public ResetableVar<float> wallrunJumpMultiplier;       // Multiplier applied to jump-off force

    [Space(25)]

    [Header("Interface")]
    public Vector2 mouseTurn = Vector2.zero;
    public float sensitivity = 1.8f;

    public bool isFwdPressed = false;
    public bool isBackPressed = false;
    public bool isLeftPressed = false;
    public bool isRightPressed = false;

    public bool isJumpPressed = false;

    public bool isSprintPressed = false;
    public bool isSneakPressed = false;

    public Rigidbody rb;
    public GameObject cameraObject;
    public PlayerManager playerManager;

    #if DEBUG
    [Space(25)]
    [Header("Debug Only")]
    public GameObject btnFwd;
    public GameObject btnBack;
    public GameObject btnLeft;
    public GameObject btnRight;
    public GameObject btnSprint;
    public GameObject btnSneak;
    public TMP_Text txtSpeed;
    public Vector3 initialPos;
    public KeyCode reset;
    #endif

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerManager = GetComponent<PlayerManager>();

        peakMomentum = new(playerManager.agent.movementInfo.peakMomentum);
        runForce = new(playerManager.agent.movementInfo.runForce);
        haltForce = new(playerManager.agent.movementInfo.haltForce);
        wallrunStuckForce = new(playerManager.agent.movementInfo.wallrunStuckForce);
        jumpMomentum = new(playerManager.agent.movementInfo.jumpMomentum);
        jumpTime = new(playerManager.agent.movementInfo.jumpTime);
        jumpCount = new(playerManager.agent.movementInfo.jumpCount);
        sprintBoost = new(playerManager.agent.movementInfo.sprintBoost);
        sneakSpeed = new(playerManager.agent.movementInfo.sneakSpeed);
        wallrunJumpMultiplier = new(playerManager.agent.movementInfo.wallrunJumpMultiplier);

        #if DEBUG
        initialPos = transform.position;
        #endif
    }

    void Update()
    {
        isFwdPressed = Input.GetKey(KeyBindManager.Instance.keyBinds["move_fwd"]);
        isBackPressed = Input.GetKey(KeyBindManager.Instance.keyBinds["move_back"]);
        isLeftPressed = Input.GetKey(KeyBindManager.Instance.keyBinds["move_left"]);
        isRightPressed = Input.GetKey(KeyBindManager.Instance.keyBinds["move_right"]);
        isJumpPressed = Input.GetKey(KeyBindManager.Instance.keyBinds["move_jump"]);
        isSprintPressed = Input.GetKey(KeyBindManager.Instance.keyBinds["move_sprint"]);
        isSneakPressed = Input.GetKey(KeyBindManager.Instance.keyBinds["move_sneak"]);

        #if DEBUG
        btnFwd.SetActive(isFwdPressed);
        btnBack.SetActive(isBackPressed);
        btnLeft.SetActive(isLeftPressed);
        btnRight.SetActive(isRightPressed);
        btnSprint.SetActive(isSprintPressed);
        btnSneak.SetActive(isSneakPressed);

        txtSpeed.text = Mathf.RoundToInt(rb.velocity.magnitude).ToString() + " m/s";

        if (Input.GetKeyDown(reset)) transform.position = initialPos + Vector3.up;
        #endif

        rb.mass = playerManager.mass.val;

        Cursor.lockState = CursorLockMode.Locked;

        mouseTurn.x += sensitivity*Input.GetAxis("Mouse X");
        mouseTurn.y += sensitivity*Input.GetAxis("Mouse Y");
        mouseTurn.y = Mathf.Clamp(mouseTurn.y, -90, 90);

        transform.localRotation = Quaternion.Euler(0, mouseTurn.x, 0);
        cameraObject.transform.localRotation = Quaternion.Euler(-mouseTurn.y, 0, 0);
    }

    void FixedUpdate()
    {
        float termVeloFactor = 1-Mathf.Abs(rb.velocity.y)/terminalVelocity;
        float gravForce = playerManager.mass.val * gravAccl * termVeloFactor * (onRunnableWall ? wallrunGravityFactor : 1);
        rb.AddForce(Vector3.up*gravForce);

        jumpTime.val -= Time.fixedDeltaTime;

        if (isSprintPressed) peakMomentum.val = sprintBoost.val*peakMomentum.trueValue;
        else peakMomentum.Reset();

        float targetSpeed = peakMomentum.val/playerManager.mass.val;
        if (isSneakPressed) targetSpeed = Mathf.Min(sneakSpeed.val, targetSpeed);

        if (onRunnableGround || onRunnableWall)
        {
            jumpTime.Reset();
            jumpCount.Reset();
        }

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        float velocityFB = Vector3.Dot(forward, rb.velocity);
        float velocityRL = Vector3.Dot(right, rb.velocity);

        float deltaVeloFB = targetSpeed * ((isFwdPressed?1:0)-(isBackPressed?1:0)) - velocityFB;
        float deltaVeloRL = targetSpeed * ((isRightPressed?1:0)-(isLeftPressed?1:0)) - velocityRL;

        float forceFB = Mathf.Abs(deltaVeloFB) > 0.5f ? Mathf.Sign(deltaVeloFB) * (
            Mathf.Sign(deltaVeloFB)==Mathf.Sign(velocityFB) ? runForce.val : haltForce.val
        ) : 0;
        float forceRL = Mathf.Abs(deltaVeloRL) > 0.5f ? Mathf.Sign(deltaVeloRL) * (
            Mathf.Sign(deltaVeloRL)==Mathf.Sign(velocityRL) ? runForce.val : haltForce.val
        ) : 0;

        if (isJumpPressed && jumpCount.val>0 && jumpTime.val>0)
        {
            if (onRunnableWall)
            {
                Vector3 up = (collisionData.GetContact(0).normal + Vector3.up).normalized;
                rb.velocity += wallrunJumpMultiplier.val*up*jumpMomentum.val/playerManager.mass.val;
                stuckTowWall = false;
            }
            else
            {
                Vector3 up = Vector3.up;
                rb.velocity += up*jumpMomentum.val/playerManager.mass.val;
            }
            jumpCount.val--;
        }

        Vector3 fNet = forward*forceFB + right*forceRL;
        rb.AddForce(fNet);

        if (stuckTowWall) {
            float wallrunOpposingForce = Mathf.Min(Vector3.Dot(fNet, collisionData.GetContact(0).normal), wallrunStuckForce.val);
            rb.AddForce(-collisionData.GetContact(0).normal*wallrunOpposingForce);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag=="RunnableGround") {
            onRunnableGround = true;
            collisionData = collision;
        }
        if (collision.gameObject.tag=="RunnableWall") {
            onRunnableWall = true;
            stuckTowWall = true;
            collisionData = collision;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag=="RunnableGround") {
            onRunnableGround = false;
            collisionData = null;
        }
        if (collision.gameObject.tag=="RunnableWall") {
            onRunnableWall = false;
            stuckTowWall = false;
            collisionData = null;
        }
    }
}
