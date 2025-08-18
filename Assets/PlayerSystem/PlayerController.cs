using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using System.Linq;

public class PlayerController : Singleton<PlayerController>, IDamageable
{
    private const float WALKING_THRESHOLD = 1f;
    private const float MAX_WALK_MAGNITUDE = 4f;
    public Transform Camera { get; private set; }

    private InputSystem_Actions _actions;
    private Rigidbody _rb;

    [Header("Health")]
    [SerializeField] private int _maxHealth;
    [SerializeField] private float _regenTimer;
    [SerializeField] private List<CanvasGroup> _screenDamageEffects;
    public int Health { get; private set; }
    private Coroutine _regenCoroutine = null;
    private Coroutine _fadeEffectCoroutine = null;

    [Header("Movement")]
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _groundDrag;
    [SerializeField] private float _rotationSpeed;
    private List<float> _speedModifiers = new();
    public float Speed { get { return _maxSpeed * _speedModifiers.Aggregate(1f, (acc, val) => acc * val); } }
    private Vector2 _moveInput;

    [Header("Ground Check")]
    [SerializeField] private LayerMask _groundMask;
    private bool _isGrounded;

    [Header("Attacking")]
    [SerializeField] private Attack _meleeAttack;
    [SerializeField] private DamageHitbox _meleeHitbox;
    [SerializeField] private Attack _rangedAttack;
    [SerializeField] private float _throwForce;
    [SerializeField] private float _throwAngle = 15f;
    [SerializeField] private Animator _handAnimator;
    private Attack _equippedAttack;
    private GameObject _projectilePrefab = null;
    private bool _canAct = true;

    [Header("Interacting")]
    [SerializeField] private InteractHitbox _interactHitbox;

    [Header("Dodging")]
    [SerializeField] private float _dodgeCooldown;
    [SerializeField] private float _dodgeDistance = 3f;
    [SerializeField] private float _dodgeDuration = 3f;
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private AnimationCurve _dodgeCameraCurve;
    private CapsuleCollider _capsuleCollider;
    private bool _isInvincible = false;
    private bool _isDodging = false;
    private bool _isDead = false;

    [Header("Camera")]
    [SerializeField] private CinemachinePanTilt _cinemachinePanTilt;
    [SerializeField] private CinemachineInputAxisController _lookController;
    private Coroutine _cameraRotationCoroutine;

    public override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        Instance.Camera = UnityEngine.Camera.main.transform;
        _actions = new InputSystem_Actions();
    }

    void Start()
    {
        Health = _maxHealth;
        _meleeAttack.InitializeAttack(MeleeAttack, _meleeHitbox);
        //_rangedAttack.InitializeAttack(RangedAttack);
        _equippedAttack = _meleeAttack;

        foreach (CanvasGroup group in _screenDamageEffects) group.alpha = 0f;

        _cinemachinePanTilt.PanAxis.Value = 0f;
        _cinemachinePanTilt.TiltAxis.Value = 0f;
    }

    void OnEnable()
    {
        _actions.Player.Enable();
        // Subscribe to input events
        _actions.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _actions.Player.Move.canceled += ctx => _moveInput = Vector2.zero;
        _actions.Player.Interact.performed += ctx => Interact();
        _actions.Player.Attack.performed += ctx => Attack();
        _actions.Player.Dodge.performed += ctx => Dodge();
    }

    void OnDisable()
    {
        _actions.Player.Disable();
        // Unsubscribe from input events
        _actions.Player.Move.performed -= ctx => _moveInput = ctx.ReadValue<Vector2>();
        _actions.Player.Move.canceled -= ctx => _moveInput = Vector2.zero;
        _actions.Player.Interact.performed -= ctx => Interact();
        _actions.Player.Attack.performed -= ctx => Attack();
        _actions.Player.Dodge.performed -= ctx => Dodge();
    }

    void Update()
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

    void FixedUpdate()
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

    private void Interact()
    {
        if (GameManager.Instance.IsInState(GameManager.GameState.Dialogue))
        {
            DialogueManager.Instance.TryAdvanceDialogue();
        }
        else if (GameManager.Instance.IsInState(GameManager.GameState.Walking))
        {
            _interactHitbox.TryInteract();
        }
        else if (GameManager.Instance.IsInState(GameManager.GameState.Combat))
        {
            if (_projectilePrefab == null) _interactHitbox.TryInteract();
        }
    }

    private void Attack()
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

    public void PickupProjectile(GameObject prefab)
    {
        Debug.Log($"Picked up {prefab.name}");
        _equippedAttack = _rangedAttack;
        _projectilePrefab = prefab;
    }

    private void RangedAttack()
    {
        Debug.Log("Throwing");
        _canAct = false;
        // TODO: add throwing animation
        // _handAnimator.SetTrigger("Throw");
        _handAnimator.SetTrigger("Attack");

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

    private void Dodge()
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

    public void TakeDamage(int damage)
    {
        if (_isInvincible) return;

        Health -= damage;
        if (Health <= 0)
        {
            _isDead = true;
            StopAllCoroutines();
            int index = _maxHealth - Health - 1;
            if (index >= 0 && index < _screenDamageEffects.Count)
            {
                _screenDamageEffects[index].alpha = 1f;
            }
            else
            {
                Debug.LogWarning($"Invalid damage effect index: {index}");
            }
            SetCameraControlActive(false);
            GameManager.Instance.OnDeath();
            Debug.Log("Health = " + Health);
        }
        else
        {
            Debug.Log("Health = " + Health);
            // Interrupt current regen timer if it exists, then start a new timer
            if (_regenCoroutine != null) StopCoroutine(_regenCoroutine);

            if (damage > 0)
            {
                // Took damage
                if (_fadeEffectCoroutine != null) StopCoroutine(_fadeEffectCoroutine);
                _screenDamageEffects[_maxHealth - Health - 1].alpha = 1f;
            }
            else if (damage < 0)
            {
                // Regenerated health
                _fadeEffectCoroutine = StartCoroutine(FadeOutDamageEffect(_maxHealth - Health));
            }

            if (Health < _maxHealth) _regenCoroutine = StartCoroutine(RegenerateHealth());
        }
    }

    private IEnumerator RegenerateHealth()
    {
        yield return new WaitForSeconds(_regenTimer);
        if (Health < _maxHealth) TakeDamage(-1); // TODO: cursed?
    }
    
    private IEnumerator FadeOutDamageEffect(int index)
    {
        float duration = 1f;
        float timer = duration;
        while (timer > 0f)
        {
            yield return null;
            timer -= Time.deltaTime;
            _screenDamageEffects[index].alpha = timer / duration;
        }
    }

    public void AddSpeedModifier(float modifier) { _speedModifiers.Add(modifier); }
    public void RemoveSpeedModifier(float modifier) { _speedModifiers.Remove(modifier); }
    public void SetCameraControlActive(bool active) { _lookController.enabled = active; }
    public void SpawnPlayer(Transform point)
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
