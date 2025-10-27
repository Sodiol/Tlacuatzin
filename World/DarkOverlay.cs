using Godot;
using System;

public partial class DarkOverlay : ColorRect
{
	[Export] public ColorRect Overlay;
	[Export] public Node2D Player;
	[Export] public Node2D CaveCenter;
	[Export] public float MaxDarkness = 0.8f;
	[Export] public float MaxDistance = 800f;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Overlay == null || Player == null || CaveCenter == null)
		return;
		float dist = Player.GlobalPosition.DistanceTo(CaveCenter.GlobalPosition);
		float darkness = Mathf.Clamp(1f - (dist / MaxDistance), 0f, 1f) * MaxDarkness;
		Overlay.Modulate = new Color(0, 0, 0, darkness);
	}
}
