using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private CharacterController characterController;

    [SerializeField]private float movementSpeed;

    [SerializeField]private AnimationCurve jumpFallOff;
    [SerializeField]private float jumpMultiplier;

    [SerializeField]private float walkSpeed,runSpeed;
    [SerializeField]private float runBuildUpSpeed;
    

    private bool isJumping;

    private void Awake() {
        characterController = GetComponent<CharacterController>();
    }
    private void Update() {
        playerMovement();
    }
    private void playerMovement()
    {
        float verticalInput = Input.GetAxis("Vertical") * movementSpeed;
        float horizontalInput = Input.GetAxis("Horizontal") * movementSpeed;

        Vector3 forwardMovement = transform.forward * verticalInput;
        Vector3 rightMovement = transform.right * horizontalInput;

        characterController.SimpleMove(forwardMovement + rightMovement);
        
        SetMovementSpeed();
        JumpInput();
    }

    private void SetMovementSpeed()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            movementSpeed = Mathf.Lerp(movementSpeed, runSpeed, Time.deltaTime * runBuildUpSpeed);

        else
            movementSpeed = Mathf.Lerp(movementSpeed, walkSpeed, Time.deltaTime * runBuildUpSpeed);
    }
    private void JumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            isJumping = true;
            StartCoroutine(JumpEvent());
        }
    }
    private IEnumerator JumpEvent()
    {
        characterController.slopeLimit = 90.0f;
        float timeInAir = 0.0f;

        do
        {
            float jumpForce = jumpFallOff.Evaluate(timeInAir);
            characterController.Move(Vector3.up * jumpForce * jumpMultiplier * Time.deltaTime);
            timeInAir += Time.deltaTime;


            yield return null;
        } while (!characterController.isGrounded && characterController.collisionFlags != CollisionFlags.Above);

        characterController.slopeLimit = 45.0f;
        isJumping = false;
    }
}
