using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Painéis")]
    public GameObject painelMenu;
    public GameObject painelConfiguracoes;

    [Header("Tela de Fade")]
    public Image telaFade;

    [Header("Botões em cascata")]
    public CanvasGroup[] botoesMenu;

    void Start()
    {
        painelConfiguracoes.SetActive(false);
        painelMenu.SetActive(false);
        StartCoroutine(SequenciaAbertura());
    }

    void Update()
{
    if (Keyboard.current.spaceKey.wasPressedThisFrame)
    {
        Debug.Log("Espaço pressionado!");
        SceneManager.LoadScene(1);
    }
}

    IEnumerator SequenciaAbertura()
    {
        telaFade.color = new Color(0, 0, 0, 1);
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FadeAlpha(1f, 0f, 2f));
        yield return new WaitForSeconds(0.5f);
        painelMenu.SetActive(true);
        yield return StartCoroutine(BotoesEmCascata());
    }

    IEnumerator FadeAlpha(float de, float para, float duracao)
    {
        float t = 0;
        while (t < duracao)
        {
            t += Time.deltaTime;
            telaFade.color = new Color(0, 0, 0, Mathf.Lerp(de, para, t / duracao));
            yield return null;
        }
        telaFade.color = new Color(0, 0, 0, para);
        if (para == 0f)
            telaFade.gameObject.SetActive(false);
    }

    IEnumerator BotoesEmCascata()
    {
        foreach (CanvasGroup btn in botoesMenu)
        {
            btn.alpha = 0f;
            btn.transform.localScale = Vector3.one * 0.7f;
        }
        foreach (CanvasGroup btn in botoesMenu)
        {
            StartCoroutine(AnimarBotao(btn));
            yield return new WaitForSeconds(0.15f);
        }
    }

    IEnumerator AnimarBotao(CanvasGroup cg)
    {
        float t = 0, dur = 0.3f;
        while (t < dur)
        {
            t += Time.deltaTime;
            float p = t / dur;
            cg.alpha = p;
            cg.transform.localScale = Vector3.Lerp(Vector3.one * 0.7f, Vector3.one, p);
            yield return null;
        }
        cg.alpha = 1f;
        cg.transform.localScale = Vector3.one;
    }

    public void AoBotaoJogar()
{
    SceneManager.LoadScene(1);
}

    public void AoBotaoContinuar()
    {
        AoBotaoJogar();
    }

    public void AoBotaoConfiguracoes()
    {
        painelMenu.SetActive(false);
        painelConfiguracoes.SetActive(true);
    }

    public void FecharConfiguracoes()
    {
        painelConfiguracoes.SetActive(false);
        painelMenu.SetActive(true);
    }
}