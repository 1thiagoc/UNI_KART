using TMPro;
using UnityEngine;

public class PassengerUI : MonoBehaviour
{
    public TMP_Text passengerText;
    public Vector2 screenOffset = new Vector2(20f, -20f);
    PassengerManager pm;

    void Start()
    {
        pm = PassengerManager.Instance;

        if (passengerText != null)
        {
            var rt = passengerText.rectTransform;
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = screenOffset;
        }
    }

    void Update()
    {
        if (pm != null && passengerText != null)
            passengerText.text = "Passageiros: " + pm.CurrentPassengers.ToString();
    }
}
