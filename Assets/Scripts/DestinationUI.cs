using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DestinationUI : MonoBehaviour
{
    [Header("Referências de UI")]
    public GameObject panel;
    public Image arrowImage;
    public TMP_Text destinationNameText;
    public TMP_Text distanceText;

    [Header("Referências de Cena")]
    public PassengerManager passengerManager;
    public Transform playerTransform;

    [Header("Configuração")]
    public string[] destinationNames;
    public float arrivedThreshold = 10f;

    private Transform currentDestination;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        if (passengerManager == null)
            passengerManager = PassengerManager.Instance;
        if (playerTransform == null && passengerManager != null)
            playerTransform = passengerManager.CarTransform;
        if (panel != null) panel.SetActive(false);
    }

    void Update()
    {
        if (passengerManager == null) return;
        Person passengerOnBoard = GetFirstPassengerOnBoard();
        if (passengerOnBoard == null || passengerOnBoard.destination == null)
        {
            if (panel != null) panel.SetActive(false);
            currentDestination = null;
            return;
        }
        if (panel != null) panel.SetActive(true);
        currentDestination = passengerOnBoard.destination;
        UpdateArrow();
        UpdateDestinationName();
        UpdateDistance();
    }

    void UpdateArrow()
    {
        if (arrowImage == null || playerTransform == null || currentDestination == null) return;
        Vector3 dir = currentDestination.position - playerTransform.position;
        dir.y = 0f;
        Vector3 screenDir = mainCam.WorldToScreenPoint(playerTransform.position + dir.normalized * 5f)
                            - mainCam.WorldToScreenPoint(playerTransform.position);
        screenDir.z = 0f;
        float angle = Mathf.Atan2(screenDir.y, screenDir.x) * Mathf.Rad2Deg - 90f;
        arrowImage.rectTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
        float dist = dir.magnitude;
        arrowImage.color = dist <= arrivedThreshold ? Color.green : Color.white;
    }

    void UpdateDestinationName()
    {
        if (destinationNameText == null || currentDestination == null) return;
        destinationNameText.text = GetFriendlyName(currentDestination);
    }

    void UpdateDistance()
    {
        if (distanceText == null || playerTransform == null || currentDestination == null) return;
        float dist = Vector3.Distance(playerTransform.position, currentDestination.position);
        distanceText.text = Mathf.RoundToInt(dist) + " m";
    }

    string GetFriendlyName(Transform dest)
    {
        if (destinationNames != null && destinationNames.Length > 0)
        {
            var spawner = FindFirstObjectByType<PersonSpawner>();
            if (spawner != null && spawner.destinationPoints != null)
            {
                for (int i = 0; i < spawner.destinationPoints.Length; i++)
                {
                    if (spawner.destinationPoints[i] == dest && i < destinationNames.Length)
                        return destinationNames[i];
                }
            }
        }
        return dest.name;
    }

    Person GetFirstPassengerOnBoard()
{
    if (passengerManager == null) return null;
    if (passengerManager.seats == null) return null;
    foreach (var seat in passengerManager.seats)
    {
        if (seat == null) continue;
        if (seat.childCount > 0)
        {
            var person = seat.GetComponentInChildren<Person>();
            if (person != null && person.destination != null)
                return person;
        }
    }
    return null;
}

    void OnValidate()
    {
        var spawner = FindFirstObjectByType<PersonSpawner>();
        if (spawner != null && spawner.destinationPoints != null && destinationNames != null)
        {
            if (destinationNames.Length != spawner.destinationPoints.Length)
                Debug.LogWarning(
                    $"[DestinationUI] destinationNames tem {destinationNames.Length} entradas " +
                    $"mas PersonSpawner tem {spawner.destinationPoints.Length} destinationPoints.");
        }
    }
}