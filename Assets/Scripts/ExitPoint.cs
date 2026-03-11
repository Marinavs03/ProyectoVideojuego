using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ExitPoint : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject panelMenu;

    [Header("Tiempo antes de volver al menú")]
    public float tiempoEspera = 3f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
            player.puedeMoverse = false;

        StartCoroutine(ActivarPanelYVolverAlMenu());
    }

    private IEnumerator ActivarPanelYVolverAlMenu()
    {
        if (panelMenu != null)
            panelMenu.SetActive(true);

        yield return new WaitForSeconds(tiempoEspera);

        SceneManager.LoadScene("Menu");
    }
}