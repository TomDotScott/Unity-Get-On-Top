using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum PickupType
    {
        BiggerShape,
        SmallerShape,
        Triangle,
        MoreSpeed,
        MoreJump
    }

    public PickupType pickupType;

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
            Destroy(gameObject);
        }
    }
}
