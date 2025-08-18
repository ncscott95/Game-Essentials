using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class PlayerControllerFirstPerson : PlayerControllerBase
{
    [Header("Attacking")]
    [SerializeField] private Attack _meleeAttack;
    [SerializeField] private DamageHitbox _meleeHitbox;
    [SerializeField] private Attack _rangedAttack;
    [SerializeField] private float _throwForce;
    [SerializeField] private float _throwAngle = 15f;
    [SerializeField] private Animator _handAnimator;
    private Attack _equippedAttack;
    private GameObject _projectilePrefab = null;

    [Header("Dodging")]
    [SerializeField] private float _dodgeCooldown;
    [SerializeField] private float _dodgeDistance = 3f;
    [SerializeField] private float _dodgeDuration = 3f;
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private AnimationCurve _dodgeCameraCurve;
    private CapsuleCollider _capsuleCollider;
    private bool _isDodging = false;

    [Header("Camera")]
    [SerializeField] private CinemachinePanTilt _cinemachinePanTilt;
    [SerializeField] private CinemachineInputAxisController _lookController;
    private Coroutine _cameraRotationCoroutine;

    public override void Awake()
    {
        base.Awake();
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    void Start()
    {
        _meleeAttack.InitializeAttack(MeleeAttack, _meleeHitbox);
        //_rangedAttack.InitializeAttack(RangedAttack);
        _equippedAttack = _meleeAttack;

        _cinemachinePanTilt.PanAxis.Value = 0f;
        _cinemachinePanTilt.TiltAxis.Value = 0f;
    }

    public override void Update()
    {
        if (GameManager.Instance.IsInState(GameManager.GameState.Walking) || GameManager.Instance.IsInState(GameManager.GameState.Combat))
        {
            // Ground check
            _isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.01f, Vector3.down, 0.3f, _groundMask);

            // Cap speed at max
            Vector3 flatVelocity = new(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            if (flatVelocity.magnitude > Speed)
            {
                Vector3 limitedVelocity = flatVelocity.normalized * Speed;
                _rb.linearVelocity = new(limitedVelocity.x, _rb.linearVelocity.y, limitedVelocity.z);
            }

            // Handle drag, naturally stops player movement
            _rb.linearDamping = _isGrounded ? _groundDrag : 0f;

            // Rotate player towards camera direction
            Vector3 lookDirection = Camera.forward;
            lookDirection.y = 0;
            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRotation, Time.deltaTime * _rotationSpeed));
            }
        }
    }

    public override void FixedUpdate()
    {
        if (_isDodging) return;

        if (GameManager.Instance.IsInState(GameManager.GameState.Walking) || GameManager.Instance.IsInState(GameManager.GameState.Combat))
        {
            // Move player
            if (_moveInput != Vector2.zero)
            {
                Vector3 move = Camera.forward * _moveInput.y + Camera.right * _moveInput.x;
                move.y = 0f;
                _rb.AddForce(move.normalized * Speed, ForceMode.Force);
            }

            // If moving fast enough, change from idle to walking animation
            _handAnimator.SetBool("Walking", _rb.linearVelocity.magnitude >= WALKING_THRESHOLD);
            _handAnimator.SetFloat("WalkSpeed", _rb.linearVelocity.magnitude / MAX_WALK_MAGNITUDE);
        }
    }

    public override void Attack()
    {
        if (GameManager.Instance.IsInState(GameManager.GameState.Combat))
        {
            if (_canAct)
            {
                _equippedAttack.UseAttack();
            }
        }
        else if (GameManager.Instance.IsInState(GameManager.GameState.Dialogue))
        {
            DialogueManager.Instance.TryAdvanceDialogue();
        }
    }

    private void MeleeAttack()
    {
        _canAct = false;
        _handAnimator.SetTrigger("Attack");
        StartCoroutine(MeleeAttackCoroutine());
    }

    private IEnumerator MeleeAttackCoroutine()
    {
        yield return new WaitForSeconds(_meleeAttack.ChargeTime);
        _meleeAttack.SetHitboxActive(true);
        yield return new WaitForSeconds(_meleeAttack.HitActiveTime);
        _meleeAttack.SetHitboxActive(false);
        yield return new WaitForSeconds(_meleeAttack.CooldownTime);
        _canAct = true;
    }

    private void RangedAttack()
    {
        Debug.Log("Throwing");
        _canAct = false;
        _handAnimator.SetTrigger("Throw");

        Quaternion angle = Quaternion.AngleAxis(-_throwAngle, Camera.right);
        GameObject projectile = Instantiate(_projectilePrefab, Camera.position + Camera.forward, angle * Camera.rotation);
        projectile.GetComponent<Projectile>()?.InitializeProjectile(_rangedAttack, _throwForce);
        StartCoroutine(RangedAttackCoroutine());
    }

    private IEnumerator RangedAttackCoroutine()
    {
        Debug.Log("Started cooldown");
        _projectilePrefab = null;
        _equippedAttack = _meleeAttack;
        yield return new WaitForSeconds(_rangedAttack.CooldownTime);
        _canAct = true;
    }

    public override void Dodge()
    {
        if (GameManager.Instance.IsInState(GameManager.GameState.Combat))
        {
            if (_canAct && Speed == _maxSpeed)
            {
                StartCoroutine(DodgeCoroutine());
            }
        }
    }

    private IEnumerator DodgeCoroutine()
    {
        _canAct = false;
        _isInvincible = true;
        _isDodging = true;
        bool movementInterrupted = false;

        // Determine dodge direction
        Vector3 dodgeDirection = transform.forward;
        if (_moveInput.sqrMagnitude > 0.1f)
        {
            dodgeDirection = (transform.forward * _moveInput.y + transform.right * _moveInput.x).normalized;
        }

        Vector3 startPos = _rb.position;
        Vector3 endPos = startPos + dodgeDirection * _dodgeDistance;

        // Disable look controls
        SetCameraControlActive(false);

        float timer = 0f;
        float horizontalCompletionTime = _dodgeDuration * 0.3f; // Horizontal motion completes at 30% of total dodge duration

        while (timer < _dodgeDuration)
        {
            // Calculate progress for camera animation (full duration)
            float cameraProgress = timer / _dodgeDuration;
            float cameraHeight = _dodgeCameraCurve.Evaluate(cameraProgress);
            _cameraTarget.localPosition = new Vector3(0, cameraHeight, 0);

            // Calculate progress for horizontal movement (completes at 30% of total duration)
            float horizontalProgress = Mathf.Min(1f, timer / horizontalCompletionTime); // Clamped to 1
            Vector3 targetPosition = Vector3.Lerp(startPos, endPos, horizontalProgress);

            // Perform CapsuleCast to check for collisions
            Vector3 capsuleBottom = _rb.position + _capsuleCollider.center - Vector3.up * (_capsuleCollider.height / 2f - _capsuleCollider.radius);
            Vector3 capsuleTop = _rb.position + _capsuleCollider.center + Vector3.up * (_capsuleCollider.height / 2f - _capsuleCollider.radius);
            Vector3 moveDirection = (targetPosition - _rb.position).normalized;
            float moveDistance = Vector3.Distance(_rb.position, targetPosition);

            RaycastHit hit;
            if (Physics.CapsuleCast(capsuleBottom, capsuleTop, _capsuleCollider.radius, moveDirection, out hit, moveDistance, _groundMask))
            {
                // If a collision is detected, move to the hit point and set flag
                _rb.MovePosition(_rb.position + moveDirection * (hit.distance - 0.01f)); // Move slightly less than hit distance to avoid sticking
                movementInterrupted = true;
            }
            else if (!movementInterrupted)
            {
                _rb.MovePosition(targetPosition);
            }

            // End invincibility when player stops moving horizontally
            if (timer >= horizontalCompletionTime && _isInvincible)
            {
                _isInvincible = false;
                SetCameraControlActive(true);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Ensure final position is set only if movement was not interrupted
        if (!movementInterrupted)
        {
            _rb.MovePosition(endPos);
        }
        _cameraTarget.localPosition = new Vector3(0, 1.75f, 0); // Reset camera height

        _isInvincible = false;
        SetCameraControlActive(true);
        _isDodging = false;

        yield return new WaitForSeconds(_dodgeCooldown);
        _canAct = true;
    }

    public override void SetCameraControlActive(bool active) { _lookController.enabled = active; }
    public override void SpawnPlayer(Transform point)
    {
        _rb.position = point.position;
        _cinemachinePanTilt.PanAxis.Value = point.eulerAngles.y;
        _cinemachinePanTilt.TiltAxis.Value = 0f;
    }

    public void RotateCameraToDirection(float targetAngle)
    {
        if (_cameraRotationCoroutine != null)
        {
            StopCoroutine(_cameraRotationCoroutine);
        }
        _cameraRotationCoroutine = StartCoroutine(RotateCameraPanCoroutine(targetAngle));
    }

    private IEnumerator RotateCameraPanCoroutine(float targetAngle)
    {
        while (_cinemachinePanTilt.PanAxis.Value != targetAngle)
        {
            yield return null;
            _cinemachinePanTilt.PanAxis.Value = Mathf.MoveTowardsAngle(_cinemachinePanTilt.PanAxis.Value, targetAngle, 1f);
        }
    }
}
