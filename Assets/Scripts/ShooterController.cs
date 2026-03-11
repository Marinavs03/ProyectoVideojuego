using UnityEngine;

public class ShooterController : MonoBehaviour
{
    [Header("Proyectil")]
    public GameObject proyectilPrefab;
    public Transform puntoDisparo;
    public float velocidadProyectil = 10f;
    public float cadenciaDisparo = 0.3f;
    public float offsetDisparo = 0.5f;
    private float timerDisparo = 0f;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private PlayerController playerController;

    [Header("Munición")]
    public int municionMaxima = 10;
    public int municionActual;
    public TMPro.TextMeshProUGUI textoMunicion;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();

        municionActual = municionMaxima;
        ActualizarTextoMunicion();
    }

    void Update()
    {
        timerDisparo -= Time.deltaTime;

        if (playerController != null && playerController.enSuelo)
        {
            if (Input.GetMouseButton(0) && timerDisparo <= 0f && !playerController.estaMuerto)
            {
                playerController.puedeMoverse = false;
                playerController.rb.velocity = new Vector2(0f, playerController.rb.velocity.y);

                if (animator != null)
                    animator.SetBool("isShooting", true);

                Disparar();
                timerDisparo = cadenciaDisparo;
            }
            else if (!Input.GetMouseButton(0))
            {
                playerController.puedeMoverse = true;

                if (animator != null)
                    animator.SetBool("isShooting", false);
            }
        }
        else
        {
            if (animator != null)
                animator.SetBool("isShooting", false);

            if (playerController != null)
                playerController.puedeMoverse = true;
        }
    }

    void ActualizarTextoMunicion()
    {
        if (textoMunicion != null)
            textoMunicion.text = municionActual.ToString();
    }

    void Disparar()
    {
        if (municionActual <= 0) return;

        municionActual--;
        ActualizarTextoMunicion();

        Vector2 direccion = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        Vector3 posicionFinal = puntoDisparo.position + (Vector3)(direccion * offsetDisparo);

        GameObject nuevoProyectil = Instantiate(proyectilPrefab, posicionFinal, Quaternion.identity);

        Proyectil proyectilScript = nuevoProyectil.GetComponent<Proyectil>();
        SpriteRenderer proyectilSprite = nuevoProyectil.GetComponent<SpriteRenderer>();

        if (proyectilSprite != null)
            proyectilSprite.flipX = spriteRenderer.flipX;

        proyectilScript.Inicializar(direccion, velocidadProyectil);
    }

    public void Recargar()
    {
        municionActual = municionMaxima;
        ActualizarTextoMunicion();
    }
}