using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SlotMachineManager : MonoBehaviour
{
    [Header("Configuracion de UI")]
    public GameObject slotGameUI;
    public GameObject interactionPromptUI;
    public GameObject efectoJackpot;

    [Header("Iconos y Rodillos")]
    public Sprite[] iconosPosibles; // 0:Cereza, 1:Campana, 2:Bar, 3:Siete
    public Image[] rodillos; // Arrastra los 9 rodillos en orden

    [Header("Ajustes de Pity (Dificultad Alta)")]
    public int pityCerezas = 30;
    public int pityCampanas = 60;
    public int pityBar = 100;
    public int pitySietes = 150;

    [Header("Ajustes de Premios")]
    public int costeApuesta = 20;
    public int[] premiosPorIcono = { 30, 60, 120, 300 };

    [Header("Filtro de Probabilidad")]
    [Range(0, 100)]
    public int probabilidadAzarReal = 8;

    [Header("Sonidos")]
    public AudioSource audioSource;
    public AudioClip sonidoPremio;

    private int c0, c1, c2, c3;
    private int tiradasConBoost = 0;
    private PlayerPersistence playerPersistor;
    private bool isPlaying = false;
    private bool jugadorCerca = false;
    private MonoBehaviour scriptCamaraPersonaje;

    void Start()
    {
        playerPersistor = PlayerPersistence.instance;
        if (slotGameUI != null) slotGameUI.SetActive(false);
        if (interactionPromptUI != null) interactionPromptUI.SetActive(false);
        if (efectoJackpot != null) efectoJackpot.SetActive(false);
    }

    void Update()
    {
        if (slotGameUI != null && slotGameUI.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (jugadorCerca && !isPlaying && Input.GetKeyDown(KeyCode.E))
        {
            EnterSlotGame();
        }
    }

    public void OnSpinButtonClicked()
    {
        if (isPlaying || playerPersistor.coins < costeApuesta) return;

        playerPersistor.AddCoins(-costeApuesta);
        c0++; c1++; c2++; c3++;
        StartCoroutine(SpinReelsRoutine());
    }

    private IEnumerator SpinReelsRoutine()
    {
        isPlaying = true;
        if (efectoJackpot != null) efectoJackpot.SetActive(false);

        float t = 0;
        while (t < 1.5f)
        {
            foreach (Image img in rodillos) img.sprite = iconosPosibles[Random.Range(0, iconosPosibles.Length)];
            t += 0.1f;
            yield return new WaitForSeconds(0.05f);
        }

        int[] res = new int[9];
        int forzar = -1;

        if (c3 >= pitySietes) { forzar = 3; c3 = 0; }
        else if (c2 >= pityBar) { forzar = 2; c2 = 0; }
        else if (c1 >= pityCampanas) { forzar = 1; c1 = 0; }
        else if (c0 >= pityCerezas) { forzar = 0; c0 = 0; }

        if (forzar != -1)
        {
            res[3] = forzar; res[4] = forzar; res[5] = forzar;
            for (int i = 0; i < 9; i++) if (i < 3 || i > 5) res[i] = Random.Range(0, iconosPosibles.Length);
        }
        else
        {
            for (int i = 0; i < 9; i++) res[i] = Random.Range(0, iconosPosibles.Length);

            if (Random.Range(0, 100) > probabilidadAzarReal)
            {
                if (res[3] == res[4] && res[4] == res[5])
                    res[5] = (res[5] + 1) % iconosPosibles.Length;
            }
        }

        for (int i = 0; i < 9; i++) rodillos[i].sprite = iconosPosibles[res[i]];

        CheckPrizes(res);
        isPlaying = false;
    }

    public void ActivarHackerBoost(int cantidadTiradas)
    {
        tiradasConBoost = cantidadTiradas;
        pitySietes = 75;
    }

    void CheckPrizes(int[] r)
    {
        int gain = 0;

        // LÍNEAS HORIZONTALES
        gain += (r[0] == r[1] && r[1] == r[2]) ? premiosPorIcono[r[0]] : 0;
        gain += (r[3] == r[4] && r[4] == r[5]) ? premiosPorIcono[r[3]] : 0;
        gain += (r[6] == r[7] && r[7] == r[8]) ? premiosPorIcono[r[6]] : 0;

        // LÍNEAS VERTICALES
        gain += (r[0] == r[3] && r[3] == r[6]) ? premiosPorIcono[r[0]] : 0;
        gain += (r[1] == r[4] && r[4] == r[7]) ? premiosPorIcono[r[1]] : 0;
        gain += (r[2] == r[5] && r[5] == r[8]) ? premiosPorIcono[r[2]] : 0;

        // LÍNEAS DIAGONALES
        gain += (r[0] == r[4] && r[4] == r[8]) ? premiosPorIcono[r[0]] : 0;
        gain += (r[2] == r[4] && r[4] == r[6]) ? premiosPorIcono[r[2]] : 0;

        if (gain > 0)
        {
            playerPersistor.AddCoins(gain);

            if (audioSource != null && sonidoPremio != null)
            {
                audioSource.clip = sonidoPremio;
                audioSource.Play();
                Invoke("StopSlotAudio", 2f);
            }

            if (gain >= premiosPorIcono[3])
            {
                if (efectoJackpot != null) efectoJackpot.SetActive(true);
            }
        }

        if (tiradasConBoost > 0)
        {
            tiradasConBoost--;
            if (tiradasConBoost <= 0) pitySietes = 150;
        }
    }

    void StopSlotAudio() { if (audioSource != null) audioSource.Stop(); }

    public void EnterSlotGame()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            scriptCamaraPersonaje = player.GetComponentInChildren<MonoBehaviour>();
            if (scriptCamaraPersonaje != null) scriptCamaraPersonaje.enabled = false;
        }
        playerPersistor.DisablePlayerControls();
        slotGameUI.SetActive(true);
        interactionPromptUI.SetActive(false);
    }

    public void ExitSlotGame()
    {
        slotGameUI.SetActive(false);
        playerPersistor.EnablePlayerControls();
        if (scriptCamaraPersonaje != null) scriptCamaraPersonaje.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPlaying = false;
    }

    private void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) { jugadorCerca = true; interactionPromptUI.SetActive(true); } }
    private void OnTriggerExit(Collider other) { if (other.CompareTag("Player")) { jugadorCerca = false; interactionPromptUI.SetActive(false); } }
}