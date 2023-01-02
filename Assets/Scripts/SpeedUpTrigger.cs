using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            StopCoroutine(col.gameObject.GetComponent<Character>().SpeedUp());
            StartCoroutine(col.gameObject.GetComponent<Character>().SpeedUp());
        }
    }
}
