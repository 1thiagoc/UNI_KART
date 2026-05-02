using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MenuPausa : MonoBehaviour
{
    public GameObject painelPausa;

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
        painelPausa.SetActive(true);
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