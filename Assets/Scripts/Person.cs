using System.Collections;
using UnityEngine;

public class Person : MonoBehaviour
{
    public Transform destination;
    public float boardingDistance = 4f;
    public float checkInterval = 0.25f;

    [HideInInspector] public Collider myStopZoneCollider; // Vaga onde este passageiro está associado

    bool inCar = false;

    void Start()
    {
        StartCoroutine(CheckForCar());
    }

    IEnumerator CheckForCar()
    {
        while (!inCar)
        {
            var pm = PassengerManager.Instance;
            if (pm != null && pm.CarTransform != null && myStopZoneCollider != null)
            {
                bool carInZone = myStopZoneCollider.bounds.Contains(pm.CarTransform.position);

                if (carInZone && pm.IsCarStopped())
                {
                    var seat = pm.TryBoard(this);
                    if (seat != null)
                    {
                        inCar = true;
                        transform.SetParent(seat, true); // mantém posição mundial ao parental
                        transform.localPosition = Vector3.zero;
                        transform.localRotation = Quaternion.identity;

                        // esconde o cilindro ao embarcar
                        var rend = GetComponent<Renderer>();
                        if (rend != null)
                            rend.enabled = false;

                        yield break;
                    }
                }
            }
            yield return new WaitForSeconds(checkInterval);
        }
    }

    public void OnDroppedOff()
    {
        transform.SetParent(null, true);

        if (destination != null)
        {
            // Tenta achar o ponto da calçada da zona de destino
            StopZone destZone = destination.GetComponent<StopZone>();
            if (destZone != null && destZone.sidewalkPoint != null)
            {
                transform.position = destZone.sidewalkPoint.position;
            }
            else
            {
                // Safe-check caso não tenha configurado o ponto da calçada
                transform.position = destination.position + Vector3.up * 1f;
            }
        }

        transform.rotation = Quaternion.identity;

        var rend = GetComponent<Renderer>();
        if (rend != null)
            rend.enabled = true;

        Destroy(gameObject, 1.5f);
    }

    IEnumerator ShowAfterDelay()
    {
        yield return null; // espera 1 frame

        var rend = GetComponent<Renderer>();
        if (rend != null)
            rend.enabled = true;

        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
}
