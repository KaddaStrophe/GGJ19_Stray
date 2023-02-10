using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public Transform groundCheck;
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
    float _jumpVelocity = 5f;
    [SerializeField]
    float _maxSpeed = 20f;
    [SerializeField]
    float _movementSpeed = 7f;
    [SerializeField]
    float _airVelocityMultiplier = 0.5f;
    [SerializeField]
    float _jumpBuffer = 5f;
    [SerializeField]
    float _pushBuffer = 50f;
    [SerializeField]
    float _pushDistance = 0.5f;
    [SerializeField]
    float _pushSpeed = 0.5f;
    [SerializeField]
    float _outOfHomeMultiplier = 1f;
    [SerializeField]
    float _debuffTimeInterval = 150f;
    [SerializeField]
    float _respawnLerpSpeed = 5f;

    float movement;
    Rigidbody2D _playerRigidbody;
    Animator playerAnimator;
    bool isGrounded;
    Vector3 _firePoint;
    AnimatorStateInfo _currentState;
    int _currentFrame;
    int _currentBufferedFrame;
    bool _jumpBufferActive;
    HomeController homeObject;
    bool _lookingLeft;
    bool pushModus;
    Vector3 _pushVectorPlayer;
    Vector3 _pushVectorBlock;
    bool _pushNow;
    bool _isWallInSight;
    RaycastHit2D _hit;
    RaycastHit2D _playerHit;
    bool carryFlag;
    float _outOfHomeFrame;
    int _currentOutOfHomeState = 0;
    float _originalMaxSpeed;
    bool _respawnMode;

    static int _animatorIdleState = Animator.StringToHash("Base Layer.PlayerIdle");
    static int _animatorJumpState = Animator.StringToHash("Baser Layer.PlayerJump");
    static int _animatorPickupState = Animator.StringToHash("Baser Layer.PlayerPickup");
    static int _animatorDropState = Animator.StringToHash("Baser Layer.PlayerDrop");

    LayerMask layerMask;


    protected void Awake() {
        _playerRigidbody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        layerMask = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Block"));
        homeObject = FindObjectOfType<HomeController>();

        _originalMaxSpeed = _maxSpeed;
        currentCheckpoint = transform;

        currentPushBlock = GameObject.FindGameObjectWithTag("PushBlock");
        movement = 0;
    }

    protected void FixedUpdate() {
        PerformMovement(movement);
    }

    public void ChangeMovementVector(float movement) {
        Debug.Log(movement);
        this.movement = movement;
    }
    void PerformMovement(float movement) {
        // Ground check
        isGrounded = Physics2D.Linecast(transform.position, groundCheck.position, layerMask);

        playerAnimator.SetBool("IsGrounded", isGrounded);

        _currentState = playerAnimator.GetCurrentAnimatorStateInfo(0);

        CalculateAirVelocity(movement);

        if (!pushModus && canMove) {
            MoveThePlayer(movement);
        }
        if (!leftTheCircle) {
            _maxSpeed = _originalMaxSpeed;
        }

        if (leftTheCircle) {
            _outOfHomeFrame = outOfCircleStartFrame;

            // tired state
            if (_outOfHomeFrame + (_debuffTimeInterval * 0) < Time.frameCount && _currentOutOfHomeState == 0) {
                _maxSpeed *= _outOfHomeMultiplier;
                SetOutOfHomeState(1);
                if (!pushModus) {
                    PlayCurrentAnimationSection();
                }
            }

            //sad state
            else if (_outOfHomeFrame + (_debuffTimeInterval * 1) < Time.frameCount && _currentOutOfHomeState == 1) {
                _maxSpeed *= _outOfHomeMultiplier;
                SetOutOfHomeState(2);
                if (!pushModus) {
                    PlayCurrentAnimationSection();
                }
            }

            // dead state
            else if (_outOfHomeFrame + (_debuffTimeInterval * 2) < Time.frameCount && _currentOutOfHomeState == 2) {
                StartCoroutine(FadeToCheckpoint());
                SetOutOfHomeState(3);
            }
        }

        if (pushModus) {
            _currentFrame = Time.frameCount;

            if (movement > 0
                && (Time.frameCount == _currentFrame + _pushBuffer
                || Time.frameCount == _currentFrame + _pushBuffer - 1
                || Time.frameCount == _currentFrame + _pushBuffer + 1)
                && _pushNow == false) {
                _pushVectorPlayer = new Vector3(transform.position.x + _pushDistance, transform.position.y, transform.position.z);
                _pushVectorBlock = new Vector3(currentPushBlock.transform.position.x + _pushDistance, currentPushBlock.transform.position.y, currentPushBlock.transform.position.z);
                _pushNow = true;
                _currentFrame = Time.frameCount;

                if ((!_lookingLeft && movement > 0)
                    || (_lookingLeft && movement < 0)) {
                    playerAnimator.SetBool("Push", true);
                } else if ((_lookingLeft && movement > 0)
                    || (!_lookingLeft && movement < 0)) {
                    playerAnimator.SetBool("Pull", true);
                }

                _hit = Physics2D.Raycast(currentPushBlock.transform.position, Vector3.right, currentPushBlock.GetComponent<SpriteRenderer>().bounds.size.x / 2 + 0.1f, 1 << LayerMask.NameToLayer("Ground"));
                _playerHit = Physics2D.Raycast(transform.position, Vector3.right, GetComponent<SpriteRenderer>().bounds.size.x / 2 + 0.1f, 1 << LayerMask.NameToLayer("Ground"));
            }
        }
        if (_pushNow) {
            if (_hit.collider == null && _playerHit.collider == null) {
                currentPushBlock.transform.position = Vector3.MoveTowards(currentPushBlock.transform.position, _pushVectorBlock, _pushSpeed * Time.deltaTime);
                transform.position = Vector3.MoveTowards(transform.position, _pushVectorPlayer, _pushSpeed * Time.deltaTime);
            } else {
                _pushNow = false;
                playerAnimator.SetBool("Push", false);
                playerAnimator.SetBool("Pull", false);
            }
        }
        if (currentPushBlock != null && currentPushBlock.transform.position == _pushVectorBlock) {
            _pushNow = false;
            playerAnimator.SetBool("Push", false);
            playerAnimator.SetBool("Pull", false);
        }

        if (_respawnMode) {
            transform.position = Vector3.MoveTowards(transform.position, currentCheckpoint.position, _respawnLerpSpeed * Time.deltaTime);
            _playerRigidbody.isKinematic = true;
            if (transform.position == currentCheckpoint.position) {
                StartCoroutine(MakeMovableAgain());
                StartCoroutine(Utility.LerpColorRoutine(GetComponent<SpriteRenderer>(), new Color(1, 1, 1, 1), 2f, false));
                DeathVFX.instance.ActivateMothmanEffects();
            }
        }

        playerAnimator.SetFloat("AirVelocity", _playerRigidbody.velocity.y);
    }

    // Handle picking up things
    public void PerformInteract() {
        isGrounded = Physics2D.Linecast(transform.position, groundCheck.position, layerMask);
        playerAnimator.SetBool("IsGrounded", isGrounded);
        if (isGrounded) {
            if (homeObject != null) {
                Debug.Log("Is carrying home: " + isCarryingHome + ", is standing near block: " + isStandingNearBlock + ", carry flag: " + carryFlag);
                if (isCarryingHome && !isStandingNearBlock && carryFlag) {
                    Debug.Log("Should drop now");
                    carryFlag = false;
                    canMove = false;
                    playerAnimator.SetTrigger("Drop");
                    homeObject.DeactivateCarryMode();
                } else if (!isCarryingHome && pickupInRange && !carryFlag) {
                    carryFlag = true;
                    canMove = false;
                    playerAnimator.SetTrigger("Pickup");
                    homeObject.ActivateCarryMode();
                }
            }

            if (isStandingNearBlock && !isCarryingHome && !pushModus) {
                ActivatePushModus();
            } else if (pushModus && !_pushNow) {
                DeactivatePushModus();
            }
        }
    }

    public void PerformJump() {
        isGrounded = Physics2D.Linecast(transform.position, groundCheck.position, layerMask);
        if (!isCarryingHome && !pushModus) {
            if (isGrounded) {
                ExecuteJump();
            }

            if (!isGrounded) {
                ActivateJumpBuffer();
            }
        }

        if (_jumpBufferActive) {
            TryBufferedJump();
        }
    }

    public void SetNormalModeTrigger() {
        PlayCurrentAnimationSection();
    }

    void PlayCurrentAnimationSection() {
        if (_currentOutOfHomeState == 0) {
            playerAnimator.SetTrigger("NormalState");
        }

        if (_currentOutOfHomeState == 1) {
            playerAnimator.SetTrigger("TiredState");
        }

        if (_currentOutOfHomeState == 2) {
            playerAnimator.SetTrigger("SadState");
        }
    }

    public void SetOutOfHomeState(int state) {
        // 0 = Normal, 1 = Tired, 2 = Sad, 3 = Dead
        _currentOutOfHomeState = state;
    }

    void MoveThePlayer(float movement) {
        // This will make the player move, as long as the maxSpeed isn't met
        if (movement * _playerRigidbody.velocity.x < _maxSpeed) {
            _playerRigidbody.AddForce(movement * _movementSpeed * Vector2.right);
        }

        // If player reaches maxSpeed, cap the velocity
        if (Mathf.Abs(_playerRigidbody.velocity.x) > _maxSpeed) {
            _playerRigidbody.velocity = new Vector2(Mathf.Sign(_playerRigidbody.velocity.x) * _maxSpeed, _playerRigidbody.velocity.y);
        }

        // Flip the player when changing directions
        if (movement < 0) {
            _lookingLeft = true;
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        } else if (movement > 0) {
            _lookingLeft = false;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        // Send the current movement speed to the animator
        playerAnimator.SetFloat("Movementspeed", Mathf.Abs(_playerRigidbody.velocity.x));
    }

    // Activates when you press E near a boulder
    void ActivatePushModus() {
        pushModus = true;
        playerAnimator.SetTrigger("PushModus");
    }

    void DeactivatePushModus() {
        pushModus = false;
        PlayCurrentAnimationSection();
    }

    // Calculate the Jump Force and add it to the rigidbody
    void ExecuteJump() {
        _playerRigidbody.velocity = new Vector2(_playerRigidbody.velocity.x, 0);
        _playerRigidbody.AddForce(new Vector2(0f, _jumpVelocity));
    }

    // set the buffer active, if the player tries to jump while airbourne
    void ActivateJumpBuffer() {
        _currentFrame = Time.frameCount;
        _currentBufferedFrame = _currentFrame;
        _jumpBufferActive = true;
    }

    void TryBufferedJump() {
        if (isGrounded) {
            ExecuteJump();
        }

        _currentFrame++;
        if (_currentFrame > _currentBufferedFrame + _jumpBuffer) {
            _jumpBufferActive = false;
        }
    }

    // This method will damp the air movement by _airVelocityMultiplier (0.5f as standard value)
    void CalculateAirVelocity(float movement) {
        if (_currentState.fullPathHash == _animatorJumpState) {
            movement *= _airVelocityMultiplier;
        }
    }

    // Gets the coordinates of a mouseclick
    Vector3 GetMouseClickPosition() {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return new Vector3(ray.origin.x, ray.origin.y, 0);
    }

    public IEnumerator FadeToCheckpoint() {
        DeathVFX.instance.Death();
        DeactivatePushModus();
        canMove = false;
        yield return new WaitForSeconds(2f);
        //this.transform.position = new Vector3(currentCheckpoint.position.x, currentCheckpoint.position.y - 0.8f, currentCheckpoint.position.z);
        //this.transform.position = Vector3.Lerp(this.transform.position, currentCheckpoint.position, 2.5f * Time.deltaTime);
        //StartCoroutine(GetReadyGo());
        _respawnMode = true;
    }

    public IEnumerator MakeMovableAgain() {
        yield return new WaitForSeconds(1f);
        canMove = true;
        _playerRigidbody.isKinematic = false;
        _respawnMode = false;
    }
}
