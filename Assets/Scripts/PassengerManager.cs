using System.Collections.Generic;
using UnityEngine;

public class PassengerManager : MonoBehaviour
{
    public static PassengerManager Instance { get; private set; }
    public CarMovement CarMovement;
    public int capacity = 4;
    public Transform[] seats;
    public float dropOffRadius = 8f;
    public float stoppedSpeedThreshold = 10f;

    List<Person> passengers = new List<Person>();
    public int totalDelivered = 0; 

    // ---- GESTÃO DE TEMPO CENTRALIZADA ----
    private float raceTime = 0f;
    private bool raceStarted = false;

    public float RaceTime => raceTime;
    public bool RaceStarted => raceStarted;
    // --------------------------------------

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        if (CarMovement == null)
            CarMovement = GetComponent<CarMovement>();
    }

    public Transform CarTransform => transform;

    public bool IsCarStopped()
    {
        if (CarMovement != null && CarMovement.sphereRB != null)
            return CarMovement.sphereRB.linearVelocity.magnitude <= stoppedSpeedThreshold;
        return true;
    }

    public Transform TryBoard(Person p)
    {
        if (passengers.Count >= capacity)
            return null;
        var seat = GetAvailableSeat();
        if (seat == null)
            return null;
        passengers.Add(p);
        // Dispara o tempo do jogo assim que o primeiro passageiro botar o pé no carro!
        if (!raceStarted)
        {
            raceStarted = true;
            Debug.Log("Primeiro passageiro entrou! Cronômetro iniciado.");
        }
        return seat;
    }

    Transform GetAvailableSeat()
    {
        if (seats == null || seats.Length == 0)
            return null;
        for (int i = 0; i < seats.Length; i++)
        {
            bool used = false;
            foreach (var pas in passengers)
            {
                if (pas != null && pas.transform.parent == seats[i])
                {
                    used = true;
                    break;
                }
            }
            if (!used)
                return seats[i];
        }
        return null;
    }

    void Update()
    {
        // Incrementa o tempo se a corrida já tiver começado
        if (raceStarted)
        {
            raceTime += Time.deltaTime;
        }

        for (int i = passengers.Count - 1; i >= 0; i--)
        {
            var p = passengers[i];
            if (p == null)
            {
                passengers.RemoveAt(i);
                continue;
            }
            if (p.destination == null) continue;

            Collider destCollider = p.destination.GetComponent<Collider>();
            bool arrivedAtDestinationZone = false;

            if (destCollider != null)
            {
                arrivedAtDestinationZone = destCollider.bounds.Contains(transform.position);
            }
            else
            {
                arrivedAtDestinationZone = Vector3.Distance(transform.position, p.destination.position) <= dropOffRadius;
            }

            if (arrivedAtDestinationZone && IsCarStopped())
            {
                passengers.RemoveAt(i);
                p.OnDroppedOff();
                totalDelivered++;
            }
        }
    }

    public int CurrentPassengers => passengers.Count;
}