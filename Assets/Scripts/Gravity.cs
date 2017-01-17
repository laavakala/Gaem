using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{

    void Start()
    {
        Physics2D.gravity = new Vector2(0.0f, -9.8f);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            Physics2D.gravity = new Vector2(0.0f, 9.8f);
            Debug.Log("Gravity wooo");
        }
    }

}
