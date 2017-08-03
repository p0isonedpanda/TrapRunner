using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10.0f;
    [SerializeField] float sprintSpeed = 15.0f;
    [SerializeField] float jumpSpeed = 10.0f;
    [SerializeField] float lookSpeed = 10.0f;
    [SerializeField] float gravity = 20.0f;
    [SerializeField] GameObject playerCam;
    Vector3 moveDir = Vector3.zero;
    CharacterController charController;
    float mouseYLook = 0.0f;

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
            if (Input.GetKey(KeyCode.LeftShift)) // Sprinting
                moveDir = transform.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))) * sprintSpeed;
            else // Running
                moveDir = transform.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))) * moveSpeed;

            if (Input.GetButtonDown("Jump"))
                moveDir.y = jumpSpeed;
        }

        // Apply movement to player
        moveDir.y -= gravity * Time.deltaTime;
        charController.Move(moveDir * Time.deltaTime);

        //Debug.Log("(" + Input.GetAxis("Mouse X") + ", " + Input.GetAxis("Mouse Y") + ")");
        charController.transform.rotation = Quaternion.Euler(
            charController.transform.eulerAngles.x,
            charController.transform.eulerAngles.y + (Input.GetAxis("Mouse X") * lookSpeed * Time.deltaTime),
            charController.transform.eulerAngles.z);

        mouseYLook += Input.GetAxis("Mouse Y") * lookSpeed * Time.deltaTime;
        mouseYLook = Mathf.Clamp(mouseYLook, -90.0f, 90.0f);
        playerCam.transform.eulerAngles = new Vector3(-mouseYLook,
            playerCam.transform.eulerAngles.y,
            playerCam.transform.eulerAngles.z);
    }
}
