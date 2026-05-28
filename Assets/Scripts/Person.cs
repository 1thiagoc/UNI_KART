using System.Collections;
using UnityEngine;

public class Person : MonoBehaviour
{
    public Transform destination;
    public float boardingDistance = 4f;
    public float checkInterval = 0.25f;

    [HideInInspector] public Collider myStopZoneCollider; // Vaga onde este passageiro está associado
    [HideInInspector] public RuntimeAnimatorController idleController;
    [HideInInspector] public RuntimeAnimatorController sitController;

    private Animator animator;

    bool inCar = false;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
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

                        if (animator != null && sitController != null)
                            animator.runtimeAnimatorController = sitController;

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
            if (destZone != null)
            {
                Transform randomCalçada = destZone.GetRandomSidewalkPoint();
                if (randomCalçada != null)
                    transform.position = randomCalçada.position;
                else
                    transform.position = destination.position + Vector3.up * 1f;
            }
            else
            {
                // Safe-check caso não tenha configurado o ponto da calçada
                transform.position = destination.position + Vector3.up * 1f;
            }
        }

        transform.rotation = Quaternion.identity;
        if (animator != null && idleController != null)
        {
            animator.runtimeAnimatorController = idleController;
        }

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
