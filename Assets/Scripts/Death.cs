using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Death : Controller2D
{

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Death")
        {
            Die();
            Debug.Log("You died well");
            SceneManager.LoadScene("Prototype");

        }
    }
}

    
