using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerController : MonoBehaviour
{
    public float moveSpeed = 10.0f;
    public float sprintSpeed = 15.0f;
    public float staminaUsageRate = 15.0f;
    public float staminaRecoveryRate = 20.0f;
    public float staminaRecoveryCooldown = 2.0f;
    public float jumpSpeed = 10.0f;
    public float lookSpeedMax = 200.0f;
    public float gravity = 30.0f;
    public float weaponRange = 100.0f;
    public float weaponFireRate = 0.1f;
    public float weaponInaccuracy = 0.0f;
    public GameObject playerCam, bulletDecal;
    public LayerMask raycastInclude, terrainLayer, weaponHitLayer;

    Vector3 moveDir = Vector3.zero;
    CharacterController charController;
    float mouseYLook = 0.0f;
    float lookSpeed;
    float stamina = 100.0f;
    float staminaRecoveryTimer = 0.0f;
    float fireRateTimer = 0.0f;
    bool holdingItem = false;
    bool itemDropped = true;
    bool invertedLook = false;
    GameObject itemHeld, pauseMenu;
    Image staminaBar, outerCrosshair;

	// Use this for initialization
	void Start()
    {
        charController = GetComponent<CharacterController>();
        staminaBar = GameObject.Find("stamina").GetComponent<Image>();
        outerCrosshair = GameObject.Find("outerCrosshair").GetComponent<Image>();
        pauseMenu = GameObject.Find("pauseMenu");
        pauseMenu.SetActive(false);
        lookSpeed = lookSpeedMax / 2;
        Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            charController.height = Mathf.Lerp(charController.height, 1.0f, 0.2f);
        }
        else
        {
            charController.height = Mathf.Lerp(charController.height, 2.0f, 0.2f);
        }

        if (charController.isGrounded)
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
            }

            if (Input.GetButtonDown("Jump"))
                moveDir.y = jumpSpeed;
        }
        else // If we are in the air
        {
            RaycastHit frontCheck;
            Debug.DrawRay(new Vector3(charController.transform.position.x, charController.transform.position.y - 0.8f, charController.transform.position.z), Vector3.forward, Color.red);

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

        staminaBar.transform.localScale = new Vector3(stamina / 100.0f, 1.0f, 1.0f);

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

        RaycastHit pickupRay;

        if (Physics.Raycast(playerCam.transform.position,
            playerCam.transform.TransformDirection(Vector3.forward),
            out pickupRay, 2.0f, raycastInclude.value))
        {
            // If we see something we can pickup, we want to make the outer crosshair visible
            switch (pickupRay.collider.tag)
            {
                case "crate":
                    outerCrosshair.color = new Vector4(outerCrosshair.color.r,
                        outerCrosshair.color.g,
                        outerCrosshair.color.b,
                        Mathf.Lerp(outerCrosshair.color.a, 1.0f, 0.3f));

                    if (Input.GetKeyDown(KeyCode.E) && !holdingItem)
                    {
                        holdingItem = true;
                        itemDropped = false;
                        itemHeld = pickupRay.collider.gameObject;
                    }
                    break;

                default: // If we can't pick it up, fade out outer crosshair
                    outerCrosshair.color = new Vector4(outerCrosshair.color.r,
                        outerCrosshair.color.g,
                        outerCrosshair.color.b,
                        Mathf.Lerp(outerCrosshair.color.a, 0.0f, 0.3f));
                    break;
            }
        }
        else
        {
            // If we don't have any pickups in range, fade out outer crosshair
            outerCrosshair.color = new Vector4(outerCrosshair.color.r,
                    outerCrosshair.color.g,
                    outerCrosshair.color.b,
                    Mathf.Lerp(outerCrosshair.color.a, 0.0f, 0.3f));
        }

        if (holdingItem)
        {
            // If we're holding an item, we need to parent it to the camera and place it in front
            itemHeld.transform.SetParent(playerCam.transform);
            itemHeld.transform.localPosition = new Vector3(0, 0, 1.5f);
            itemHeld.transform.localEulerAngles = Vector3.zero;
            if (Input.GetKeyDown(KeyCode.Mouse0)) // Check if we want to drop it
            {
                holdingItem = false;
            }
        }
        else
        {
            // When we drop the item, make sure it has no velocity
            if (!itemDropped)
            {
                playerCam.transform.DetachChildren();
                itemHeld.GetComponent<Rigidbody>().velocity = Vector3.zero;
                itemHeld.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                itemDropped = true;
            }
        }

        // Pause menu
        if (Time.timeScale != 0 && Input.GetKeyDown(KeyCode.P)) // Pause the game
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
        else if (Time.timeScale == 0 && Input.GetKeyDown(KeyCode.P)) // Unpause the game
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }

        // Weapons hit detection
        RaycastHit weaponRay;
        fireRateTimer += Time.deltaTime;

        // If we're allowed to fire...
        if (Input.GetKey(KeyCode.Mouse0) && fireRateTimer >= weaponFireRate)
        {
            // ...then we want to raycast
            if (Physics.Raycast(playerCam.transform.position,
            playerCam.transform.TransformDirection(GetWeaponInaccuracy()),
            out weaponRay, weaponRange, weaponHitLayer.value))
            {
                fireRateTimer = 0.0f;

                // Check if we hit the enemy AI
                if (weaponRay.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    weaponRay.collider.gameObject.GetComponent<AINavigation>().ApplyDamage(10.0f);
                }
                else // If not, then create a bullet decal
                {
                    Instantiate(bulletDecal, weaponRay.point, Quaternion.Euler(weaponRay.normal));
                }
            }
        }  
    }

    // Used to create inaccuracy when the weapon is shot
    Vector3 GetWeaponInaccuracy()
    {
        if (weaponInaccuracy > 0)
            return Vector3.forward + new Vector3(
                Random.Range(-weaponInaccuracy / 2, weaponInaccuracy / 2),
                Random.Range(-weaponInaccuracy / 2, weaponInaccuracy / 2), 0);
        else
            return Vector3.forward;
    }

    // Function to be called when the look sensitivity is changed
    public void ChangeLookSensitivity()
    {
        lookSpeed = GameObject.Find("HUD/pauseMenu/lookSensitivity/lookSensitivitySlider").
            GetComponent<Slider>().value * lookSpeedMax;
        GameObject.Find("HUD/pauseMenu/lookSensitivity/value").GetComponent<Text>().
            text = (Mathf.Floor(lookSpeed) / lookSpeedMax).ToString();
    }

    // Used to invert Y look
    public void InvertYLook()
    {
        if (GameObject.Find("HUD/pauseMenu/invertY").GetComponent<Toggle>().isOn)
            invertedLook = true;
        else
            invertedLook = false;
    }

    public void ResumePlay()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }
}
