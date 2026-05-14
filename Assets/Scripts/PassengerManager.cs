using System.Collections.Generic;
using UnityEngine;

public class PassengerManager : MonoBehaviour
{
    public static PassengerManager Instance { get; private set; }
    public CarMoviment carMoviment;
    public int capacity = 4;
    public Transform[] seats;
    public float dropOffRadius = 8f;
    public float stoppedSpeedThreshold = 1.2f;

    List<Person> passengers = new List<Person>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        if (carMoviment == null)
            carMoviment = GetComponent<CarMoviment>();
    }

    public Transform CarTransform => transform;

    public bool IsCarStopped()
    {
        if (carMoviment != null && carMoviment.sphereRB != null)
            return carMoviment.sphereRB.linearVelocity.magnitude <= stoppedSpeedThreshold;
        return true;
    }

    // Try to board a person; returns assigned seat Transform or null if boarding failed
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
        // check drop-off conditions per passenger
        for (int i = passengers.Count - 1; i >= 0; i--)
        {
            var p = passengers[i];
            if (p == null)
            {
                passengers.RemoveAt(i);
                continue;
            }
            if (p.destination == null)
                continue;
            float d = Vector3.Distance(transform.position, p.destination.position);
            Debug.Log($"Passenger {p.name} | dist={d} | stopped={IsCarStopped()}");
            if (d <= dropOffRadius && IsCarStopped())
            {
                // drop off
                passengers.RemoveAt(i);
                p.OnDroppedOff();
            }
        }
    }

    public int CurrentPassengers => passengers.Count;
}
