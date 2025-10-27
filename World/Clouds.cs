using Godot;
using System;

public partial class Clouds : Node2D
{
	private Random _rand = new Random();

	public override void _Ready()
	{
		// Darle una velocidad aleatoria a cada nube
		foreach (Node child in GetChildren())
		{
			if (child is Sprite2D cloud)
			{
				// Guardamos un valor de velocidad en Metadata
				cloud.SetMeta("speed", (float)_rand.Next(50, 200)); // entre 20 y 100
			}
		}
	}

	public override void _Process(double delta)
	{
		foreach (Node child in GetChildren())
		{
			if (child is Sprite2D cloud)
			{
				float speed = (float)cloud.GetMeta("speed");

				// Mover la nube hacia la izquierda
				cloud.Position += new Vector2(-speed * (float)delta, 0);

				// Obtener cámara actual
				var camera = GetViewport().GetCamera2D();
				if (camera != null)
				{
					float screenWidth = GetViewportRect().Size.X;
					float leftEdge = camera.GlobalPosition.X - screenWidth / 2;
					float rightEdge = camera.GlobalPosition.X + screenWidth / 2;

					// Ancho de la nube (considerando escala)
					float cloudWidth = cloud.Texture.GetSize().X * cloud.Scale.X;

					// Si salió de la cámara por la izquierda → reaparece a la derecha
					if (cloud.GlobalPosition.X < leftEdge - cloudWidth)
					{
						cloud.GlobalPosition = new Vector2(rightEdge, cloud.GlobalPosition.Y);
					}
				}
			}
		}
	}
}
