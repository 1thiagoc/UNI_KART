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
    public int totalDelivered = 0; // NOVO

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
                totalDelivered++; // NOVO
            }
        }
    }

    public int CurrentPassengers => passengers.Count;
}