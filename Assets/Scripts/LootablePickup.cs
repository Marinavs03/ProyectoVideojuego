using UnityEngine;

public class LootablePickup : MonoBehaviour
{
    public enum TipoLoot
    {
        Vida,
        Municion
    }

    [Header("Configuración de loot")]
    public TipoLoot tipo = TipoLoot.Vida;

    [Header("Valores")]
    public int cantidadCuracion = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        ShooterController shooter = other.GetComponent<ShooterController>();

        if (player == null) return;

        switch (tipo)
        {
            case TipoLoot.Vida:
                if (player.vidaActual < player.vidaMaxima)
                {
                    player.vidaActual += cantidadCuracion;
                    player.vidaActual = Mathf.Clamp(player.vidaActual, 0, player.vidaMaxima);
                    player.SendMessage("ActualizarUI");
                    Destroy(gameObject);
                }
                break;

            case TipoLoot.Municion:
                if (shooter != null)
                {
                    shooter.Recargar();
                    Destroy(gameObject);
                }
                break;
        }
    }
}