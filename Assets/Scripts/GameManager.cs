using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Vector3 UltimoCheckpoint;

    [Header("UI Panels")]
    public GameObject panelMenu;
    public GameObject panelGameOver;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += EscenaCargada;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            string escenaActual = SceneManager.GetActiveScene().name;

            if (escenaActual == "Menu")
            {
                SceneManager.LoadScene("Game");
            }
            else if (escenaActual == "Game_Over")
            {
                SceneManager.LoadScene("Game");
                SceneManager.sceneLoaded += PosicionarJugadorEnCheckpoint;
            }
        }
    }

    private void EscenaCargada(Scene escena, LoadSceneMode modo)
    {
        if (panelMenu != null) panelMenu.SetActive(false);
        if (panelGameOver != null) panelGameOver.SetActive(false);

        switch (escena.name)
        {
            case "Menu":
                if (panelMenu != null) panelMenu.SetActive(true);
                break;
            case "Game_Over":
                if (panelGameOver != null) panelGameOver.SetActive(true);
                break;
        }
    }

    private void PosicionarJugadorEnCheckpoint(Scene escena, LoadSceneMode modo)
    {
        if (escena.name != "Game") return;

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.transform.position = GameManager.Instance.UltimoCheckpoint;
            player.vidaActual = player.vidaMaxima;
            player.energiaActual = player.energiaMaxima;
            player.ActualizarUI();
            player.estaMuerto = false;
            player.rb.isKinematic = false;
            player.puedeMoverse = true;
            player.animator.SetBool("IsDead", false);
        }

        SceneManager.sceneLoaded -= PosicionarJugadorEnCheckpoint;
    }
}