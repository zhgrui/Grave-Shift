using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

/// <summary>
/// Hold left grip to show teleport ray, release to teleport.
/// Attach to the same GameObject as the XRRayInteractor (Left Controller).
/// </summary>
[RequireComponent(typeof(XRRayInteractor))]
public class TeleportInputSetup : MonoBehaviour
{
    [SerializeField] private TeleportationProvider teleportationProvider;

    private XRRayInteractor rayInteractor;
    private TeleportCooldown cooldown;
    private InputAction gripAction;

    private void Awake()
    {
        rayInteractor = GetComponent<XRRayInteractor>();
        cooldown = GetComponent<TeleportCooldown>();
        rayInteractor.enabled = false;

        gripAction = new InputAction("Grip", InputActionType.Button);
        gripAction.AddBinding("<XRController>{LeftHand}/gripButton");

        if (teleportationProvider == null)
            teleportationProvider = FindAnyObjectByType<TeleportationProvider>();
    }

    private void OnEnable()
    {
        gripAction.Enable();
        gripAction.started += OnGripPressed;
        gripAction.canceled += OnGripReleased;
    }

    private void OnDisable()
    {
        gripAction.started -= OnGripPressed;
        gripAction.canceled -= OnGripReleased;
        gripAction.Disable();
    }

    private void OnGripPressed(InputAction.CallbackContext ctx)
    {
        if (cooldown != null && cooldown.IsOnCooldown)
            return;

        rayInteractor.enabled = true;
    }

    private void OnGripReleased(InputAction.CallbackContext ctx)
    {
        if (!rayInteractor.enabled)
            return;

        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            var teleportArea = hit.collider.GetComponent<TeleportationArea>();
            if (teleportArea != null && teleportationProvider != null)
            {
                var request = new TeleportRequest
                {
                    destinationPosition = hit.point,
                    matchOrientation = teleportArea.matchOrientation
                };
                teleportationProvider.QueueTeleportRequest(request);

                if (cooldown != null)
                    cooldown.StartCooldown();
            }
        }

        rayInteractor.enabled = false;
    }
}
