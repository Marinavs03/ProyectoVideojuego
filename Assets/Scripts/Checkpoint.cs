using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    [Header("UI Notificación")]
    public GameObject panelMensaje;
    public float fadeDuration = 1.5f;
    public float mostrarTiempo = 2f;

    [Header("Sprite del Checkpoint")]
    public SpriteRenderer spriteCheckpoint;
    public int parpadeos = 3;
    public float parpadeoRapido = 0.1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            GameManager.Instance.UltimoCheckpoint = transform.position;
            Debug.Log("Checkpoint activado! Nuevo checkpoint: " + GameManager.Instance.UltimoCheckpoint);

            if (panelMensaje != null)
            {
                StartCoroutine(MostrarMensaje());
            }

            if (spriteCheckpoint != null)
            {
                StartCoroutine(ParpadearSprite());
            }
        }
    }

    private IEnumerator MostrarMensaje()
    {
        panelMensaje.SetActive(true);

        CanvasGroup canvasGroup = panelMensaje.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = panelMensaje.AddComponent<CanvasGroup>();
        }

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

        panelMensaje.SetActive(false);
    }

    private IEnumerator ParpadearSprite()
    {
        Color colorOriginal = spriteCheckpoint.color;
        for (int i = 0; i < parpadeos; i++)
        {
            spriteCheckpoint.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, 0f);
            yield return new WaitForSeconds(parpadeoRapido);
            spriteCheckpoint.color = colorOriginal;
            yield return new WaitForSeconds(parpadeoRapido);
        }
        spriteCheckpoint.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, 0f);
    }
}