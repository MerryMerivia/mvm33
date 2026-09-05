using System;
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

        public PhysicsVariables(float walkSpeed, float delayBeforeNormalGravity, float jumpImpulse, float normalGravity, float jumpingGravity, float maxFallingSpeed)
        {
            this.walkSpeed = walkSpeed;
            this.delayBeforeNormalGravity = delayBeforeNormalGravity;
            this.jumpImpulse = jumpImpulse;
            this.normalGravity = normalGravity;
            this.jumpingGravity = jumpingGravity;
            this.maxFallingSpeed = maxFallingSpeed;
        }
    }

    [SerializeField] PhysicsVariables currentPhysic;
    [SerializeField] PhysicsVariables normalPhysic;
    [SerializeField] PhysicsVariables underWaterPhysic;

    [SerializeField] private LayerMask groundMask;
    [SerializeField] private AnimationClip deathAnimationClip;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Collider2D _collider;

    private float lastTimeSinceJumped = 0;
    private bool wantsToJump = false;

    private float directionFactor = 0f;
    private bool movingLeft = false;

    private float timeSinceGrounded = 0f;
    private float coyoteJumpWindow = 0.1f;
    public bool[] closeToWalls = new bool[2] { false, false };

    private Torche torche;

    private bool canMove = true;

    private bool firstJumpPerformed = false;
    private bool doubleJumpUnlocked = true;
    private bool doubleJumpPerformed = false;

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
        //GetComponent<PlayerInput>().currentActionMap
    }

    // Update is called once per frame
    void Update()
    {
        if (cheat.WasPerformedThisFrame())
        {
            Debug.Log("C'est celle là");
            this.doubleJumpUnlocked = !this.doubleJumpUnlocked;
            Debug.Log(doubleJumpUnlocked);
        }

        if (this.canMove)
        {
            CheckMovement();
        }
    }

    private void FixedUpdate()
    {
        if (this.canMove)
        this.MovePatate();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
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

    /**
     * Déplace notre patate en fonction de l'accélération lue dans CheckMovements()
     */
    private void MovePatate()
    {
        // Le saut
        if ((((IsGrounded() || this.timeSinceGrounded < this.coyoteJumpWindow) && !this.firstJumpPerformed) // Touche le sol ou coyote jump, pour saut simple
            || (this.doubleJumpUnlocked && !this.doubleJumpPerformed))    // Ou alors double jump, c'est bien aussi
            // Il faut au moins un des deux au dessus, et celui en dessous
            && wantsToJump && this._rigidbody.linearVelocity.y < 0.1f)  // Veut sauter
        {
            if (this.firstJumpPerformed)
            {
                this.doubleJumpPerformed = true;
            } 
            this.firstJumpPerformed = true;

            //animator.SetBool("IsJumping", true);
            //audioManager.PlayClip(audioManager.jumpClip);
            this._rigidbody.linearVelocity = new Vector2(this._rigidbody.linearVelocity.x, 0);
            _rigidbody.AddForce(Vector2.up * this.currentPhysic.jumpImpulse, ForceMode2D.Impulse);
            _rigidbody.gravityScale = this.currentPhysic.jumpingGravity;
        }
        //wantsToJump = false;

        // Les directions
        if (!this.closeToWalls[1]) // Pour corriger le bug d'entrée partielle dans le mur
        {
            transform.Translate(this.currentPhysic.walkSpeed * directionFactor * Time.deltaTime * Vector3.right);
        }

        if (directionFactor != 0)
        {
            if(movingLeft)
            {
                spriteRenderer.flipX = true;
                //animator.SetInteger("Direction", -1);
                //animator.SetBool("IsMoving", true);
            }
            else
            {
                spriteRenderer.flipX = false;
                //animator.SetInteger("Direction", 1);
                //animator.SetBool("IsMoving", true);
            }
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
    }

    private bool IsGrounded()
    {
        float checkExtent = 0.03f;
        RaycastHit2D castHit = Physics2D.BoxCast(_collider.bounds.center, _collider.bounds.size - new Vector3(0.1f, 0, 0), 0f, Vector2.down, checkExtent, groundMask);
        bool isGrounded = castHit.collider != null;

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
}
