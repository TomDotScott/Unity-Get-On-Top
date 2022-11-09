using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum PickupType
    {
        Normal,
        BiggerShape,
        SmallerShape,
        Triangle,
        MoreSpeed,
        MoreJump
    }

    public PickupType pickupType;
    public float powerupDuration;

    [SerializeField] private float lifeTime;
    private float activeTimer;

    private void Awake()
    {
        activeTimer = lifeTime;
    }

    private void Update()
    {
        activeTimer -= Time.deltaTime;
        if(activeTimer <= 0)
        {
            Kill();
        }
    }

    public void Kill()
    {
        Destroy(gameObject);
    }
}
