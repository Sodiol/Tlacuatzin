using Godot;

public partial class Player : CharacterBody2D
{
	
	
	
	[Export] public int Speed { get; set; } = 150;
	[Export] public int JumpForce { get; set; } = -1600;
	[Export] public int Gravity { get; set; } = 4000;
////////////////////////////////////
	
	private bool _inputEnabled = true;
	
	private AudioStreamPlayer2D DeadFall;
	private AudioStreamPlayer2D Garras;
	private AudioStreamPlayer2D stepSound;
	
	private AnimatedSprite2D anim;
	private Vector2 velocity = Vector2.Zero;
	

	// Dash vars
	private float dashTime = 0.15f;
	private bool hasDashed = false;
	private float dashTimer = 0f;
	private int dashSpeed = 1000;
	
	//SnowARE
	private SnowArea SnowArea;
	private DangerousCave DangerousCave;
	private Camera2d Camara;
	
	// Estados de salto
	private enum JumpState { Grounded, PreJump, Jumping, Falling, Landing }
	private JumpState jumpState = JumpState.Grounded;
	
	// Caminar Rápido
	private bool walkFastLeft = false;
	private bool walkFastRight = false;
	private float doubleTapWindow = 0.25f;
	private float lastTapTimeLeft = -1f;
	private float lastTapTimeRight = -1f;
	//varibles para intercalar sprites iguales en diferentes terrenos(wlkTlakuachil=snowWalk || walktlacuachil; IdleTlaucachil = idleSnow || idletlacuachil)
	string walkTlacuachil;
	string idleTlacuachil;
	
	public override void _Ready()
	{
		anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		anim.Connect("animation_finished", new Callable(this, nameof(OnAnimationFinished)));
		stepSound = GetNode<AudioStreamPlayer2D>("StepSound");
		AddToGroup("Player");
		SnowArea = GetTree().Root.GetNode<SnowArea>("World/Snow/SnowArea");
		DangerousCave = GetTree().Root.GetNode<DangerousCave>("World/DangerousCave");
		Camara = GetTree().Root.GetNode<Camera2d>("World/Camera2D");
		SnowArea.PlayerEntered += OnSnowAreaEntered;
		SnowArea.PlayerExited += OnSnowAreaExited;
		DangerousCave.PlayerisDead += OnPlayerIsDead;

		Garras = GetNode<AudioStreamPlayer2D>("Garras");
		DeadFall = GetNode<AudioStreamPlayer2D>("FallDead");
		
		walkTlacuachil="snowWalk";
		idleTlacuachil = "IdleSnow";
		
	}
	
	private void OnPlayerIsDead()
	{
		_inputEnabled = false;
		Camara.StartZoom();
		Garras.Play();
		anim.Play("CanonDeadCave");
		if(!Garras.Playing)
		{
			DeadFall.Play();
		}
		
		GD.Print("Player received death signal");
	}
	
	/////COntrol de animaciones si personaje esta en la nieve o no (Area2D llamada SnowArea)
	private void OnSnowAreaEntered()
	{
	   	walkTlacuachil = "snowWalk";
		idleTlacuachil = "IdleSnow";
		stepSound = GetNode<AudioStreamPlayer2D>("StepSound");
	}

	private void OnSnowAreaExited()
	{
		walkTlacuachil = "walkTlacuachil";
		idleTlacuachil = "idleTlacuachil";
		stepSound = GetNode<AudioStreamPlayer2D>("PasoSeco");
	}
	
	
	private void OnAnimationFinished()
	{
		if (jumpState == JumpState.PreJump && anim.Animation == "JumpPre")
		{
			// Termina JumpPre, ahora saltamos
			jumpState = JumpState.Jumping;
			velocity.Y = JumpForce;
			anim.Play("JumpFly");
		}
		
		if (jumpState == JumpState.Landing && anim.Animation == "Landing")
		{
			jumpState = JumpState.Grounded;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_inputEnabled) return;
		float deltaF = (float)delta;
		
		// ========== MOVIMIENTO HORIZONTAL ==========
		float targetVelocityX = CalculateHorizontalVelocity();
		
		// Solo aplicar fricción/aceleración si no estamos en dash
		if (dashTimer <= 0)
		{
			if (targetVelocityX != 0)
			{
				
					velocity.X = targetVelocityX;
				
			}
			else
			{
				// Fricción cuando no hay input
				if(jumpState == JumpState.Landing)
				{
					velocity.X = Mathf.MoveToward(velocity.X, 0, Speed * 12 * deltaF);
				}else{
					velocity.X = Mathf.MoveToward(velocity.X, 0, Speed * 2 * deltaF);
				}
				
			}
		}
		
		// ========== GRAVEDAD Y ESTADOS DE SALTO ==========
		HandleGravityAndJumpStates(deltaF);
		
		// ========== INICIAR SALTO ==========
		if (Input.IsActionJustPressed("jump") && jumpState == JumpState.Grounded)
		{
			StartJump();
		}
		
		// ========== LANDING ==========
		if (IsOnFloor() && jumpState == JumpState.Falling)
		{
			Land();
		}
		
		// ========== DASH EN EL AIRE ==========
		HandleAirDash(deltaF);
		
		// ========== SONIDO DE PASOS ==========
		HandleStepSound();
		
		// ========== APLICAR MOVIMIENTO ==========
		Velocity = velocity;
		MoveAndSlide();
		
		// Resetear velocidad vertical si golpea el techo
		if (IsOnCeiling())
		{
			velocity.Y = 0;
		}
		
		UpdateAnimations();
	}

	private float CalculateHorizontalVelocity()
	{
		float now = Time.GetTicksMsec() / 1000f;
		
		// Detectar doble tap izquierda
		if (Input.IsActionJustPressed("move_left"))
		{
			if (now - lastTapTimeLeft <= doubleTapWindow)
			{
				walkFastLeft = true;
			}
			lastTapTimeLeft = now;
		}
		
		// Detectar doble tap derecha
		if (Input.IsActionJustPressed("move_right"))
		{
			if (now - lastTapTimeRight <= doubleTapWindow)
			{
				walkFastRight = true;
			}
			lastTapTimeRight = now;
		}
		
		// Resetear walkFast cuando se suelta la tecla correspondiente
		if (Input.IsActionJustReleased("move_left"))
		{
			walkFastLeft = false;
		}
		
		if (Input.IsActionJustReleased("move_right"))
		{
			walkFastRight = false;
		}
		
		// Calcular velocidad objetivo
		float targetVel = 0;
		
		if (Input.IsActionPressed("move_left"))
		{
			targetVel = walkFastLeft ? -Speed * 3 : -Speed;
		}
		else if (Input.IsActionPressed("move_right"))
		{
			targetVel = walkFastRight ? Speed * 3 : Speed;
		}
		
		// Control aéreo adicional durante salto/caída
		if (jumpState == JumpState.Jumping || jumpState == JumpState.Falling)
		{
			if (Input.IsActionPressed("move_right"))
			{
				targetVel = Mathf.Max(targetVel, Speed);
			}
			else if (Input.IsActionPressed("move_left"))
			{
				targetVel = Mathf.Min(targetVel, -Speed);
			}
		}
		
		return targetVel;
	}

	private void HandleGravityAndJumpStates(float delta)
	{
		switch (jumpState)
		{
			case JumpState.Grounded:
				// No forzar velocity.Y = 0 aquí, MoveAndSlide lo maneja
				if (!IsOnFloor())
				{
					jumpState = JumpState.Falling;
				}
				else
				{
					// Aplicar gravedad suave para mantener pegado al suelo/pendientes
					velocity.Y = Gravity * delta;
				}
				break;

			case JumpState.PreJump:
				// Durante pre-salto, pequeña gravedad para mantener en el suelo
				velocity.Y = Gravity * delta * 0.1f;
				break;

			case JumpState.Jumping:
				// Aplicar gravedad
				velocity.Y += Gravity * delta;
				
				if (velocity.Y > 0)
				{
					jumpState = JumpState.Falling;
				}
				
				if (IsOnFloor() && velocity.Y >= 0)
				{
					jumpState = JumpState.Landing;
				}
				break;

			case JumpState.Falling:
				velocity.Y += Gravity * delta;
				break;
				
			case JumpState.Landing:
				// Pequeña gravedad para ayudar con pendientes
				velocity.Y = Gravity * delta * 0.5f;
				break;
		}
	}

	private void StartJump()
	{
		bool isMoving = Input.IsActionPressed("move_right") || Input.IsActionPressed("move_left");
		
		if (isMoving)
		{
			// Salto inmediato si se está moviendo
			jumpState = JumpState.Jumping;
			velocity.Y = JumpForce;
			anim.Play("JumpFly");
		}
		else
		{
			// Animación de pre-salto si está quieto
			jumpState = JumpState.PreJump;
			anim.Play("JumpPre");
			velocity.Y = 0;
		}
	}

	private void Land()
	{
		jumpState = JumpState.Landing;
		hasDashed = false;
		dashTimer = 0f;
		bool isMoving = Input.IsActionPressed("move_right") || Input.IsActionPressed("move_left");
		
		// Vibración del control (solo si hay un joystick conectado)
		if (Input.GetConnectedJoypads().Count > 0)
		{
			Input.StartJoyVibration(0, 0.5f, 1.0f, 0.1f);
		}
		
		if (isMoving)
		{
			// Cancelar landing si se está moviendo
			jumpState = JumpState.Grounded;
			anim.Play(walkTlacuachil);
		}
		else
		{
			anim.Play("Landing");
		}
	}

	private void HandleAirDash(float delta)
	{
		// Iniciar dash
		if (Input.IsActionJustPressed("Dash") && !hasDashed && jumpState != JumpState.Grounded && jumpState != JumpState.Landing)
		{
			hasDashed = true;
			dashTimer = dashTime;
			if(IsOnFloor())
			{
				dashTimer = 0;
			}
			// Determinar dirección del dash
			if (Input.IsActionPressed("move_left"))
			{
				velocity.X = -dashSpeed;
			}
			if(Input.IsActionPressed("move_right"))
			{
				velocity.X = dashSpeed;
			}
			/*else
			{
				// Dash en la dirección que está mirando
				velocity.X = anim.FlipH ? -dashSpeed : dashSpeed;
			}*/
		}
		
		// Mantener dash activo
		if (dashTimer > 0 && !IsOnFloor() && jumpState == JumpState.Falling)
		{
			dashTimer -= delta;
			velocity.Y = 0; // Mantener altura durante dash
		}
	}

	private void HandleStepSound()
	{
		if (velocity.X != 0 && IsOnFloor() && jumpState == JumpState.Grounded)
		{
			if (!stepSound.Playing)
			{
				stepSound.Play();
			}
		}
		else
		{
			if (stepSound.Playing)
			{
				stepSound.Stop();
			}
		}
	}

	private void UpdateAnimations()
	{
		// No cambiar animación durante ciertos estados
		if (jumpState == JumpState.PreJump || jumpState == JumpState.Landing)
		{
			return;
		}
		
		switch (jumpState)
		{
			case JumpState.Jumping:
				if (anim.Animation != "JumpFly")
				{
					anim.Play("JumpFly");
				}
				break;
				
			case JumpState.Falling:
				if (anim.Animation != "Fall")
				{
					anim.Play("Fall");
				}
				break;

			case JumpState.Grounded:
				if (velocity.X != 0 && anim.Animation != walkTlacuachil)
				{
					anim.Play(walkTlacuachil);
				}
				else if (velocity.X == 0 && anim.Animation != idleTlacuachil)
				{
					anim.Play(idleTlacuachil);
				}
				break;
		}

		// Flip horizontal basado en la dirección del movimiento
		if (velocity.X != 0)
		{
			anim.FlipH = velocity.X < 0;
		}
	}
}
