using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;
    public float multiplicadorSprint = 1.8f;
    public float fuerzaSalto = 8f;

    [HideInInspector]
    public Vector3 UltimoCheckpoint;
    [HideInInspector]
    public bool puedeMoverse = true;

    [Header("Vida y Energía")]
    public int vidaMaxima = 3;
    public int vidaActual;
    public float energiaMaxima = 100f;
    public float energiaActual;
    public float gastoEnergia = 20f;
    public float recuperacionEnergia = 35f;

    [Header("Cooldown de daño")]
    public float tiempoInvulnerable = 1.0f;
    private float timerInvulnerable = 0f;
    private bool puedeRecibirDaño = true;
    private bool primerGolpe = false;

    [Header("Componentes")]
    public Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    [HideInInspector]
    public bool enSuelo;
    public bool estaMuerto = false;

    public Transform comprobadorSuelo;
    public float radioSuelo = 1f;
    public LayerMask capaSuelo;

    [Header("UI Corazones")]
    public Image corazon1;
    public Image corazon2;
    public Image corazon3;
    public Sprite corazonLleno;
    public Sprite corazonRoto;

    public Slider barraEnergia;

    [Header("Animator")]
    public Animator animator;

    [Header("Paneles de inicio/fade")]
    public GameObject panelHeli;
    public GameObject panelObj;
    public float fadeDuration = 1f;
    public float mostrarTiempo = 2f;

    void Start()
    {
        MostrarPanel();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        vidaActual = vidaMaxima;
        energiaActual = energiaMaxima;

        if (barraEnergia != null) barraEnergia.maxValue = energiaMaxima;

        ActualizarUI();

        UltimoCheckpoint = transform.position;
    }

    void Update()
    {
        if (!estaMuerto)
        {
            Mover();
            ActualizarAnimator();
        }

        if (!puedeRecibirDaño && !estaMuerto)
        {
            timerInvulnerable -= Time.deltaTime;

            if (primerGolpe)
            {
                if (timerInvulnerable <= tiempoInvulnerable - 0.15f)
                    primerGolpe = false;
            }
            else
            {
                float frecuencia = 3f;
                float alpha = Mathf.Abs(Mathf.Sin((tiempoInvulnerable - timerInvulnerable) / tiempoInvulnerable * Mathf.PI * frecuencia));
                alpha = Mathf.Lerp(0.3f, 1f, alpha);
                spriteRenderer.color = new Color(1f, 1f, 1f, alpha);
            }

            if (timerInvulnerable <= 0f)
            {
                puedeRecibirDaño = true;
                spriteRenderer.color = Color.white;
            }
        }
    }

    void Mover()
    {
        if (!puedeMoverse) return;

        enSuelo = Physics2D.OverlapCircle(comprobadorSuelo.position, radioSuelo, capaSuelo);
        float x = Input.GetAxis("Horizontal");
        bool quiereSprint = Input.GetKey(KeyCode.LeftShift) && energiaActual > 0f;
        float velocidadActual = velocidad * (quiereSprint ? multiplicadorSprint : 1f);

        rb.velocity = new Vector2(x * velocidadActual, rb.velocity.y);

        if (quiereSprint)
            energiaActual -= gastoEnergia * Time.deltaTime;
        else
            energiaActual += recuperacionEnergia * Time.deltaTime;

        energiaActual = Mathf.Clamp(energiaActual, 0f, energiaMaxima);

        if (Input.GetButtonDown("Jump") && enSuelo)
            rb.velocity = new Vector2(rb.velocity.x, fuerzaSalto);

        if (x > 0.1f) spriteRenderer.flipX = false;
        else if (x < -0.1f) spriteRenderer.flipX = true;

        ActualizarUI();
    }

    void ActualizarAnimator()
    {
        float velocidadHorizontal = Mathf.Abs(rb.velocity.x);
        float velocidadAnimActual = animator.GetFloat("Speed");
        float velocidadSuave = Mathf.Lerp(velocidadAnimActual, velocidadHorizontal, Time.deltaTime * 10f);
        animator.SetFloat("Speed", velocidadSuave);
        animator.SetBool("IsGrounded", enSuelo);
    }

    public void RecibirDaño(int cantidad)
    {
        if (!puedeRecibirDaño) return;

        vidaActual -= cantidad;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

        ActualizarUI();

        if (vidaActual <= 0)
        {
            estaMuerto = true;
            animator.SetBool("IsDead", true);
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
            StartCoroutine(MorirConDelay(3f));
        }
        else
        {
            puedeRecibirDaño = false;
            timerInvulnerable = tiempoInvulnerable;

            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(1f, 0.3f, 0.3f, 0.6f);
                primerGolpe = true;
            }
        }
    }

    IEnumerator MorirConDelay(float delay)
    {
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene("Game_Over");
    }

    public void ActualizarUI()
    {
        corazon1.sprite = vidaActual >= 1 ? corazonLleno : corazonRoto;
        corazon2.sprite = vidaActual >= 2 ? corazonLleno : corazonRoto;
        corazon3.sprite = vidaActual >= 3 ? corazonLleno : corazonRoto;

        if (barraEnergia != null)
            barraEnergia.value = energiaActual;
    }

    void OnDrawGizmosSelected()
    {
        if (comprobadorSuelo != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(comprobadorSuelo.position, radioSuelo);
        }
    }

    public void MostrarPanel()
    {
        if (panelHeli != null)
            StartCoroutine(MostrarPanelCoroutine(panelHeli));

        if (panelObj != null)
            StartCoroutine(MostrarPanelCoroutine(panelObj));
    }

    private IEnumerator MostrarPanelCoroutine(GameObject panel)
    {
        panel.SetActive(true);
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = panel.AddComponent<CanvasGroup>();

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(mostrarTiempo);

        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        panel.SetActive(false);
    }
}