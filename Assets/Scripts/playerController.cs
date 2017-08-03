using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 250.0f;
    [SerializeField] float sprintSpeed = 500.0f;
    [SerializeField] float jumpSpeed = 1000.0f;
    [SerializeField] float lookSpeed = 10.0f;
    [SerializeField] float gravity = 20.0f;
    [SerializeField] GameObject playerCam;
    Vector3 moveDir = Vector3.zero;
    CharacterController charController;

	// Use this for initialization
	void Start()
    {
        charController = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            charController.Move(new Vector3(
                Input.GetAxis("Horizontal") * sprintSpeed * Time.deltaTime,
                0.0f,
                Input.GetAxis("Vertical") * sprintSpeed * Time.deltaTime));
        }
        else
        {
            charController.Move(new Vector3(
                Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime,
                0.0f,
                Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime));
        }
    }
}
