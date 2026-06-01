using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RaceEndScreen : MonoBehaviour
{
    [Header("Painel")]
    public GameObject panel;

    [Header("Textos")]
    public TMP_Text titleText;
    public TMP_Text timeText;
    public TMP_Text ratingText;
    public TMP_Text runningTimeText;

    [Header("Botões")]
    public Button btnRestart;
    public Button btnMenu;

    [Header("Configuração")]
    public string menuSceneName = "Menu";
    public int totalPassengersToDeliver = 0;

    private float raceTime = 0f;
    private bool raceActive = false;
    private bool raceEnded = false;
    private int totalCount = 0;

    void Start()
    {
        if (panel != null) panel.SetActive(false);

        if (btnRestart != null) btnRestart.onClick.AddListener(RestartRace);
        if (btnMenu != null)    btnMenu.onClick.AddListener(GoToMenu);

        totalCount = totalPassengersToDeliver > 0
            ? totalPassengersToDeliver
            : FindObjectsByType<Person>(FindObjectsSortMode.None).Length;

        raceActive = true;
    }

   private float noPassengerTimer = 0f;

    void Update()
    {
        if (!raceActive || raceEnded) {
            runningTimeText.text = "Corrida não iniciada";
            return;
        }

        raceTime += Time.deltaTime;
        runningTimeText.text = "Tempo: " + FormatTime(raceTime);
        if (raceTime < 3f) return;

        var pm = PassengerManager.Instance;
        if (pm == null) return;

        // Se não tem passageiro no carro e contador parou, fim de corrida
        if (pm.CurrentPassengers == 0 && pm.totalDelivered > 0)
        {
            noPassengerTimer += Time.deltaTime;
            if (noPassengerTimer >= 3f)
                EndRace();
        }
        else
        {
            noPassengerTimer = 0f;
        }
    }
    void EndRace()
    {
        if (raceEnded) return;
        raceEnded = true;
        raceActive = false;
        StartCoroutine(ShowEndScreen());
    }

    IEnumerator ShowEndScreen()
    {
        yield return new WaitForSeconds(1.2f);

        if (panel != null) panel.SetActive(true);

        if (titleText != null)
            titleText.text = "CORRIDA CONCLUÍDA!";

        if (timeText != null)
            timeText.text = "Tempo: " + FormatTime(raceTime);

        if (ratingText != null)
            ratingText.text = GetRating();
    }

    string FormatTime(float seconds)
    {
        int min = Mathf.FloorToInt(seconds / 60f);
        int sec = Mathf.FloorToInt(seconds % 60f);
        return $"{min:00}:{sec:00}";
    }

    string GetRating()
    {
        float timePerPassenger = totalCount > 0 ? raceTime / totalCount : raceTime;

        if      (timePerPassenger < 15f) return "***** INCRIVEL!";
        else if (timePerPassenger < 25f) return "**** OTIMO!";
        else if (timePerPassenger < 40f) return "*** BOM!";
        else if (timePerPassenger < 60f) return "** OK";
        else                             return "* PODE MELHORAR";
    }

    void RestartRace()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}