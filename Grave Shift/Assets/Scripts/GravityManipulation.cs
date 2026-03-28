using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;

/// <summary>
/// Rotates the world 90 degrees around the X-axis when the player presses a button.
/// Attach to a manager GameObject. Requires a "World" parent containing all environment objects.
/// </summary>
public class GravityManipulation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform worldContainer;
    [SerializeField] private Transform playerTransform;

    [Header("Settings")]
    [SerializeField] private float rotationDuration = 0.8f;
    [SerializeField] private float cooldownDuration = 2f;

    [Header("Teleportation Surfaces")]
    [Tooltip("All teleportation areas in the scene. Only the current floor surface will be active.")]
    [SerializeField] private TeleportationSurface[] surfaces;

    private InputAction gravityAction;
    private bool isRotating;
    private float cooldownTimer;
    private int currentRotationStep;

    public bool IsTransitioning => isRotating;

    public static GravityManipulation Instance { get; private set; }

    public event Action OnGravityShiftStart;
    public event Action OnGravityShiftEnd;

    private void Awake()
    {
        Instance = this;

        gravityAction = new InputAction("GravityFlip", InputActionType.Button);
        gravityAction.AddBinding("<XRController>{RightHand}/primaryButton");

        if (playerTransform == null)
        {
            var xrOrigin = FindAnyObjectByType<Unity.XR.CoreUtils.XROrigin>();
            if (xrOrigin != null)
                playerTransform = xrOrigin.transform;
        }

        if (worldContainer == null)
        {
            var world = GameObject.Find("World");
            if (world != null)
                worldContainer = world.transform;
        }

        AutoDiscoverSurfaces();
    }

    private void Start()
    {
        ActivateCurrentFloorSurface();
    }

    private void AutoDiscoverSurfaces()
    {
        if (surfaces != null && surfaces.Length > 0)
            return;

        var allAreas = FindObjectsByType<UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationArea>(
            FindObjectsSortMode.None);
        var list = new System.Collections.Generic.List<TeleportationSurface>();

        foreach (var area in allAreas)
        {
            var surface = new TeleportationSurface { teleportArea = area };
            string goName = area.gameObject.name.ToLower();

            if (goName.Contains("floor"))
                surface.gravityStep = 0;
            else if (goName.Contains("front"))
                surface.gravityStep = 1;
            else if (goName.Contains("ceiling"))
                surface.gravityStep = 2;
            else if (goName.Contains("back"))
                surface.gravityStep = 3;

            list.Add(surface);
        }

        surfaces = list.ToArray();
    }

    private void OnEnable()
    {
        gravityAction.Enable();
        gravityAction.performed += OnGravityButtonPressed;
    }

    private void OnDisable()
    {
        gravityAction.performed -= OnGravityButtonPressed;
        gravityAction.Disable();
    }

    private void TickCooldown()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    private void Update()
    {
        TickCooldown();
    }

    private void OnGravityButtonPressed(InputAction.CallbackContext ctx)
    {
        if (isRotating || cooldownTimer > 0f)
            return;

        StartCoroutine(RotateWorld());
    }

    private IEnumerator RotateWorld()
    {
        isRotating = true;
        OnGravityShiftStart?.Invoke();

        SetAllSurfacesActive(false);

        Vector3 pivot = playerTransform.position;
        float targetAngle = 90f;
        float elapsed = 0f;

        Quaternion startRotation = worldContainer.rotation;
        Vector3 startPosition = worldContainer.position;

        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / rotationDuration);
            float currentAngle = Mathf.Lerp(0f, targetAngle, t);

            worldContainer.rotation = startRotation;
            worldContainer.position = startPosition;
            worldContainer.RotateAround(pivot, Vector3.right, currentAngle);

            yield return null;
        }

        worldContainer.rotation = startRotation;
        worldContainer.position = startPosition;
        worldContainer.RotateAround(pivot, Vector3.right, targetAngle);

        currentRotationStep = (currentRotationStep + 1) % 4;
        ApplyGravityForStep(currentRotationStep);
        ActivateCurrentFloorSurface();

        cooldownTimer = cooldownDuration;
        isRotating = false;
        OnGravityShiftEnd?.Invoke();
    }

    private void ApplyGravityForStep(int step)
    {
        switch (step)
        {
            case 0: Physics.gravity = new Vector3(0, -9.81f, 0); break;
            case 1: Physics.gravity = new Vector3(0, 0, 9.81f); break;
            case 2: Physics.gravity = new Vector3(0, 9.81f, 0); break;
            case 3: Physics.gravity = new Vector3(0, 0, -9.81f); break;
        }
    }

    private void SetAllSurfacesActive(bool active)
    {
        if (surfaces == null) return;
        foreach (var surface in surfaces)
        {
            if (surface != null && surface.teleportArea != null)
                surface.teleportArea.enabled = active;
        }
    }

    private void ActivateCurrentFloorSurface()
    {
        if (surfaces == null) return;
        foreach (var surface in surfaces)
        {
            if (surface != null && surface.teleportArea != null)
                surface.teleportArea.enabled = (surface.gravityStep == currentRotationStep);
        }
    }
}

[Serializable]
public class TeleportationSurface
{
    public UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationArea teleportArea;
    [Tooltip("Which gravity rotation step (0-3) this surface is the floor for.")]
    public int gravityStep;
}
