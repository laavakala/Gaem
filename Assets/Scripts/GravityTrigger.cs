using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityTrigger : MonoBehaviour {

    public Player player;


   void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Player")
        {
            player = col.gameObject.GetComponent<Player>();
            player.ReverseGravity();
            Debug.Log("Gravity trigger destroyed");
            Destroy(gameObject);

        }
    
  
    }

}
