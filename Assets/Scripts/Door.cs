using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Animator anim;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Player.sharedInstance.door = this.transform;
            Player.sharedInstance.canHideInDoor = true;
            Player.sharedInstance.doorAnim = this.anim;
        }
        

        
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            //Player.sharedInstance.door = null;
            Player.sharedInstance.canHideInDoor = false;
            //Player.sharedInstance.doorAnim = null;
        }

    }
}
