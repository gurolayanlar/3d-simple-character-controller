using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerControler : MonoBehaviour
{
    #region Movement
    
    private Vector2 _input;
    private CharacterController _characterController;
    private Vector3 _direction;

    [SerializeField] private float speed;
    #endregion

    #region Rotation

    private float _currentVelocity;

    [SerializeField] private float smoothTime = 0.05f;

    #endregion



    #region Gravity

    private float _gravity = -9.81f;
    private float _velocity;

    [SerializeField] private float gravityMultiplier = 3.0f;
    #endregion

    #region Jumping

    [SerializeField] private float jumpPower;
    [SerializeField] private int maxNumberOfJumps = 2;
    private int _numberOfJumps;

    #endregion


    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        Gravity();
        Rotation();
        Movement();
    }

    void Rotation()
    {
        if (_input.sqrMagnitude == 0) return;

        var targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _currentVelocity, smoothTime);

        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    void Movement()
    {
        _characterController.Move(_direction * speed * Time.deltaTime);
    }

    void Gravity()
    {
        if (IsGrounded() && _velocity < 0f)
        {
            _velocity = -1f;
        }
        else
        {
            _velocity += _gravity * gravityMultiplier * Time.deltaTime;
        }
        
        _direction.y = _velocity;
    }

    bool IsGrounded() => _characterController.isGrounded;
    public void Move(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();

        _direction = new Vector3(_input.x, 0f, _input.y);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!IsGrounded() && _numberOfJumps >= maxNumberOfJumps) return;
        if (_numberOfJumps == 0) StartCoroutine(WaitForLanding());

        _numberOfJumps++;

        _velocity = jumpPower;
        //_velocity = jumpPower / _numberOfJumps;
    }

    IEnumerator WaitForLanding()
    {
        yield return new WaitUntil(() => !IsGrounded());
        yield return new WaitUntil(IsGrounded);

        _numberOfJumps = 0;
    }

}
