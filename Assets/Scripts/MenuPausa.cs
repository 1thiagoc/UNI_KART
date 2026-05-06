using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MenuPausa : MonoBehaviour
{
    public GameObject painelPausa;
    public AudioClip pauseSfx;

    private bool pausado = false;

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (pausado) Retomar();
            else         Pausar();
        }
    }

    public void Pausar()
    {
        Debug.Log("MenuPausa: Pausar() called");
        painelPausa.SetActive(true);
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("MenuPausa: AudioManager.Instance is null when trying to play pause SFX");
        }
        else if (pauseSfx == null)
        {
            Debug.LogWarning("MenuPausa: pauseSfx is not assigned in Inspector");
        }
        else
        {
            AudioManager.Instance.PlaySFX(pauseSfx);
        }
        Time.timeScale = 0f;
        pausado = true;
    }

    public void Retomar()
    {
        painelPausa.SetActive(false);
        Time.timeScale = 1f;
        pausado = false;
    }

    public void VoltarAoMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}