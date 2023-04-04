using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public Transform groundCheck;
    public static PlayerController instance;
    public Transform carryPosition;

    [HideInInspector]
    public bool pickupInRange;
    [HideInInspector]
    public bool isCarryingHome;
    [HideInInspector]
    public bool isStandingNearBlock;
    [HideInInspector]
    public GameObject currentPushBlock;
    [HideInInspector]
    public bool canMove = true;
    [HideInInspector]
    public bool leftTheCircle;
    [HideInInspector]
    public float outOfCircleStartFrame;
    [HideInInspector]
    public Transform currentCheckpoint;

    [SerializeField]
    private float _jumpVelocity = 5f;
    [SerializeField]
    private float _maxSpeed = 20f;
    [SerializeField]
    private float _movementSpeed = 7f;
    [SerializeField]
    private float _airVelocityMultiplier = 0.5f;
    [SerializeField]
    private float _jumpBuffer = 5f;
    [SerializeField]
    private float _pushBuffer = 50f;
    [SerializeField]
    private float _pushDistance = 0.5f;
    [SerializeField]
    private float _pushSpeed = 0.5f;
    [SerializeField]
    private float _outOfHomeMultiplier = 1f;
    [SerializeField]
    private float _debuffTimeInterval = 150f;
    [SerializeField]
    private float _respawnLerpSpeed = 5f;


    private Rigidbody2D _playerRigidbody;
    private Animator _playerAnimator;
    private bool _isGrounded;
    private float _horizontalValue;
    private Vector3 _firePoint;
    private AnimatorStateInfo _currentState;
    private int _currentFrame;
    private int _currentBufferedFrame;
    private bool _jumpBufferActive;
    private HomeController _homeObject;
    private bool _lookingLeft;
    private bool _pushModus;
    private Vector3 _pushVectorPlayer;
    private Vector3 _pushVectorBlock;
    private bool _pushNow;
    private bool _isWallInSight;
    private RaycastHit2D _hit;
    private RaycastHit2D _playerHit;
    private bool _carryFlag;
    private float _outOfHomeFrame;
    private int _currentOutOfHomeState = 0;
    private float _originalMaxSpeed;
    private bool _respawnMode;

    static int _animatorIdleState = Animator.StringToHash("Base Layer.PlayerIdle");
    static int _animatorJumpState = Animator.StringToHash("Baser Layer.PlayerJump");
    static int _animatorPickupState = Animator.StringToHash("Baser Layer.PlayerPickup");
    static int _animatorDropState = Animator.StringToHash("Baser Layer.PlayerDrop");

    private LayerMask _layerMask;


    void Awake()
    {
        _playerRigidbody = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();
        _layerMask = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Block"));
        GameObject homeObjectTemp = GameObject.FindGameObjectWithTag("Home");
        if (homeObjectTemp != null) _homeObject = homeObjectTemp.GetComponent<HomeController>();
        _originalMaxSpeed = _maxSpeed;
        currentCheckpoint = this.transform;

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        currentPushBlock = GameObject.FindGameObjectWithTag("PushBlock");
    }

    /* This method is handling all the button inputs a player makes
    *  Input 1 (bool jump): is true, when the player pressed space
    *  Input 2 (bool shoot): is true, when the player clicked
    *  Input 3 (bool interact): is true, when the player presses right strg
    */
    public void HandleButtonInputs(bool jump, bool shoot, bool interact)
    {
        // Ground check
        _isGrounded = Physics2D.Linecast(transform.position, groundCheck.position, _layerMask);

        _playerAnimator.SetBool("IsGrounded", _isGrounded);

        _currentState = _playerAnimator.GetCurrentAnimatorStateInfo(0);

        _horizontalValue = Input.GetAxisRaw("Horizontal");

        CalculateAirVelocity();

        if (!_pushModus && canMove) MoveThePlayer();

        // Jump handling
        if (jump && !isCarryingHome && !_pushModus)
        {
            if (_isGrounded) ExecuteJump();
            if (!_isGrounded) ActivateJumpBuffer();
        }

        if (_jumpBufferActive) TryBufferedJump();
        // /Jump handling


        // Handle picking up things
        if (Input.GetKeyDown(KeyCode.E) && _isGrounded)
        {
            if (_homeObject != null)
            {
                if (isCarryingHome && !isStandingNearBlock && _carryFlag)
                {
                    _carryFlag = false;
                    canMove = false;
                    _playerAnimator.SetTrigger("Drop");
                    _homeObject.deactivateCarryMode();
                }
                else if (!isCarryingHome && pickupInRange && !_carryFlag)
                {
                    _carryFlag = true;
                    canMove = false;
                    _playerAnimator.SetTrigger("Pickup");
                    _homeObject.activateCarryMode();
                }
            }

            if (isStandingNearBlock && !isCarryingHome && !_pushModus)
            {
                ActivatePushModus();
            }
            else if (_pushModus && !_pushNow)
            {
                DeactivatePushModus();
            }    
        }

        if (!leftTheCircle)
        {
            _maxSpeed = _originalMaxSpeed;
        }

        if (leftTheCircle)
        {
            _outOfHomeFrame = outOfCircleStartFrame;
            
            // tired state
            if (_outOfHomeFrame + _debuffTimeInterval * 0 < Time.frameCount && _currentOutOfHomeState == 0)
            {
                _maxSpeed = _maxSpeed * _outOfHomeMultiplier;
                SetOutOfHomeState(1);
                if (!_pushModus) PlayCurrentAnimationSection();
            }

            //sad state
            else if (_outOfHomeFrame + _debuffTimeInterval * 1 < Time.frameCount && _currentOutOfHomeState == 1)
            {
                _maxSpeed = _maxSpeed * _outOfHomeMultiplier;
                SetOutOfHomeState(2);
                if (!_pushModus) PlayCurrentAnimationSection();
            }

            // dead state
            else if (_outOfHomeFrame + _debuffTimeInterval * 2 < Time.frameCount && _currentOutOfHomeState == 2)
            {
                StartCoroutine(FadeToCheckpoint());
                SetOutOfHomeState(3);
            }
        }
        
        if (_pushModus)
        {
            if (Input.GetKeyDown(KeyCode.D)) _currentFrame = Time.frameCount;
            if (_horizontalValue > 0 && (Time.frameCount == _currentFrame + _pushBuffer || Time.frameCount == _currentFrame + _pushBuffer - 1 || Time.frameCount == _currentFrame + _pushBuffer + 1) && _pushNow == false)
            {
                _pushVectorPlayer = new Vector3(this.transform.position.x + _pushDistance, this.transform.position.y, this.transform.position.z);
                _pushVectorBlock = new Vector3(currentPushBlock.transform.position.x + _pushDistance, currentPushBlock.transform.position.y, currentPushBlock.transform.position.z);
                _pushNow = true;
                _currentFrame = Time.frameCount;
                if (!_lookingLeft) _playerAnimator.SetBool("Push", true);
                else _playerAnimator.SetBool("Pull", true);
                _hit = Physics2D.Raycast(currentPushBlock.transform.position, Vector3.right, currentPushBlock.GetComponent<SpriteRenderer>().bounds.size.x / 2 + 0.1f, 1 << LayerMask.NameToLayer("Ground"));
                _playerHit = Physics2D.Raycast(this.transform.position, Vector3.right, this.GetComponent<SpriteRenderer>().bounds.size.x / 2 + 0.1f, 1 << LayerMask.NameToLayer("Ground"));
            }
            if (Input.GetKeyDown(KeyCode.A)) _currentFrame = Time.frameCount;
            else if (_horizontalValue < 0 && (Time.frameCount == _currentFrame + _pushBuffer || Time.frameCount == _currentFrame + _pushBuffer - 1 || Time.frameCount == _currentFrame + _pushBuffer + 1) && _pushNow == false)
            {
                _pushVectorPlayer = new Vector3(this.transform.position.x - _pushDistance, this.transform.position.y, this.transform.position.z);
                _pushVectorBlock = new Vector3(currentPushBlock.transform.position.x - _pushDistance, currentPushBlock.transform.position.y, currentPushBlock.transform.position.z);
                _pushNow = true;
                _currentFrame = Time.frameCount;
                if (_lookingLeft) _playerAnimator.SetBool("Push", true);
                else _playerAnimator.SetBool("Pull", true);
                _hit = Physics2D.Raycast(currentPushBlock.transform.position, Vector3.left, currentPushBlock.GetComponent<SpriteRenderer>().bounds.size.x / 2 + 0.1f, 1 << LayerMask.NameToLayer("Ground"));
                _playerHit = Physics2D.Raycast(this.transform.position, Vector3.left, this.GetComponent<SpriteRenderer>().bounds.size.x / 2 + 0.1f, 1 << LayerMask.NameToLayer("Ground"));
            }
        }
        if (_pushNow)
        {
            if (_hit.collider == null && _playerHit.collider == null)
            {
                currentPushBlock.transform.position = Vector3.MoveTowards(currentPushBlock.transform.position, _pushVectorBlock, _pushSpeed * Time.deltaTime);
                this.transform.position = Vector3.MoveTowards(this.transform.position, _pushVectorPlayer, _pushSpeed * Time.deltaTime);
            }
            else
            {
                _pushNow = false;
                _playerAnimator.SetBool("Push", false);
                _playerAnimator.SetBool("Pull", false);
            }
        }
        if (currentPushBlock != null && currentPushBlock.transform.position == _pushVectorBlock)
        {
            _pushNow = false;
            _playerAnimator.SetBool("Push", false);
            _playerAnimator.SetBool("Pull", false);
        }

        // /Handle picking up things

        if (_respawnMode)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, currentCheckpoint.position, _respawnLerpSpeed * Time.deltaTime);
            _playerRigidbody.isKinematic = true;
            if (this.transform.position == currentCheckpoint.position)
            {
                StartCoroutine(MakeMovableAgain());
                StartCoroutine(Utility.LerpColorRoutine(GetComponent<SpriteRenderer>(), new Color(1, 1, 1, 1), 2f, false));
                DeathVFX.instance.ActivateMothmanEffects();
            }
        }

        _playerAnimator.SetFloat("AirVelocity", _playerRigidbody.velocity.y);
    }

    public void SetNormalModeTrigger()
    {
        PlayCurrentAnimationSection();
    }

    private void PlayCurrentAnimationSection()
    {
        if (_currentOutOfHomeState == 0)
            _playerAnimator.SetTrigger("NormalState");
        if (_currentOutOfHomeState == 1)
            _playerAnimator.SetTrigger("TiredState");
        if (_currentOutOfHomeState == 2)
            _playerAnimator.SetTrigger("SadState");

    }

    public void SetOutOfHomeState(int state)
    {
        // 0 = Normal, 1 = Tired, 2 = Sad, 3 = Dead
        _currentOutOfHomeState = state;
    }

    private void MoveThePlayer()
    {
        // This will make the player move, as long as the maxSpeed isn't met
        if (_horizontalValue * _playerRigidbody.velocity.x < _maxSpeed)
        {
            _playerRigidbody.AddForce(Vector2.right * _horizontalValue * _movementSpeed);
        }

        // If player reaches maxSpeed, cap the velocity
        if (Mathf.Abs(_playerRigidbody.velocity.x) > _maxSpeed)
            _playerRigidbody.velocity = new Vector2(Mathf.Sign(_playerRigidbody.velocity.x) * _maxSpeed, _playerRigidbody.velocity.y);

        // Flip the player when changing directions
        if (_horizontalValue < 0)
        {
            _lookingLeft = true;
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else if (_horizontalValue > 0)
        {
            _lookingLeft = false;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        // Send the current movement speed to the animator
        _playerAnimator.SetFloat("Movementspeed", Mathf.Abs(_playerRigidbody.velocity.x));
    }

    // Activates when you press E near a boulder
    private void ActivatePushModus()
    {
        _pushModus = true;
        _playerAnimator.SetTrigger("PushModus");
    }

    private void DeactivatePushModus()
    {
        _pushModus = false;
        PlayCurrentAnimationSection();
    }

    // Calculate the Jump Force and add it to the rigidbody
    private void ExecuteJump()
    {
        _playerRigidbody.velocity = new Vector2(_playerRigidbody.velocity.x, 0);
        _playerRigidbody.AddForce(new Vector2(0f, _jumpVelocity));
    }

    // set the buffer active, if the player tries to jump while airbourne
    private void ActivateJumpBuffer()
    {
        _currentFrame = Time.frameCount;
        _currentBufferedFrame = _currentFrame;
        _jumpBufferActive = true;
    }

    private void TryBufferedJump()
    {
        if (_isGrounded) ExecuteJump();
        _currentFrame++;
        if (_currentFrame > _currentBufferedFrame + _jumpBuffer) _jumpBufferActive = false;
    }

    // This method will damp the air movement by _airVelocityMultiplier (0.5f as standard value)
    private void CalculateAirVelocity()
    {
        if (_currentState.fullPathHash == _animatorJumpState)
        {
            _horizontalValue *= _airVelocityMultiplier;
        }
    }

    // Gets the coordinates of a mouseclick
    private Vector3 GetMouseClickPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return new Vector3(ray.origin.x, ray.origin.y, 0);
    }

    public IEnumerator FadeToCheckpoint()
    {
        DeathVFX.instance.Death();
        DeactivatePushModus();
        canMove = false;
        yield return new WaitForSeconds(2f);
        //this.transform.position = new Vector3(currentCheckpoint.position.x, currentCheckpoint.position.y - 0.8f, currentCheckpoint.position.z);
        //this.transform.position = Vector3.Lerp(this.transform.position, currentCheckpoint.position, 2.5f * Time.deltaTime);
        //StartCoroutine(GetReadyGo());
        _respawnMode = true;
    }

    public IEnumerator MakeMovableAgain()
    {
        yield return new WaitForSeconds(1f);
        canMove = true;
        _playerRigidbody.isKinematic = false;
        _respawnMode = false;
    }
}
