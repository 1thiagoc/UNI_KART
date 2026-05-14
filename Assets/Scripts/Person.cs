using System.Collections;
using UnityEngine;

public class Person : MonoBehaviour
{
    public Transform destination;
    public float boardingDistance = 4f;
    public float checkInterval = 0.25f;

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
            if (pm != null && pm.CarTransform != null)
            {
                float dist = Vector3.Distance(transform.position, pm.CarTransform.position);
                if (dist <= boardingDistance && pm.IsCarStopped())
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
            transform.position = destination.position + destination.right * 2f + Vector3.up * 1f;
        }

        transform.rotation = Quaternion.Euler(0f, 0f, 0f);

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
