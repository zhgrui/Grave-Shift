using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class TeleportCooldown : MonoBehaviour
{
    [Tooltip("Seconds before the player can teleport again.")]
    [SerializeField] private float cooldownDuration = 2f;

    private XRRayInteractor rayInteractor;
    private float cooldownTimer;
    private bool onCooldown;

    private void Awake()
    {
        rayInteractor = GetComponent<XRRayInteractor>();
    }

    private void OnEnable()
    {
        rayInteractor.selectEntered.AddListener(OnTeleport);
    }

    private void OnDisable()
    {
        rayInteractor.selectEntered.RemoveListener(OnTeleport);
    }

    private void OnTeleport(SelectEnterEventArgs args)
    {
        if (args.interactableObject is not UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationArea and not UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationAnchor)
            return;

        onCooldown = true;
        cooldownTimer = cooldownDuration;
        rayInteractor.enabled = false;
    }

    private void Update()
    {
        if (!onCooldown)
            return;

        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f)
        {
            onCooldown = false;
            rayInteractor.enabled = true;
        }
    }
}
