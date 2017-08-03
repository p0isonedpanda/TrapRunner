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
    [SerializeField] GameObject playerCam;
    [SerializeField] Image staminaBar;
    Vector3 moveDir = Vector3.zero;
    CharacterController charController;
    float mouseYLook = 0.0f;
    float stamina = 100.0f;
    float staminaRecoveryTimer = 0.0f;

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
                stamina -= staminaUsageRate * Time.deltaTime;
                staminaRecoveryTimer = 0.0f;

                if (stamina < 0.0f)
                {
                    stamina = 0.0f;
                }
            }
            else // Running
            {
                moveDir = transform.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))) * moveSpeed;
                staminaRecoveryTimer += Time.deltaTime;

                if (staminaRecoveryTimer >= staminaRecoveryCooldown && stamina < 100.0f)
                {
                    stamina += staminaRecoveryRate * Time.deltaTime;
                }

                if (stamina > 100.0f)
                {
                    stamina = 100.0f;
                }
            }

            if (Input.GetButtonDown("Jump"))
                moveDir.y = jumpSpeed;
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
    }
}
