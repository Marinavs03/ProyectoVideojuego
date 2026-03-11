using UnityEngine;

public class Pausa : MonoBehaviour
{
    public GameObject canvasPausa;
    private bool juegoPausado = false;

    void Start()
    {
        if (canvasPausa != null)
            canvasPausa.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (juegoPausado)
                ReanudarJuego();
            else
                PausarJuego();
        }
    }

    void PausarJuego()
    {
        Time.timeScale = 0f;
        juegoPausado = true;
        if (canvasPausa != null)
            canvasPausa.SetActive(true);
    }

    void ReanudarJuego()
    {
        Time.timeScale = 1f;
        juegoPausado = false;
        if (canvasPausa != null)
            canvasPausa.SetActive(false);
    }
}