using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Configuracoes : MonoBehaviour
{
    [Header("Sliders")]
    public Slider sliderMusica;
    public Slider sliderEfeitos;

    [Header("Dropdown")]
    public TMP_Dropdown dropdownIdioma;

    void Start()
    {
        // Carrega valores salvos ou usa padrão 1.0
        sliderMusica.value  = PlayerPrefs.GetFloat("VolMusica",  1f);
        sliderEfeitos.value = PlayerPrefs.GetFloat("VolEfeitos", 1f);
        dropdownIdioma.value = PlayerPrefs.GetInt("Idioma", 0);

        sliderMusica.onValueChanged.AddListener(MudarMusica);
        sliderEfeitos.onValueChanged.AddListener(MudarEfeitos);
        dropdownIdioma.onValueChanged.AddListener(MudarIdioma);
    }

    void MudarMusica(float valor)
    {
        AudioListener.volume = valor;
        PlayerPrefs.SetFloat("VolMusica", valor);
    }

    void MudarEfeitos(float valor)
    {
        PlayerPrefs.SetFloat("VolEfeitos", valor);
    }

    void MudarIdioma(int index)
    {
        PlayerPrefs.SetInt("Idioma", index);
    }

    void OnDestroy()
    {
        PlayerPrefs.Save();
    }
}