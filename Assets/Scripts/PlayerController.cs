using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [Serializable]
    public struct PhysicsVariables
    {
        public float walkSpeed;
        public float delayBeforeNormalGravity;
        public float jumpImpulse;
        public float normalGravity;
        public float jumpingGravity;
        public float maxFallingSpeed;
        public float wallJumpHorizontalEjectionSpeed;
        public float wallJumpMinimalTime;
        public float rollingSpeed;
        public float rollChargingTime;
        public float rollTimeBeforeCanJump;

        public PhysicsVariables(float walkSpeed, float delayBeforeNormalGravity, float jumpImpulse, float normalGravity, float jumpingGravity, float maxFallingSpeed, float wallJumpHorizontalEjectionSpeed, float wallJumpMinimalTime, float rollingSpeed, float rollChargingTime, float rollTimeBeforeCanJump)
        {
            this.walkSpeed = walkSpeed;
            this.delayBeforeNormalGravity = delayBeforeNormalGravity;
            this.jumpImpulse = jumpImpulse;
            this.normalGravity = normalGravity;
            this.jumpingGravity = jumpingGravity;
            this.maxFallingSpeed = maxFallingSpeed;
            this.wallJumpHorizontalEjectionSpeed = wallJumpHorizontalEjectionSpeed;
            this.wallJumpMinimalTime = wallJumpMinimalTime;
            this.rollingSpeed = rollingSpeed;
            this.rollChargingTime = rollChargingTime;
            this.rollTimeBeforeCanJump = rollTimeBeforeCanJump;
        }
    }

    [SerializeField] PhysicsVariables currentPhysic;
    [SerializeField] PhysicsVariables normalPhysic;
    [SerializeField] PhysicsVariables underWaterPhysic;

    [SerializeField] private LayerMask groundMask;
    [SerializeField] private AnimationClip deathAnimationClip;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private WallCheck wallCheck;
    [SerializeField] private WallClipChecker wallClipCheck;

    [SerializeField] private float transitionShiftDistance;

    private float lastTimeSinceJumped = 0;
    private float lastTimeSinceWallJump = 0f;
    private bool wantsToJump = false;

    private float directionFactor = 0f;
    private bool movingLeft = false;

    private float timeSinceGrounded = 0f;
    private float coyoteJumpWindow = 0.1f;
    public bool[] closeToWalls = new bool[2] { false, false };

    private Torche torche;

    private bool canMove = true;

    [Header("Upgrades")]
    [SerializeField] private bool doubleJumpUnlocked = false;
    [SerializeField] private bool wallJumpUnlocked = false;
    [SerializeField] private bool spinDashUnlocked = false;

    private bool firstJumpPerformed = false;
    private bool doubleJumpPerformed = false;
    private bool isWallJumping = false; // Vrai tant que le joueur maintient le saut après un walljump. Force d'avancer dans la direction opposée au mur (empêche infinite single wall jumps)
    private bool releasedJump = true;   // Pour forcer le relachement du saut pour des sauts complémentaires
    private bool isRolling = false;
    private float currentRollingTimeAgainstWall = 0;

    private Vector3 originalScale;
    [SerializeField] Vector3 rollingScale;
    [SerializeField] float maxRollingTimeAgainstWall;
    [SerializeField] float bonkForce;

    private SpriteRenderer spriteRenderer;

    private InputAction cheat;

    void Awake()
    {
        Application.targetFrameRate = 60;
        this.currentPhysic = this.normalPhysic;

        spriteRenderer = GetComponent<SpriteRenderer>();
        cheat = InputSystem.actions.FindAction("Cheat");
    }

    // Start is called before the first frame update
    void Start()
    {
        this.originalScale = this.transform.localScale;
        //GetComponent<PlayerInput>().currentActionMap
    }

    // Update is called once per frame
    void Update()
    {
        if (cheat.WasPerformedThisFrame())
        {
            //Debug.Log("C'est celle là");
            //this.doubleJumpUnlocked = !this.doubleJumpUnlocked;
            //Debug.Log(doubleJumpUnlocked);
        }

        if (this.canMove)
        {
            this.CheckMovement();
        }
        this.ProcessDurationEvents();
    }

    private void FixedUpdate()
    {
        if (this.canMove)
        this.MovePatate();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed && !this.isRolling && this.canMove)   // En roulant, on a pas libre contrôle
        {
            float val = context.ReadValue<Vector2>().x;

            if(Math.Abs(val) > 0.01)
            {
                directionFactor = context.ReadValue<Vector2>().x;
                movingLeft = directionFactor < 0;
            }
            else
            {
                directionFactor = 0f;
            }
            
        }

        if (context.canceled)
        {
            directionFactor = 0f;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            wantsToJump = true;
        }

        if (context.canceled)
        {
            this.wantsToJump = false;
            this.releasedJump = true;
        }
    }

    public void OnSpinDash(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (!this.isRolling)
            {
                if (this.spinDashUnlocked && this.currentPhysic.rollingSpeed > 0)
                {
                    StartCoroutine(ChargeSpinDash());
                }
            }
            else
            {
                this.Bonk();
            }
        }
    }


    private void CheckMovement()
    {
        if (_rigidbody.gravityScale == this.currentPhysic.jumpingGravity)
        {
            lastTimeSinceJumped += Time.deltaTime;

            // Arrêt de gravité de saut
            if (!wantsToJump || lastTimeSinceJumped > this.currentPhysic.delayBeforeNormalGravity)
            {
                _rigidbody.gravityScale = this.currentPhysic.normalGravity;
                lastTimeSinceJumped = 0f;
                this.wantsToJump = false;
            }
        }
    }

    private void ProcessDurationEvents()
    {
        if (this.isWallJumping)
        {
            this.lastTimeSinceWallJump += Time.deltaTime;
            /*
            if (this.lastTimeSinceWallJump > this.currentPhysic.wallJumpMinimalTime)
            {
                this.isWallJumping = false;
            }*/
        }

        if (this.isRolling)
        {
            if (this.wallCheck.IsThereWall())
            {
                if (this.currentRollingTimeAgainstWall < this.maxRollingTimeAgainstWall)
                {
                    this.currentRollingTimeAgainstWall += Time.deltaTime;
                }
                else
                {
                    this.Bonk();
                }
            }
        }
    }

    /**
     * Déplace notre patate en fonction de l'accélération lue dans CheckMovements()
     */
    private void MovePatate()
    {
        // Replacement par le repousseur de mur
        if (this.closeToWalls[1])
        {
            Debug.Log("REPOUSSE !");
            this.transform.Translate(0.1f * (this.movingLeft ? 1 : -1), 0, 0);
        }


        // Le saut
        if (wantsToJump && this._rigidbody.linearVelocity.y < 0.1f && this.releasedJump) // La base, faut vouloir sauter
        {
            // Walljump en priorité
            if ((!IsGrounded() || this.wallClipCheck.IsInsideWall()) && this.wallJumpUnlocked && this.wallCheck.CanWallJump())  // Si on clip dans un mur, le jeu nous considèrera à terre, alors qu'on l'est pas vraiment
            {
                this.isWallJumping = true;
                //this.movingLeft = !this.movingLeft;
                this.FlipPatate(!this.movingLeft);
                this.Jump();
                this.lastTimeSinceWallJump = 0f;
                this.currentRollingTimeAgainstWall = 0f;
            }

            // Saut classique
            else if (((IsGrounded() || this.timeSinceGrounded < this.coyoteJumpWindow) && !this.firstJumpPerformed) // Touche le sol ou coyote jump, pour saut simple
            || (this.doubleJumpUnlocked && !this.doubleJumpPerformed))    // Ou alors double jump, c'est bien aussi
            {
                if (this.firstJumpPerformed)
                {
                    this.doubleJumpPerformed = true;
                }
                this.firstJumpPerformed = true;

                this.Jump();
            }
        }
        //wantsToJump = false;

        // Les directions

        // Prios : Rolling > Walljump > Normal
        if (!this.closeToWalls[1])
        {
            if (this.isRolling)
            {
                transform.Translate(this.currentPhysic.rollingSpeed * Time.deltaTime * (this.movingLeft ? Vector3.left : Vector3.right));
            }
            else if (this.isWallJumping)
            {
                transform.Translate(this.currentPhysic.wallJumpHorizontalEjectionSpeed * Time.deltaTime * (this.movingLeft ? Vector3.left : Vector3.right));
            }
            else
            {
                transform.Translate(this.currentPhysic.walkSpeed * directionFactor * Time.deltaTime * Vector3.right);
            }
        }

        if (directionFactor != 0)
        {
            //Debug.Log("C'est toi qui fout la merde ?");
            this.FlipPatate(movingLeft);
        }
        else
        {
            //animator.SetBool("IsMoving", false);
        }

        // Vitesse max de chute. Pas possible d'éditer directement les composants de rigidbody.velocity
        if (this._rigidbody.linearVelocity.y < this.currentPhysic.maxFallingSpeed)
        {
            this._rigidbody.linearVelocity = new Vector2(this._rigidbody.linearVelocity.x, this.currentPhysic.maxFallingSpeed);
        }


        this.ResetWallJumpPhysicsIfNeeded();
    }

    private void ResetWallJumpPhysicsIfNeeded()
    {
        if (this._rigidbody.linearVelocity.y < 0.01f && this.lastTimeSinceWallJump > this.currentPhysic.wallJumpMinimalTime)   // Si on tombe, et que le walljump était il y a suffisamment longtemps
        {
            this.isWallJumping = false;
            if ((this.directionFactor == 1 && this.movingLeft) || (this.directionFactor == -1 && !this.movingLeft))
            {
                this.FlipPatate(!movingLeft);
            }
        }
    }

    private void FlipPatate(bool left)
    {
        spriteRenderer.flipX = left;
        this.wallCheck.Flip(left);
        this.movingLeft = left;
        //animator.SetInteger("Direction", (left ? -1 : 1);
        //animator.SetBool("IsMoving", true);
    }

    private void Jump()
    {
        //animator.SetBool("IsJumping", true);
        //audioManager.PlayClip(audioManager.jumpClip);
        this._rigidbody.linearVelocity = new Vector2(this._rigidbody.linearVelocity.x, 0);
        _rigidbody.AddForce(Vector2.up * this.currentPhysic.jumpImpulse, ForceMode2D.Impulse);
        _rigidbody.gravityScale = this.currentPhysic.jumpingGravity;
        lastTimeSinceJumped = 0f;   // TODO y'a un monde où cette ligne fout la merde
        this.releasedJump = false;
    }

    private bool IsGrounded()
    {
        float checkExtent = 0.03f;
        RaycastHit2D castHit = Physics2D.BoxCast(_collider.bounds.center, _collider.bounds.size - new Vector3(0.1f, 0, 0), 0f, Vector2.down, checkExtent, groundMask);
        bool isGrounded = (castHit.collider != null && !this.wallClipCheck.IsInsideWall());

        // Atterissage
        //if (isGrounded && animator.GetBool("IsAirborne"))
        {
            //audioManager.PlayClip(audioManager.landingClip);
            //animator.SetBool("IsJumping", false);
        }

        //animator.SetBool("IsAirborne", !isGrounded);

        if (isGrounded)
        {
            this._rigidbody.gravityScale = this.currentPhysic.normalGravity;
            this.timeSinceGrounded = 0f;
            this.lastTimeSinceJumped = 0f;
            this.firstJumpPerformed = false;
            this.doubleJumpPerformed = false;
        }

        if (!isGrounded/* && Math.Abs(_rigidbody.linearVelocity.y) > 0.1f*/)
        {
            //animator.SetBool("IsAirborne", true);
            this.timeSinceGrounded += Time.deltaTime;
        }

        Color rayColor = (isGrounded ? Color.green : Color.red);

        Debug.DrawRay(_collider.bounds.center + new Vector3(_collider.bounds.extents.x, 0), Vector2.down * (_collider.bounds.extents.y + checkExtent), rayColor);
        Debug.DrawRay(_collider.bounds.center - new Vector3(_collider.bounds.extents.x, 0), Vector2.down * (_collider.bounds.extents.y + checkExtent), rayColor);
        Debug.DrawRay(_collider.bounds.center - new Vector3(_collider.bounds.extents.x, _collider.bounds.extents.y + checkExtent), Vector2.right * (_collider.bounds.extents.x * 2f), rayColor);

        return isGrounded;
    }

    public void SetActive(bool activate)
    {
        canMove = activate;
        _rigidbody.gravityScale = (activate ? (Input.GetKey(KeyCode.Space) ? this.currentPhysic.jumpingGravity : this.currentPhysic.normalGravity) : 0);
    }

    public void SetWaterPhysics(bool activate)
    {
        this.currentPhysic = (activate ? this.underWaterPhysic : this.normalPhysic);
        this._rigidbody.gravityScale = this.currentPhysic.jumpingGravity;
        this._rigidbody.AddForce(Vector2.up * this.currentPhysic.jumpImpulse / 4, ForceMode2D.Impulse);
    }

    /// <summary>
    /// Permet de déclarer que le joueur est collé aux murs. Utilisé notamment pour éviter de clip dans les bords
    /// </summary>
    /// <param name="direction">Direction : -1 pour gauche, 0 pour aucune, 1 pour droite</param>
    public void SetWallsProximity(int direction)
    {
        switch (direction)
        {
            case -1:    // Gauche
                this.closeToWalls = new bool[] { true, false };
                break;
            case 0:     // Aucune direction
                this.closeToWalls = new bool[] { false, false };
                break;
            case 1:     // Droite
                this.closeToWalls = new bool[] { false, true };
                break;
        }
    }

    /**
     * Appelé par une instance de Transition pour faire changer Patate de salle
     */
    public void ChangeRoom(float transitionAngle = 0)
    {
        Vector2 positionShift = Vector2.left;
        switch (transitionAngle)
        {
            case 90:
                positionShift = Vector2.down;
                break;
            case 180:
                positionShift = Vector2.right;
                break;
            case -90:
            case 270:
                positionShift = Vector2.up;
                break;
        }

        positionShift *= this.transitionShiftDistance;

        this.transform.Translate(positionShift);
    }


    public void SetCanMove(bool canMove)
    {
        this.canMove = canMove;
    }

    public void SetTorche(Torche torche)
    {
        this.torche = torche;
    }

    public Torche GetTorche() 
    { 
        return this.torche; 
    }

    /**
     * Nom marrant pour dire qu'on commence à rouler avec un spindash
     */
    private void RockNRoll()
    {
        this.isRolling = true;
        this.canMove = true;
    }

    /**
     * Quand Patate roule contre un mur
     */
    private void Bonk()
    {
        this.isRolling = false;
        this._rigidbody.linearVelocity = Vector2.zero;
        this._rigidbody.AddForce(this.bonkForce * new Vector2((this.movingLeft ? 1 : -1), 2), ForceMode2D.Impulse);
        this.transform.localScale = this.originalScale;
        this.currentRollingTimeAgainstWall = 0f;
    }

    private IEnumerator ChargeSpinDash()
    {
        this.canMove = false;
        while (this.transform.localScale != this.rollingScale)
        {
            this.transform.localScale = Vector3.MoveTowards(this.transform.localScale, this.rollingScale, ((Time.deltaTime / this.currentPhysic.rollChargingTime) * (this.originalScale.x - this.rollingScale.x)));
            yield return new WaitForSeconds(0.01f);
        }
        this.RockNRoll();   // Après la charge, on lance le spin dash
    }
}
