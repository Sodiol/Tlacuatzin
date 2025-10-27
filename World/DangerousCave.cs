using Godot;
using System;

public partial class DangerousCave : Area2D
{
	private Node2D player;
	private float radius;
	private bool playerInside = false;
	private bool hasKilledPlayer = false;
	
	// Nueva referencia al ColorRect
	private ColorRect _oscuridad;
	
	[Export] public int ControllerId = 0;
	[Export] public float MaxVibration = 1.0f;
	[Export] public float MinVibration = 0.0f;
	
	private  AudioStreamPlayer2D catSound;
	
	[Signal]
	public delegate void PlayerisDeadEventHandler();
	
	public override void _Ready()
	{
		// Obtener el radio del círculo
		var shape = (CircleShape2D)GetNode<CollisionShape2D>("CollisionShape2D").Shape;
		radius = shape.Radius;
		catSound = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
		
		// Obtener el ColorRect (asegúrate de añadirlo como hijo en el editor)
		_oscuridad = GetNode<ColorRect>("ColorRect");
		_oscuridad.Visible = false;
		
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
	}
	
	private void OnBodyEntered(Node2D body)
	{
		if (body.IsInGroup("Player"))
		{
			player = body;
			playerInside = true;
			hasKilledPlayer = false;
			
			// Oscurecer la cueva cuando entra
			OscurecerCueva();
		}
	}
	
	private void OnBodyExited(Node2D body)
	{
		if (body == player)
		{
			playerInside = false;
			player = null;
			hasKilledPlayer = false;
			
			// Detener vibración cuando sale
			Input.StopJoyVibration(ControllerId);
			
			// Iluminar la cueva cuando sale
			IluminarCueva();
		}
	}
	
	public override void _Process(double delta)
	{
		if (!playerInside || player == null) return;
		
		// Calcular la distancia al jugador
		float distance = GlobalPosition.DistanceTo(player.GlobalPosition);
		
		// Normalizar (0 = centro, 1 = borde)
		float normalized = Mathf.Clamp(distance / radius, 0f, 1f);
		
		// Invertir: más cerca → mayor intensidad
		float intensity = 1f - normalized;
		
		// Escalar la vibración entre min y max
		float vibration = Mathf.Lerp(MinVibration, MaxVibration, intensity);
		
		// Aplicar vibración a los motores izquierdo y derecho del mando
		Input.StartJoyVibration(ControllerId, vibration, vibration, 0.05f);
		
		if(vibration >= 0.7f && !hasKilledPlayer)
		{
			hasKilledPlayer = true;
			EmitSignal(SignalName.PlayerisDead);
			GD.Print("Player is dead signal emitted");
			catSound.Stop();
		}
	}
	
	// Métodos para oscurecer/iluminar
	private void OscurecerCueva()
	{
		Tween tween = CreateTween();
		_oscuridad.Visible = true;
		_oscuridad.Modulate = new Color(1, 1, 1, 0);
		tween.TweenProperty(_oscuridad, "modulate:a", 1.0f, 3);
	}
	
	private void IluminarCueva()
	{
		Tween tween = CreateTween();
		tween.TweenProperty(_oscuridad, "modulate:a", 0.0f, 3);
		tween.TweenCallback(Callable.From(() => _oscuridad.Visible = false));
	}
}
