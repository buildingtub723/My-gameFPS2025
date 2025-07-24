using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera; // Assign your main or player camera here
    public InputActionAsset inputActions; // Assign your InputActionAsset here
    public LayerMask interactableMask;

    [Header("Interaction")]
    public float interactionDistance = 3f;

    private InputAction interactAction;

    private void Awake()
    {
        if (inputActions != null)
        {
            var playerMap = inputActions.FindActionMap("Player");
            if (playerMap != null)
                interactAction = playerMap.FindAction("Interact");
        }
    }

    private void OnEnable()
    {
        if (interactAction != null)
            interactAction.Enable();
    }

    private void OnDisable()
    {
        if (interactAction != null)
            interactAction.Disable();
    }

    private void Update()
    {
        if (interactAction != null && interactAction.WasPressedThisFrame())
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("PlayerInteract: No camera assigned!");
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactableMask))
        {
            var interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(gameObject);
            }
        }
    }
}