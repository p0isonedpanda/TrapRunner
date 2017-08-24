using UnityEngine;
using UnityEngine.UI;

public class playerController : MonoBehaviour
{
    public static playerController instance { get; private set; }
    gameController gc;

    [Header("Movement")]
    public float moveSpeed = 10.0f;
    public float sprintSpeed = 15.0f;
    public float jumpSpeed = 10.0f;
    public float gravity = 30.0f;
    [HideInInspector] public bool trapped;

    Vector3 moveDir = Vector3.zero;

    [Header("Look")]
    public GameObject playerCam;
    public float lookSpeedMax = 200.0f;
    [HideInInspector] public float lookSpeed;
    [HideInInspector] public float mouseYLook;
    [HideInInspector] public bool invertedLook = false;

    [Header("Health")]
    [HideInInspector]public float health;
    public float maxHealth;
    public float healthRegenRate = 1.0f;
    public float healthRegenCooldown = 5.0f;

    float healthRegenTimer;

    [Header("Stamina")]
    [HideInInspector] public float stamina;
    public float maxStamina;
    public float staminaUsageRate = 15.0f;
    public float staminaRecoveryRate = 20.0f;
    public float staminaRecoveryCooldown = 2.0f;

    float staminaRecoveryTimer;

    [Header("Layers")]
    public LayerMask terrainLayer;

    CharacterController charController;
    GameObject pauseMenu;
    Animator firstPersonAnim;


    // Used to initialise singleton
    void Awake()
    {
        if (instance != null)
            throw new System.Exception();

        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        charController = GetComponent<CharacterController>();
        gc = gameController.instance;
        pauseMenu = GameObject.Find("pauseMenu");
        lookSpeed = lookSpeedMax / 2;
        Cursor.lockState = CursorLockMode.Locked;
        firstPersonAnim = GameObject.FindGameObjectsWithTag("weapon")[0].GetComponent<Animator>();
        stamina = maxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        // Health regen
        healthRegenTimer += Time.deltaTime;

        if (healthRegenTimer >= healthRegenCooldown)
        {
            health = Mathf.Clamp(health + healthRegenRate * Time.deltaTime, 0.0f, maxHealth);
        }

        // Just to test the health system
        if (Input.GetKeyDown(KeyCode.RightControl))
            ApplyDamage(10.0f);

        // Crouching
        if (Input.GetKey(KeyCode.LeftControl))
        {
            charController.height = Mathf.Lerp(charController.height, 1.0f, 0.2f);
        }
        else if (!Physics.Raycast(transform.position, Vector3.up, 1.0f, terrainLayer)) // Check if we can uncrouch
        {
            charController.height = Mathf.Lerp(charController.height, 2.0f, 0.2f);
        }

        if (charController.isGrounded) // Make sure we can only move while we are grounded
        {
            if (Input.GetKey(KeyCode.LeftShift) && stamina > 0.0f) // Sprinting
            {
                moveDir = transform.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))) * sprintSpeed;

                // Make sure we're moving before consuming stamina
                if (moveDir != Vector3.zero)
                {
                    stamina = Mathf.Clamp(stamina - staminaUsageRate * Time.deltaTime, 0.0f, maxStamina);
                    staminaRecoveryTimer = 0.0f;
                }

                // Start sprinting animation
                firstPersonAnim.SetBool("sprinting", true);
            }
            else // Running
            {
                moveDir = transform.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))) * moveSpeed;
                staminaRecoveryTimer += Time.deltaTime;

                // If we've hit the stamina cooldown time, then start to regen stamina
                if (staminaRecoveryTimer >= staminaRecoveryCooldown && stamina < 100.0f)
                {
                    stamina = Mathf.Clamp(stamina + staminaRecoveryRate * Time.deltaTime, 0.0f, maxStamina);
                }

                // Stop sprinting animation
                firstPersonAnim.SetBool("sprinting", false);
            }

            if (Input.GetButtonDown("Jump")) // Jumping
                moveDir.y = jumpSpeed;

            // Start movement animation
            if (moveDir.x != 0 || moveDir.z != 0)
            {
                firstPersonAnim.SetBool("running", true);
            }
            else
            {
                firstPersonAnim.SetBool("running", false);
            }
        }
        else // If we are in the air...
        {
            if (Input.GetKey(KeyCode.LeftShift) && stamina > 0.0f && moveDir != Vector3.zero)
            {
                stamina = Mathf.Clamp(stamina - staminaUsageRate * Time.deltaTime, 0.0f, maxStamina);
                staminaRecoveryTimer = 0.0f;
            }

            RaycastHit frontCheck;
            Debug.DrawRay(new Vector3(charController.transform.position.x, charController.transform.position.y - 0.8f, charController.transform.position.z), Vector3.forward, Color.red);

            //...check if there's a wall we can climb
            if (Physics.Raycast(new Vector3(charController.transform.position.x,
                charController.transform.position.y - 0.8f,
                charController.transform.position.z),
                playerCam.transform.TransformDirection(Vector3.forward),
                out frontCheck, 1.0f, terrainLayer.value))
            {
                if (Input.GetButtonDown("Jump") && stamina > 20.0f)
                {
                    moveDir.y = jumpSpeed;
                    stamina = Mathf.Clamp(stamina - 20.0f, 0.0f, maxStamina);
                }

            }
        }

        // Apply movement to player
        moveDir.y -= gravity * Time.deltaTime;
        if (!trapped)
            charController.Move(moveDir * Time.deltaTime);

        // Rotate the player on the Y axis
        charController.transform.rotation = Quaternion.Euler(
            charController.transform.eulerAngles.x,
            charController.transform.eulerAngles.y + (Input.GetAxis("Mouse X") * lookSpeed * Time.deltaTime),
            charController.transform.eulerAngles.z);

        // Rotate the camera on the X axis
        if (!invertedLook)
        {
            mouseYLook += Input.GetAxis("Mouse Y") * lookSpeed * Time.deltaTime;
            mouseYLook = Mathf.Clamp(mouseYLook, -90.0f, 90.0f);
            playerCam.transform.eulerAngles = new Vector3(-mouseYLook,
                playerCam.transform.eulerAngles.y,
                playerCam.transform.eulerAngles.z);
        }
        else
        {
            mouseYLook += Input.GetAxis("Mouse Y") * lookSpeed * Time.deltaTime;
            mouseYLook = Mathf.Clamp(mouseYLook, -90.0f, 90.0f);
            playerCam.transform.eulerAngles = new Vector3(mouseYLook,
                playerCam.transform.eulerAngles.y,
                playerCam.transform.eulerAngles.z);
        }

        // Pause menu
        if (!gc.paused && Input.GetKeyDown(KeyCode.P)) // Pause the game
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            gc.paused = true;
        }
        else if (gc.paused && Input.GetKeyDown(KeyCode.P)) // Unpause the game
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            gc.paused = false;
        }
    }

    public void ApplyDamage (float damage)
    {
        health = Mathf.Clamp(health - damage, 0.0f, 100.0f);
        healthRegenTimer = 0.0f;
    }
}
