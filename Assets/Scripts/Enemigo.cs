using UnityEngine;

public class Enemigo : MonoBehaviour
{
    public int vidaMaxima = 8;
    private int vidaActual;

    public float velocidad = 2f;
    private bool moviendoDerecha = true;

    [Header("Ataque")]
    public float tiempoEntreAtaques = 1.5f;
    private float temporizadorAtaque;

    public Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    Collider2D collider2d;

    [Header("Drop")]
    public GameObject prefabDrop;
    [Range(0f, 1f)]
    public float probabilidadDrop = 0.3f;
    public Transform puntoDrop;

    [Header("Detección Jugador")]
    public string tagJugador = "Player";
    public Transform puntoDeteccion;
    public float radioDeteccion = 2f;
    public LayerMask capaJugador;

    private bool jugadorDetectado;

    void Start()
    {
        vidaActual = vidaMaxima;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider2d = GetComponent<Collider2D>();
    }

    void Update()
    {
        DetectarJugador();
        Mover();
    }

    void Mover()
    {
        if (jugadorDetectado)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        rb.velocity = moviendoDerecha ? new Vector2(velocidad, rb.velocity.y) : new Vector2(-velocidad, rb.velocity.y);
    }

    void DetectarJugador()
    {
        if (puntoDeteccion == null) return;

        Collider2D jugador = Physics2D.OverlapCircle(puntoDeteccion.position, radioDeteccion, capaJugador);

        if (jugador != null)
        {
            jugadorDetectado = true;
            temporizadorAtaque -= Time.deltaTime;

            if (temporizadorAtaque <= 0f)
            {
                animator.SetTrigger("Atacar");
                PlayerController player = jugador.GetComponent<PlayerController>();
                if (player != null)
                    player.RecibirDaño(1);
                temporizadorAtaque = tiempoEntreAtaques;
            }
        }
        else
        {
            jugadorDetectado = false;
            temporizadorAtaque = 0f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Suelo"))
            CambiarDireccion();
    }

    void CambiarDireccion()
    {
        moviendoDerecha = !moviendoDerecha;
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    private void OnDrawGizmosSelected()
    {
        if (puntoDeteccion != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(puntoDeteccion.position, radioDeteccion);
        }
    }

    public void RecibirDaño(int cantidad)
    {
        vidaActual -= cantidad;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

        if (spriteRenderer != null)
            StartCoroutine(ParpadeoDaño());

        if (vidaActual <= 0)
            Morir();
    }

    private System.Collections.IEnumerator ParpadeoDaño()
    {
        if (spriteRenderer == null) yield break;

        Color colorOriginal = spriteRenderer.color;
        Color colorGolpe = new Color(1f, 0.3f, 0.3f, 0.6f);

        spriteRenderer.color = colorGolpe;
        yield return new WaitForSeconds(0.15f);
        spriteRenderer.color = colorOriginal;
    }

    void Morir()
    {
        if (prefabDrop != null && puntoDrop != null && Random.value <= probabilidadDrop)
            Instantiate(prefabDrop, puntoDrop.position, Quaternion.identity);

        if (animator != null)
            animator.SetTrigger("Morir");

        rb.velocity = Vector2.zero;

        Destroy(collider2d);
        Destroy(rb);

        this.enabled = false;
    }
}