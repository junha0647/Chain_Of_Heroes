using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class testPlayerController : MonoBehaviour
{
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float rotationSpeed;

    // private float jumpSpeed;
    // private float jumpButtonGracePeriod;

    [SerializeField]
    private Transform cameraTransform;

    private Animator animator;
    private CharacterController characterController;

    // private float ySpeed;
    // private float originalStepOffset;
    // private float? lastGroundedTime;
    // private float? jumpButtonPressedTime;

    private void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        // originalStepOffset = characterController.stepOffset;
    }

    private void Update()
    {
        Movement();

    }

    private void Movement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = new Vector3(h, 0, v);
        float inputMagnitude = Mathf.Clamp01(moveDir.magnitude);

        if(Input.GetKey(KeyCode.LeftShift))
        {
            inputMagnitude *= 2;
        }

        animator.SetFloat("Blend", inputMagnitude / 2, 0.05f, Time.deltaTime);

        float speed = inputMagnitude * maxSpeed;
        moveDir = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * moveDir;
        moveDir.Normalize();

        // ...

        Vector3 velocity = moveDir * speed;

        characterController.Move(velocity * Time.deltaTime);

        if(moveDir != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if(focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
