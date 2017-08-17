using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerController : MonoBehaviour
{
    public static playerController instance { get; private set; }
    public float moveSpeed = 10.0f;
    public float sprintSpeed = 15.0f;
    public float staminaUsageRate = 15.0f;
    public float staminaRecoveryRate = 20.0f;
    public float staminaRecoveryCooldown = 2.0f;
    public float jumpSpeed = 10.0f;
    public float lookSpeedMax = 200.0f;
    public float gravity = 30.0f;
    public float lookSpeed;
    public float mouseYLook = 0.0f;
    public bool invertedLook = false;
    public GameObject playerCam;
    public LayerMask raycastInclude, terrainLayer, weaponHitLayer;

    Vector3 moveDir = Vector3.zero;
    CharacterController charController;
    float stamina = 100.0f;
    float staminaRecoveryTimer = 0.0f;
    GameObject pauseMenu;
    Image staminaBar, staminaUsed;
    gameController gc;

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
        staminaBar = GameObject.Find("stamina").GetComponent<Image>();
        staminaUsed = GameObject.Find("used").GetComponent<Image>();
        pauseMenu = GameObject.Find("pauseMenu");
        pauseMenu.SetActive(false);
        lookSpeed = lookSpeedMax / 2;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // Crouching
        if (Input.GetKey(KeyCode.LeftControl))
        {
            charController.height = Mathf.Lerp(charController.height, 1.0f, 0.2f);
        }
        else
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
                    stamina -= staminaUsageRate * Time.deltaTime; // Consume stamina while sprinting
                    staminaRecoveryTimer = 0.0f;
                }

                // Make sure stamina doesn't go lower than 0
                if (stamina < 0.0f)
                    stamina = 0.0f;

                // Start sprinting animation
                GameObject.FindGameObjectsWithTag("weapon")[0].GetComponent<Animator>().SetBool("sprinting", true);
            }
            else // Running
            {
                moveDir = transform.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))) * moveSpeed;
                staminaRecoveryTimer += Time.deltaTime;

                // If we've hit the stamina cooldown time, then start to regen stamina
                if (staminaRecoveryTimer >= staminaRecoveryCooldown && stamina < 100.0f)
                {
                    stamina += staminaRecoveryRate * Time.deltaTime;
                }

                // Make sure stamina doesn't go higher than 100
                if (stamina > 100.0f)
                    stamina = 100.0f;

                // Stop sprinting animation
                GameObject.FindGameObjectsWithTag("weapon")[0].GetComponent<Animator>().SetBool("sprinting", false);
            }

            if (Input.GetButtonDown("Jump")) // Jumping
                moveDir.y = jumpSpeed;

            // Start movement animation
            if (moveDir.x != 0 || moveDir.z != 0)
            {
                GameObject.FindGameObjectsWithTag("weapon")[0].GetComponent<Animator>().SetBool("running", true);
            }
            else
            {
                GameObject.FindGameObjectsWithTag("weapon")[0].GetComponent<Animator>().SetBool("running", false);
            }
        }
        else // If we are in the air...
        {
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
                    stamina -= 20.0f;
                }

            }
        }

        // Change stamina bar display to represent remaining stamina
        staminaBar.fillAmount = stamina / 100.0f;
        staminaUsed.fillAmount = 1.0f - staminaBar.fillAmount;

        // Apply movement to player
        moveDir.y -= gravity * Time.deltaTime;
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
}
