using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//git test
enum HorizontalMovement
{
    Left,
    Right,
    None
}

public enum SlapDirection
{
    Left,
    Right
}

public class Character : MonoBehaviour
{
    private Animator _animator;
    private CharacterController _controller;
    private HorizontalMovement _horizontalMovement = HorizontalMovement.None;
    private float _terminalVelocity = 53.0f;
    private float _verticalVelocity;
    private Touch _touch;
    private bool _dragStarted;
    private float _xMoveSpeed = 5f;
    private float _rotationSpeed = 100f;
    private float _rotationAngle = 15f;
    private Quaternion _rightRotation;
    private Quaternion _leftRotation;
    private Quaternion _initialRotation;
    private float _leftSlapRef = 0;
    private float _rightSlapRef = 0;


    public float FastRunMoveSpeed = 12.0f;
    public float FastRunDuration = 3000f;
    public float Gravity = -15.0f;
    public bool Grounded = true;
    public LayerMask GroundLayers;
    public float GroundedOffset = -0.14f;
    public float GroundedRadius = 0.28f;
    public bool IsJumping = false;
    public bool IsRunning = false;
    public int JumpGravityDelay = 200;
    public float MoveSpeed = 6.0f;
    public float HorizontalMovementMultiplier = 6f;

    public float LeftBorderCoor = -2;
    public float RightBorderCoor = 2;

    void Awake()
    {
        _initialRotation = transform.rotation;
        _rightRotation = Quaternion.Euler(transform.rotation.x, _rotationAngle, transform.rotation.z);
        _leftRotation = Quaternion.Euler(transform.rotation.x, -_rotationAngle, transform.rotation.z);
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        TouchControl();
        GroundedCheck();
        HandleRun();
        HandleJump();
        HandleHorizontalMovement();
        HandleGravity();
        Move();
    }

    void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);
    }

    void HandleGravity()
    {
        if (Grounded)
        {
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = 0.0f;
            }
        }
        if (IsJumping)
        {
            _verticalVelocity = 0f;
        }
        else if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }

    void HandleHorizontalMovement()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _horizontalMovement = HorizontalMovement.Left;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            _horizontalMovement = HorizontalMovement.Right;
        }
        if (_horizontalMovement == HorizontalMovement.Left)
        {
            GoLeft();
        }
        else if (_horizontalMovement == HorizontalMovement.Right)
        {
            GoRight();
        }
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopCoroutine(Jump());
            StartCoroutine(Jump());
        }
    }

    void HandleRun()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Run();
        }
    }

    public IEnumerator Jump()
    {
        IsJumping = true;
        _animator.SetTrigger("Jump");
        yield return new WaitForSeconds(JumpGravityDelay / 1000f);
        _animator.ResetTrigger("Jump");
        IsJumping = false;
    }

    public void SuperJump()
    {
        _verticalVelocity = 11f;
        _animator.SetTrigger("SuperJump");
    }

    public IEnumerator SpeedUp()
    {
        _animator.SetBool("IsFastRun", true);
        _animator.SetTrigger("FastRun");
        yield return new WaitForSeconds(FastRunDuration / 1000f);
        _animator.SetBool("IsFastRun", false);
        _animator.SetTrigger("Run");
    }

    public void GoLeft()
    {   // Maybe we can write this function using yield. I dont know will it be better or worse
        if (_controller.transform.position.x >= LeftBorderCoor)
        {
            _controller.Move(Vector3.left * HorizontalMovementMultiplier * Time.deltaTime);
        }
        else
        {
            _horizontalMovement = HorizontalMovement.None;
        }
    }

    public void GoRight()
    {
        if (_controller.transform.position.x <= RightBorderCoor)
        {
            _controller.Move(Vector3.right * HorizontalMovementMultiplier * Time.deltaTime);
        }
        else
        {
            _horizontalMovement = HorizontalMovement.None;
        }
    }

    public void Slap(SlapDirection direction)
    {
        if (direction == SlapDirection.Left && Time.time - _leftSlapRef > 0.5f)
        {
            _leftSlapRef = Time.time;
            _animator.SetTrigger("LeftSlap");
        }
        else if (Time.time - _rightSlapRef > 0.5f)
        {
            _rightSlapRef = Time.time;
            _animator.SetTrigger("RightSlap");
        }
    }

    void Move()
    {
        if (IsRunning)
        {
            _controller.Move(Vector3.forward * ((_animator.GetBool("IsFastRun") ? FastRunMoveSpeed : MoveSpeed) * Time.deltaTime) +
                                new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }
    }

    void Run()
    {
        IsRunning = true;
        _animator.SetTrigger("Run");
    }

    void TouchControl()
    {
        if (Input.touchCount > 0)
        {
            _touch = Input.GetTouch(0);
            if (_touch.phase == TouchPhase.Began)
            {
                _dragStarted = true;
            }

            if (_dragStarted)
            {
                if (!IsRunning)
                {
                    Run();
                }
                if (_touch.deltaPosition.x > 3)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, _rightRotation, Time.deltaTime * _rotationSpeed);
                    _controller.Move(Vector3.right * _xMoveSpeed * Time.deltaTime);
                }
                if (_touch.deltaPosition.x < -3)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, _leftRotation, Time.deltaTime * _rotationSpeed);
                    _controller.Move(Vector3.left * _xMoveSpeed * Time.deltaTime);
                }

            }

            if (_touch.phase == TouchPhase.Ended || _touch.phase == TouchPhase.Stationary)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, _initialRotation, Time.deltaTime * _rotationSpeed);
            }
        }
    }
}
