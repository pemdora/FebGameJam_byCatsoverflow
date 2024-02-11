using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    [SerializeField] private int _duration;

    public int Duration { get => _duration; set => _duration = value; }
    public void ActivateCargo()
    {
        Cargo cargo = GetComponentInChildren<Cargo>();
        if (cargo)
        {
            cargo.ActivateCargo();
        }
    }

    public void DeactivateCargo()
    {
        Cargo cargo = GetComponentInChildren<Cargo>();
        if (cargo)
        {
            cargo.DeactivateCargo();
        }
    }
}
