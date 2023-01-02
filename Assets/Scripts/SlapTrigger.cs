using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlapTrigger : MonoBehaviour
{
    static GameObject _player;
    private GameObject _npc;
    private bool _shouldLookAt = false;
    private float _timeRef = 0;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log("Player found: " + _player);
        _npc = transform.parent.gameObject;
    }

    void Update()
    {
        if (_shouldLookAt)
        {
            if (Time.time - _timeRef > 0.5f)
            {
                _shouldLookAt = false;
            }
            _npc.transform.LookAt(_player.transform);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            _shouldLookAt = true;
            _timeRef = Time.time;
            SlapDirection direction = _player.transform.position.x > transform.position.x ? SlapDirection.Left : SlapDirection.Right;
            _player.GetComponent<Character>().Slap(direction);
            _npc.GetComponent<Animator>().SetTrigger("Fall");
        }
    }
}
