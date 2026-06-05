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

    private bool raceEnded = false;
    private int totalCount = 0;
    private float noPassengerTimer = 0f;
    private float finalSavedTime = 0f; // Guarda o tempo exato de fechamento

    void Start()
    {
        if (panel != null) panel.SetActive(false);

        if (btnRestart != null) btnRestart.onClick.AddListener(RestartRace);
        if (btnMenu != null)    btnMenu.onClick.AddListener(GoToMenu);

        totalCount = totalPassengersToDeliver > 0
        ? totalPassengersToDeliver
        : FindObjectsByType<Person>().Length;
    }

    void Update()
    {
        if (raceEnded) return;

        var pm = PassengerManager.Instance;
        if (pm == null) return;

        // Se a corrida ainda não começou no PassengerManager, exibe a mensagem de espera
        if (!pm.RaceStarted)
        {
            if (runningTimeText != null)
                runningTimeText.text = "Aguardando primeiro passageiro...";
            return;
        }

        // Atualiza o texto da UI lendo diretamente do gerenciador central
        if (runningTimeText != null)
            runningTimeText.text = "Tempo: " + FormatTime(pm.RaceTime);

        // Evita encerrar a corrida nos primeiros instantes de segurança do jogo
        if (pm.RaceTime < 3f) return;

        // Se não tem mais passageiros no carro e o contador de entregas já operou, inicia o encerramento
        if (pm.CurrentPassengers == 0 && pm.totalDelivered > 0)
        {
            noPassengerTimer += Time.deltaTime;
            if (noPassengerTimer >= 3f)
            {
                finalSavedTime = pm.RaceTime; // Salva o tempo final exato
                EndRace();
            }
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
        StartCoroutine(ShowEndScreen());
    }

    IEnumerator ShowEndScreen()
    {
        yield return new WaitForSeconds(1.2f);

        if (panel != null) panel.SetActive(true);

        if (titleText != null)
            titleText.text = "CORRIDA CONCLUÍDA!";

        if (timeText != null)
            timeText.text = "Tempo Final: " + FormatTime(finalSavedTime);

        if (ratingText != null)
            ratingText.text = GetRating(finalSavedTime);
    }

    string FormatTime(float seconds)
    {
        int min = Mathf.FloorToInt(seconds / 60f);
        int sec = Mathf.FloorToInt(seconds % 60f);
        return $"{min:00}:{sec:00}";
    }

    string GetRating(float finalTime)
    {
        float timePerPassenger = totalCount > 0 ? finalTime / totalCount : finalTime;

        if      (timePerPassenger < 15f) return "***** INCRÍVEL!";
        else if (timePerPassenger < 25f) return "**** ÓTIMO!";
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