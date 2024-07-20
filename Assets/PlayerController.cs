using UnityEngine;
using Cinemachine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float gravityScale = 1f;
    public float rotationSpeed = 2f;
    public float wallOffset = 0.5f; // Adjust based on wall thickness
    public float jumpHeight = 2f; // Adjust this value for the jump height

    public GameObject hollowgramPrefab; // Reference to the hollowgram prefab
    public CinemachineVirtualCamera virtualCamera; // Reference to the Cinemachine Virtual Camera

    private GameObject hollowgramInstance; // Instance of the hollowgram
    private Vector3 moveDirection;
    private Vector3 gravityDirection = Vector3.down;
    private Vector3 velocity;
    private bool isGrounded = true;
    private bool rightArrowPressed = false;

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleGravityManipulation();
        ApplyGravity();
        UpdateCameraRotation();
    }

    void HandleMovement()
    {
        if (isGrounded)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                float moveX = Input.GetAxis("Horizontal") * moveSpeed;
                float moveZ = Input.GetAxis("Vertical") * moveSpeed;

                Vector3 forwardMovement = transform.forward * moveZ;
                Vector3 rightMovement = transform.right * moveX;

                moveDirection = (forwardMovement + rightMovement).normalized * moveSpeed;
                velocity = new Vector3(moveDirection.x, velocity.y, moveDirection.z);

                transform.position += velocity * Time.deltaTime;
            }
        }
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = jumpForce;
            isGrounded = false;
        }
    }

    void HandleGravityManipulation()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            rightArrowPressed = true;
            // Instantiate the hologram without rotation
            Vector3 hollowGramPosition = transform.position + new Vector3(transform.localScale.x + 0.7f, (transform.localScale.y * 2) - .1f, 0f);
            hollowgramInstance = Instantiate(hollowgramPrefab, hollowGramPosition, Quaternion.identity);
            hollowgramInstance.transform.rotation = Quaternion.Euler(0, 0, 90f);
            hollowgramInstance.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Return) && rightArrowPressed)
        {
            rightArrowPressed = false;
            StartCoroutine(MovePlayerToHologram());
        }
    }

    void ApplyGravity()
    {
        if (!isGrounded)
        {
            velocity += gravityDirection * gravityScale * Time.deltaTime;
            if (transform.position.y <= 0) // Simulate ground hit
            {
                isGrounded = true;
                velocity.y = 0;
            }
        }
    }

    IEnumerator MovePlayerToHologram()
    {
        if (hollowgramInstance != null)
        {
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = hollowgramInstance.transform.position;
            Quaternion targetRotation = hollowgramInstance.transform.rotation;
            float moveDuration = 0.5f; // Duration of the move
            float elapsedTime = 0f;

            while (elapsedTime < moveDuration)
            {
                float t = elapsedTime / moveDuration;
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition;
            transform.rotation = targetRotation;

            // Apply gravity to simulate falling down
            isGrounded = false;
            velocity = new Vector3(0, -jumpForce, 0); // Initial downward velocity

            while (!isGrounded)
            {
                velocity += gravityDirection * gravityScale * Time.deltaTime;
                transform.position += velocity * Time.deltaTime;
                if (transform.position.y <= 0) // Simulate ground hit
                {
                    isGrounded = true;
                    velocity.y = 0;
                }
                yield return null;
            }

            // Destroy the hologram
            Destroy(hollowgramInstance);
        }
    }

    void UpdateCameraRotation()
    {
        if (virtualCamera != null)
        {
            // Smoothly rotate the camera to match the player's rotation
            Quaternion targetRotation = transform.rotation;
            virtualCamera.transform.rotation = Quaternion.Slerp(
                virtualCamera.transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}
