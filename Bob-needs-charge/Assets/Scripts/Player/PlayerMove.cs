using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//gravity when is in bubble

public class PlayerMove : MonoBehaviour
{

	//PHYSICS
	[Header("Gravity")]
	public float gravityScale; //overrides rb.gravityScale
	public float fallGravityMult;
	public float quickFallGravityMult; //not

	[Header("Drag")]
	public float dragAmount; //drag in air
	public float frictionAmount; //drag on ground

	[Header("Other Physics")]
	[Range(0, 0.5f)] public float coyoteTime; //grace time to Jump after the player has fallen off a platformer


	//GROUND
	[Header("Run")]
	public float runMaxSpeed;
	public float runAccel;
	public float runDeccel;
	[Range(0, 1)] public float accelInAir;
	[Range(0, 1)] public float deccelInAir;
	[Space(5)]
	[Range(.5f, 2f)] public float accelPower;
	[Range(.5f, 2f)] public float stopPower;
	[Range(.5f, 2f)] public float turnPower;


	//JUMP
	[Header("Jump")]
	public float jumpForce;
	[Range(0, 1)] public float jumpCutMultiplier;
	[Space(10)]
	[Range(0, 0.5f)] public float jumpBufferTime; //time after pressing the jump button where if the requirements are met a jump will be automatically performed

	[Header("Wall Jump")]
	public Vector2 wallJumpForce;
	[Space(5)]
	[Range(0f, 1f)] public float wallJumpRunLerp; //slows the affect of player movement while wall jumping
	[Range(0f, 1.5f)] public float wallJumpTime;


	//WALL
	[Header("Slide")]
	public float slideAccel;
	[Range(.5f, 2f)] public float slidePower;


	//ABILITIES
	[Header("Dash")]
	public int dashAmount;
	public float dashSpeed;
	[Space(5)]
	public float dashAttackTime;
	public float dashAttackDragAmount;
	[Space(5)]
	public float dashEndTime; //time after you finish the inital drag phase, smoothing the transition back to idle (or any standard state)
	[Range(0f, 1f)] public float dashUpEndMult; //slows down player when moving up, makes dash feel more responsive (used in Celeste)
	[Range(0f, 1f)] public float dashEndRunLerp; //slows the affect of player movement while dashing
	[Space(5)]
	[Range(0, 0.5f)] public float dashBufferTime;


	//OTHER
	[Header("Other Settings")]
	public bool doKeepRunMomentum; //player movement will not decrease speed if above maxSpeed, letting only drag do so. Allows for conservation of momentum
	public bool doTurnOnWallJump; //player will rotate to face wall jumping direction

	#region COMPONENTS
	public Rigidbody2D rb { get; private set; }
	#endregion

	#region STATE PARAMETERS
	public bool IsFacingRight { get; private set; }
	public bool IsJumping { get; private set; }
	public bool IsWallJumping { get; private set; }
	public bool IsDashing { get; private set; }

	public float LastOnGroundTime { get; private set; }
	public float LastOnWallTime { get; private set; }

	private float _wallJumpStartTime;
	private int _lastWallJumpDir;

	private int _dashesLeft;
	private float _dashStartTime;
	private Vector2 _lastDashDir;
	private bool _dashAttacking;
	#endregion

	#region INPUT PARAMETERS
	public Vector2 MoveInput { get; private set; }
	public float LastPressedJumpTime { get; private set; }
	public float LastPressedDashTime { get; private set; }
	#endregion

	#region CHECK PARAMETERS
	[Header("Checks")]
	[SerializeField] private Transform _groundCheckPoint;
	[SerializeField] private Vector2 _groundCheckSize;
	[Space(5)]
	[SerializeField] private Transform _frontWallCheckPoint;
	[SerializeField] private Transform _backWallCheckPoint;
	[SerializeField] private Vector2 _wallCheckSize;
	#endregion

	#region LAYERS & TAGS
	[Header("Layers & Tags")]
	[SerializeField] private LayerMask _groundLayer;
	#endregion

	private PlayerControls controls;

	private void Awake()
	{
		controls = new PlayerControls();
		rb = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		#region SETUP INPUTS
		//InputHandler.instance.OnJumpPressed += args => OnJump(args);
		//InputHandler.instance.OnJumpReleased += args => OnJumpUp(args);
		//InputHandler.instance.OnDash += args => OnDash(args);
		#endregion

		SetGravityScale(gravityScale);
		IsFacingRight = true;
	}

	private void Update()
	{
		#region TIMERS
		LastOnGroundTime -= Time.deltaTime;
		LastOnWallTime -= Time.deltaTime;

		LastPressedJumpTime -= Time.deltaTime;
		LastPressedDashTime -= Time.deltaTime;
		#endregion

		#region GENERAL CHECKS
		if (MoveInput.x != 0)
			CheckDirectionToFace(MoveInput.x > 0);
		#endregion

		#region PHYSICS CHECKS
		if (!IsDashing && !IsJumping)
		{
			//Ground Check
			if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer)) //checks if set box overlaps with ground
				LastOnGroundTime = coyoteTime; //if so sets the lastGrounded to coyoteTime

			//Wall Check
			if (Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer))
				LastOnWallTime = coyoteTime;
		}
		#endregion

		#region GRAVITY
		if (!IsDashing)
		{
			if (rb.velocity.y >= 0 || IsWallJumping)
				SetGravityScale(gravityScale);
			else if (MoveInput.y < 0)
				SetGravityScale(gravityScale * quickFallGravityMult);
			else
				SetGravityScale(gravityScale * fallGravityMult);
		}
		#endregion

		#region JUMP CHECKS
		if (IsJumping && rb.velocity.y < 0)
		{
			IsJumping = false;
			//Debug.Break();
		}

		if (IsWallJumping && Time.time - _wallJumpStartTime > wallJumpTime)
			IsWallJumping = false;

		if (!IsDashing)
		{
			//Jump
			//if (CanJump() && LastPressedJumpTime < 0)
			//{
			//	IsJumping = true;
			//	IsWallJumping = false;
			//	Jump();
			//}
			//WALL JUMP
			if (CanWallJump() && LastPressedJumpTime > 0)
			{
				IsWallJumping = true;
				IsJumping = false;

				_wallJumpStartTime = Time.time;
				_lastWallJumpDir = (LastOnWallTime > 0) ? -1 : 1;

				WallJump(_lastWallJumpDir);
			}
		}
		#endregion

		#region DASH CHECKS
		if (DashAttackOver())
		{
			if (_dashAttacking)
			{
				_dashAttacking = false;
				StopDash(_lastDashDir); //begins stopping dash
			}
			else if (Time.time - _dashStartTime > dashAttackTime + dashEndTime)
			{
				IsDashing = false; //dash state over, returns to idle/run/inAir
			}
		}

		if (CanDash() && LastPressedDashTime > 0)
		{
			if (MoveInput != Vector2.zero)
				_lastDashDir = MoveInput;
			else
				_lastDashDir = IsFacingRight ? Vector2.right : Vector2.left;

			_dashStartTime = Time.time;
			_dashesLeft--;
			_dashAttacking = true;

			IsDashing = true;
			IsJumping = false;
			IsWallJumping = false;

			StartDash(_lastDashDir);
		}
		#endregion
	}

	private void FixedUpdate()
	{
		#region DRAG
		if (IsDashing)
			Drag(DashAttackOver() ? dragAmount : dashAttackDragAmount);
		else if (LastOnGroundTime <= 0)
			Drag(dragAmount);
		else
			Drag(frictionAmount);
		#endregion

		#region RUN
		if (!IsDashing)
		{
			if (IsWallJumping)
				Run(wallJumpRunLerp);
			else
				Run(1);
		}
		else if (DashAttackOver())
		{
			Run(dashEndRunLerp);
		}
		#endregion

		#region SLIDE
		if (LastOnWallTime > 0 && !IsJumping && !IsWallJumping && !IsDashing && LastOnGroundTime <= 0)
		{
			if (MoveInput.x > 0) //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			{
				Slide();
			}
		}
		#endregion
	}

	#region INPUT
	//These functions are called when an even is triggered in my InputHandler. You could call these methods through a if(Input.GetKeyDown) in Update
	private void OnMove(InputValue value)
	{
		MoveInput = value.Get<Vector2>();
	}

	private void OnJump(InputValue value)
	{
		if (CanJump())
		{
			IsJumping = true;
			IsWallJumping = false;
			Jump();
		}
		//LastPressedJumpTime = jumpBufferTime;
	}

	private void OnJumpUp(InputValue value)
	{
		if (CanJumpCut() || CanWallJumpCut())
			JumpCut();
	}

	private void OnDash(InputValue value)
	{
		LastPressedDashTime = dashBufferTime;
	}
	#endregion

	#region OnEnable/OnDisable
	private void OnEnable()
	{
		controls.Enable();
	}

	private void OnDisable()
	{
		controls.Disable();
	}
	#endregion*/

	#region MOVEMENT METHODS

	public void SetGravityScale(float scale)
	{
		rb.gravityScale = scale;
	}

	private void Drag(float amount)
	{
		Vector2 force = amount * rb.velocity.normalized;
		force.x = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(force.x)); //ensures we only slow the player down, if the player is going really slowly we just apply a force stopping them
		force.y = Mathf.Min(Mathf.Abs(rb.velocity.y), Mathf.Abs(force.y));
		force.x *= Mathf.Sign(rb.velocity.x); //finds direction to apply force
		force.y *= Mathf.Sign(rb.velocity.y);

		rb.AddForce(-force, ForceMode2D.Impulse); //applies force against movement direction
	}

	private void Run(float lerpAmount)
	{
		float targetSpeed = MoveInput.x * runMaxSpeed; //calculate the direction we want to move in and our desired velocity
		float speedDif = targetSpeed - rb.velocity.x; //calculate difference between current velocity and desired velocity

		#region Acceleration Rate
		float accelRate;

		//gets an acceleration value based on if we are accelerating (includes turning) or trying to decelerate (stop). As well as applying a multiplier if we're air borne
		if (LastOnGroundTime > 0)
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccel : runDeccel;
		else
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccel * accelInAir : runDeccel * deccelInAir;

		//if we want to run but are already going faster than max run speed
		if (((rb.velocity.x > targetSpeed && targetSpeed > 0.01f) || (rb.velocity.x < targetSpeed && targetSpeed < -0.01f)) && doKeepRunMomentum)
		{
			accelRate = 0; //prevent any deceleration from happening, or in other words conserve are current momentum
		}
		#endregion

		#region Velocity Power
		float velPower;
		if (Mathf.Abs(targetSpeed) < 0.01f)
		{
			velPower = stopPower;
		}
		else if (Mathf.Abs(rb.velocity.x) > 0 && (Mathf.Sign(targetSpeed) != Mathf.Sign(rb.velocity.x)))
		{
			velPower = turnPower;
		}
		else
		{
			velPower = accelPower;
		}
		#endregion

		// applies acceleration to speed difference, then is raised to a set power so the acceleration increases with higher speeds, finally multiplies by sign to preserve direction
		float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
		movement = Mathf.Lerp(rb.velocity.x, movement, lerpAmount); // lerp so that we can prevent the Run from immediately slowing the player down, in some situations eg wall jump, dash 

		rb.AddForce(movement * Vector2.right); // applies force force to rigidbody, multiplying by Vector2.right so that it only affects X axis 

		if (MoveInput.x != 0)
			CheckDirectionToFace(MoveInput.x > 0);
	}

	private void Turn()
	{
		Quaternion rotation = transform.localRotation; //stores rotation and rotate y axis
		if(IsFacingRight)
        {
			rotation.y = 180;
		} else
        {
			rotation.y = 0;
        }

		transform.localRotation = rotation;

		IsFacingRight = !IsFacingRight;
	}

	private void Jump()
	{
		//ensures we can't call a jump multiple times from one press
		LastPressedJumpTime = 0;
		LastOnGroundTime = 0;

		#region Perform Jump
		float force = jumpForce;
		if (rb.velocity.y < 0)
			force -= rb.velocity.y;

		rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
		#endregion
	}

	private void WallJump(int dir)
	{
		//ensures we can't call a jump multiple times from one press
		LastPressedJumpTime = 0;
		LastOnGroundTime = 0;
		LastOnWallTime = 0;

		#region Perform Wall Jump
		Vector2 force = new Vector2(wallJumpForce.x, wallJumpForce.y);
		force.x *= dir; //apply force in opposite direction of wall

		if (Mathf.Sign(rb.velocity.x) != Mathf.Sign(force.x))
			force.x -= rb.velocity.x;

		if (rb.velocity.y < 0) //checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
			force.y -= rb.velocity.y;

		rb.AddForce(force, ForceMode2D.Impulse);
		#endregion
	}

	private void JumpCut()
	{
		//applies force downward when the jump button is released. Allowing the player to control jump height
		rb.AddForce(Vector2.down * rb.velocity.y * (1 - jumpCutMultiplier), ForceMode2D.Impulse);
	}

	private void Slide()
	{
		//works the same as the Run but only in the y-axis
		float targetSpeed = 0;
		float speedDif = targetSpeed - rb.velocity.y;

		float movement = Mathf.Pow(Mathf.Abs(speedDif) * slideAccel, slidePower) * Mathf.Sign(speedDif);
		rb.AddForce(movement * Vector2.up, ForceMode2D.Force);
	}

	private void StartDash(Vector2 dir)
	{
		LastOnGroundTime = 0;
		LastPressedDashTime = 0;

		SetGravityScale(0);

		rb.velocity = dir.normalized * dashSpeed;
	}

	private void StopDash(Vector2 dir)
	{
		SetGravityScale(gravityScale);

		if (dir.y > 0)
		{
			if (dir.x == 0)
				rb.AddForce(Vector2.down * rb.velocity.y * (1 - dashUpEndMult), ForceMode2D.Impulse);
			else
				rb.AddForce(Vector2.down * rb.velocity.y * (1 - dashUpEndMult) * .7f, ForceMode2D.Impulse);
		}
	}
	#endregion

	#region CHECK METHODS
	public void CheckDirectionToFace(bool isMovingRight)
	{
		if (isMovingRight != IsFacingRight)
			Turn();
	}

	private bool CanJump()
	{
		return LastOnGroundTime >= 0 && !IsJumping;
	}

	private bool CanWallJump()
	{
		return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && _lastWallJumpDir == 1;
	}

	private bool CanJumpCut()
	{
		return IsJumping && rb.velocity.y > 0;
	}

	private bool CanWallJumpCut()
	{
		return IsWallJumping && rb.velocity.y > 0;
	}

	private bool CanDash()
	{
		if (_dashesLeft < dashAmount && LastOnGroundTime > 0)
			_dashesLeft = dashAmount;

		return _dashesLeft > 0;
	}

	private bool DashAttackOver()
	{
		return IsDashing && Time.time - _dashStartTime > dashAttackTime;
	}
	#endregion
}
