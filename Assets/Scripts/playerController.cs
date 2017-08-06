using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10.0f;
    [SerializeField] float sprintSpeed = 15.0f;
    [SerializeField] float staminaUsageRate = 15.0f;
    [SerializeField] float staminaRecoveryRate = 20.0f;
    [SerializeField] float staminaRecoveryCooldown = 2.0f;
    [SerializeField] float jumpSpeed = 10.0f;
    [SerializeField] float lookSpeed = 10.0f;
    [SerializeField] float gravity = 30.0f;
    [SerializeField] float weaponRange = 100.0f;
    [SerializeField] float weaponFireRate = 0.1f;
    [SerializeField] float weaponInaccuracy = 0.0f;
    [SerializeField] GameObject playerCam, pauseMenu, bulletDecal;
    [SerializeField] Image staminaBar, outerCrosshair;
    [SerializeField] LayerMask raycastInclude, terrainLayer, weaponHitLayer;
    Vector3 moveDir = Vector3.zero;
    CharacterController charController;
    float mouseYLook = 0.0f;
    float stamina = 100.0f;
    float staminaRecoveryTimer = 0.0f;
    float fireRateTimer = 0.0f;
    bool holdingItem = false;
    bool itemDropped = true;
    GameObject itemHeld;

	// Use this for initialization
	void Start()
    {
        charController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update()
    {
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
        mouseYLook += Input.GetAxis("Mouse Y") * lookSpeed * Time.deltaTime;
        mouseYLook = Mathf.Clamp(mouseYLook, -90.0f, 90.0f);
        playerCam.transform.eulerAngles = new Vector3(-mouseYLook,
            playerCam.transform.eulerAngles.y,
            playerCam.transform.eulerAngles.z);

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
        Debug.DrawRay(playerCam.transform.position, playerCam.transform.TransformDirection(GetWeaponInaccuracy()) * 100, Color.red);
        fireRateTimer += Time.deltaTime;

        if (Physics.Raycast(playerCam.transform.position,
            playerCam.transform.TransformDirection(GetWeaponInaccuracy()),
            out weaponRay, weaponRange, weaponHitLayer.value) &&
            fireRateTimer >= weaponFireRate &&
            Input.GetKey(KeyCode.Mouse0))
        {
            Debug.Log("Pew!");
            Instantiate(bulletDecal, weaponRay.point, Quaternion.Euler(Vector3.zero));
            fireRateTimer = 0.0f;
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
}
