using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpHeight;

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            gameObject.transform.position = new Vector2(gameObject.transform.position.x + speed * Time.deltaTime, gameObject.transform.position.y);
        }
        if (Input.GetKey(KeyCode.A))
        {
            gameObject.transform.position = new Vector2(gameObject.transform.position.x - speed * Time.deltaTime, gameObject.transform.position.y);
        }
    }
}
