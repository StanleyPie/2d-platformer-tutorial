using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedItem : MonoBehaviour, IItems
{
    public static event Action<float> onSpeedCollected;
    public float speedMultiplier = 1.5f;


    public void Collected()
    {
        onSpeedCollected.Invoke(this.speedMultiplier);
        Destroy(gameObject);
    }
}
