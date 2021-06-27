using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{

    public CollectableType type;
    public int value = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            switch (type)
            {
                case CollectableType.Live:
                    Player.sharedInstance.PlayerHealth(this.value);
                    Destroy(gameObject);
                    break;
            }
        }
    }

}
