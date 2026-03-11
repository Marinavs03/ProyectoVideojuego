using UnityEngine;

public class TrapController : MonoBehaviour
{
    [Header("Daño")]
    public int daño = 1;

    [Header("Cambio de estado")]
    public Sprite spriteActivado;

    private SpriteRenderer spriteRenderer;
    private Collider2D trapCollider;
    private bool yaActivada = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        trapCollider = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (yaActivada) return;

        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
                player.RecibirDaño(daño);

            ActivarTrampa();
        }
    }

    void ActivarTrampa()
    {
        yaActivada = true;

        if (spriteActivado != null && spriteRenderer != null)
            spriteRenderer.sprite = spriteActivado;

        if (trapCollider != null)
            trapCollider.enabled = false;
    }
}