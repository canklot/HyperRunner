using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperJumpTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<Character>().SuperJump();
        }
    }
}
