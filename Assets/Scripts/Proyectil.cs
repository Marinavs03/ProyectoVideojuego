using UnityEngine;

public class Proyectil : MonoBehaviour
{
    private Rigidbody2D rb;
    private float velocidad;
    private Vector2 direccion;

    public int daño = 1;
    public float tiempoVida = 5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, tiempoVida);
    }

    public void Inicializar(Vector2 dir, float vel)
    {
        direccion = dir.normalized;
        velocidad = vel;
    }

    void FixedUpdate()
    {
        rb.velocity = direccion * velocidad;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Suelo"))
        {
            Destroy(gameObject);
        }
        else if (col.CompareTag("Enemigo"))
        {
            Enemigo enemigo = col.GetComponent<Enemigo>();
            if (enemigo != null)
                enemigo.RecibirDaño(daño);
            Destroy(gameObject);
        }
    }
}