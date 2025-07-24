using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonCharacterController : MonoBehaviour
{
    public InputActionAsset InputActions;

    private InputAction m_moveAction;
    private InputAction m_lookAction;
    private InputAction m_jumpAction;
    private InputAction shootAction;

    public Weapon_Controller_Script currentWeapon;
    public Weapon_Controller_Script[] availableWeapons = new Weapon_Controller_Script[0];
    private int currentWeaponIndex = 0;

    public CharacterController controller;
    public Animator gunAnimator;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Transform weaponHolder;
    public Transform CameraCenter; // camera pivot
    //audio Variables
    [SerializeField] private PlayerAudioHandler playerAudio;
    public float footstepInterval = 1f;

    private float footstepTimer = 0f;

    public float walkspeed = 5f;
    public float rotateSpeed = 3f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float bulletSpeed = 20f;

    private Vector3 currentMoveVelocity;
    public float acceleration = 10f;
    public float deceleration = 15f;

    [Header("Ground Check")]
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayers;

    private Vector3 velocity;
    private float rotationY;
    private bool isGrounded;
    private void Awake()
    {
        shootAction = InputActions.FindActionMap("Player").FindAction("Attack");
        m_moveAction = InputActions.FindActionMap("Player").FindAction("Move");
        m_lookAction = InputActions.FindActionMap("Player").FindAction("Look");
        m_jumpAction = InputActions.FindActionMap("Player").FindAction("Jump");
    }

    private void OnEnable()
    {
        shootAction.Enable();
        m_moveAction.Enable();
        m_lookAction.Enable();
        m_jumpAction.Enable();
    }

    private void OnDisable()
    {
        shootAction.Disable();
        m_moveAction.Disable();
        m_lookAction.Disable();
        m_jumpAction.Disable();
    }

    private void Update()
    {
        rotate();
        HandleJumpAndGravity();
        Move();

        Vector2 input = m_moveAction.ReadValue<Vector2>();
        bool isMoving = input.magnitude > 0.1f;

        if (isMoving && isGrounded)
        {
            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0f)
            {
                playerAudio.PlayFootstep();
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            // Reset the timer so it doesn't instantly play again when you move
            footstepTimer = 0f;
        }

        if (currentWeapon != null)
        {
            if (currentWeapon.fireMode == FireMode.SemiAuto)
            {
                if (shootAction.WasPressedThisFrame())
                    currentWeapon.Fire();
            }
            else if (currentWeapon.fireMode == FireMode.FullAuto)
            {
                if (shootAction.IsPressed())
                    currentWeapon.StartFiring();
                else
                    currentWeapon.StopFiring();
            }
        }
        if (Keyboard.current.rKey.wasPressedThisFrame && currentWeapon != null)
        {
            currentWeapon.Reload();
        }
        if (Keyboard.current.digit1Key.wasPressedThisFrame) SwitchWeapon(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SwitchWeapon(1);
    }
    private bool CheckIfGrounded()
    {
        // Raycast from just above the bottom of the controller downward
        float rayLength = controller.height / 2 + groundCheckDistance;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        return Physics.Raycast(rayOrigin, Vector3.down, rayLength, groundLayers);
    }

    private void HandleJumpAndGravity()
    {
        isGrounded = CheckIfGrounded();

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // small downward force to keep grounded
        }

        if (m_jumpAction.WasPressedThisFrame())
        {
            if (isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void Move()
    {
        Vector2 input = m_moveAction.ReadValue<Vector2>();
        Vector3 inputDir = transform.forward * input.y + transform.right * input.x;
        Vector3 targetVelocity = inputDir.normalized * walkspeed;

        // Determine acceleration or deceleration rate
        float smoothFactor = (inputDir.magnitude > 0) ? acceleration : deceleration;

        // Smoothly interpolate the velocity
        currentMoveVelocity = Vector3.Lerp(currentMoveVelocity, targetVelocity, smoothFactor * Time.deltaTime);

        // Apply movement
        controller.Move(currentMoveVelocity * Time.deltaTime);
    }

    private void rotate()
    {
        Vector2 lookInput = m_lookAction.ReadValue<Vector2>();
        float mouseX = lookInput.x * rotateSpeed;
        float mouseY = lookInput.y * rotateSpeed;

        // Horizontal rotation (rotate player)
        transform.Rotate(Vector3.up * mouseX);

        // Vertical rotation (clamp and apply to camera only)
        rotationY -= mouseY;
        rotationY = Mathf.Clamp(rotationY, -80f, 80f);
        CameraCenter.localRotation = Quaternion.Euler(rotationY, 0f, 0f);
    }
    private void SwitchWeapon(int index)
    {
        if (index < 0 || index >= availableWeapons.Length) return;

        // Disable all weapons first
        for (int i = 0; i < availableWeapons.Length; i++)
        {
            availableWeapons[i].gameObject.SetActive(i == index);
        }

        currentWeaponIndex = index;
        currentWeapon = availableWeapons[currentWeaponIndex];
    }
    public void PickupWeapon(GameObject weaponPrefab)
    {
        if (weaponPrefab == null)
        {
            Debug.LogError("Weapon prefab is null!");
            return;
        }

        // Safety check for availableWeapons
        if (availableWeapons == null)
        {
            availableWeapons = new Weapon_Controller_Script[0];
        }

        // Prevent picking up duplicates (compare by prefab name)
        foreach (var weapon in availableWeapons)
        {
            if (weapon != null && weapon.name.StartsWith(weaponPrefab.name))
            {
                Debug.Log("Weapon already picked up!");
                return;
            }
        }

        // Instantiate weapon under weaponHolder
        GameObject weaponInstance = Instantiate(weaponPrefab, weaponHolder);
        weaponInstance.transform.localPosition = Vector3.zero;
        weaponInstance.transform.localRotation = Quaternion.identity;

        Weapon_Controller_Script newWeapon = weaponInstance.GetComponent<Weapon_Controller_Script>();
        if (newWeapon == null)
        {
            Debug.LogError("Weapon prefab does not have a Weapon_Controller_Script attached!");
            Destroy(weaponInstance); // clean up the bad object
            return;
        }

        // Add new weapon to list
        var weaponList = new System.Collections.Generic.List<Weapon_Controller_Script>(availableWeapons);
        weaponList.Add(newWeapon);
        availableWeapons = weaponList.ToArray();

        // Deactivate all weapons
        foreach (var weapon in availableWeapons)
        {
            if (weapon != null)
                weapon.gameObject.SetActive(false);
        }

        // Activate the newly picked-up weapon
        currentWeapon = newWeapon;
        currentWeaponIndex = availableWeapons.Length - 1;
        currentWeapon.gameObject.SetActive(true);

        Debug.Log("Picked up weapon: " + currentWeapon.name);
    }
    private void OnDrawGizmosSelected()
    {
        if (controller == null) return;

        Gizmos.color = Color.red;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * (controller.height / 2 + groundCheckDistance));
    }
}