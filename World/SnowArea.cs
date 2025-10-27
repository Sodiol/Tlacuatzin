	using Godot;
	using System.Collections.Generic;

	public partial class SnowArea : Area2D
	{
		
		[Signal]
		public delegate void PlayerEnteredEventHandler();
		[Signal]
		public delegate void PlayerExitedEventHandler();
		
		private List<int> joypads = new List<int>();
		private bool playerInside = false;
		private AudioEffectCapture capture;

		public override void _Ready()
		{
			// Buscar mandos
			var ids = Input.GetConnectedJoypads();
			foreach (int id in ids)
				joypads.Add(id);

			// Obtener el efecto de captura del bus "WindBus"
			int busIdx = AudioServer.GetBusIndex("WindBus");
			capture = (AudioEffectCapture)AudioServer.GetBusEffect(busIdx, 0);
		}

		public override void _Process(double delta)
		{
			if (!playerInside || capture == null)
				return;

			// Leer samples del audio
			var data = capture.GetBuffer(capture.GetFramesAvailable());
			if (data.Length == 0)
				return;

			// Calcular RMS (intensidad del sonido)
			float sum = 0f;
			for (int i = 0; i < data.Length; i++)
			{
				// data[i] es Vector2, así que calculamos la magnitud para obtener un float
				Vector2 sample = data[i];
				float magnitude = sample.Length(); // O puedes usar (sample.X + sample.Y) / 2f para promedio
				sum += magnitude * magnitude;
			}
			float rms = Mathf.Sqrt(sum / data.Length);

			// Mapear RMS a intensidad de vibración
			float strength = Mathf.Clamp(rms * 5f, 0f, 1f);
			
			foreach (int id in joypads)
				Input.StartJoyVibration(id, strength * 0.3f, strength * 0.6f, 0.1f);
		}

		private void _on_body_entered(Node body)
		{
			if (body.IsInGroup("Player"))
			{
				playerInside = true;
				EmitSignal(SignalName.PlayerEntered);//Enviar Señal al Player
			}
		}

		private void _on_body_exited(Node body)
		{
			if (body.IsInGroup("Player"))
			{
				playerInside = false;
				EmitSignal(SignalName.PlayerExited);// Enviar señal al Player
				foreach (int id in joypads)
					Input.StopJoyVibration(id);
			}
		}
	}
