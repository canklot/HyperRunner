using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationState
{
    Talking = 1,
    Talking2,
    Yelling,
    TellingASecret,
    Arguing,
    StandingArguing,
    HappyWalk,
    WalkingCircle,
    Rapping,
    SalsaDancing,
    Clapping,
    Cheering
}

public class NPCController : MonoBehaviour
{
    public AnimationState Animation;
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.SetInteger("Animation", (int)Animation);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
