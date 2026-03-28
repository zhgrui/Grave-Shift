using UnityEngine;

public class TeleportCooldown : MonoBehaviour
{
    [Tooltip("Seconds before the player can teleport again.")]
    [SerializeField] private float cooldownDuration = 2f;

    private float cooldownTimer;

    public bool IsOnCooldown => cooldownTimer > 0f;

    public void StartCooldown()
    {
        cooldownTimer = cooldownDuration;
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }
}
