using Godot;
using System;

public partial class Asteroid : Area2D
{
	private Sprite2D _sprite;
	private CollisionShape2D _collisionShape;
	private Vector2 _velocity;
	private Vector2 _direction;
	private Vector2 _screenSize;
	private Playfield playfield;
	private const float Speed = 100f;  // You can adjust this value as needed

	private struct AsteroidSize
	{
		public string SpritePath;
		public float Radius;

		public AsteroidSize(string spritePath, float radius)
		{
			SpritePath = spritePath;
			Radius = radius;
		}
	}

	private AsteroidSize[] _sizes = new AsteroidSize[]
	{
		new AsteroidSize("res://src/arts/asteroid1.png", 20f),
		new AsteroidSize("res://src/arts/asteroid2.png", 20f),
		new AsteroidSize("res://src/arts/asteroid3.png", 20f),
		new AsteroidSize("res://src/arts/asteroid4.png", 20f)
	};

	// Method to set the direction of the asteroid
	public void SetDirection(Vector2 direction)
	{
		// Normalize the direction to ensure it has a length of 1
		_direction = direction.Normalized();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playfield = GetParent<Playfield>();
		_sprite = GetNode<Sprite2D>("Sprite2D");
		_collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");

		// Randomly select an asteroid size
		var randomSize = _sizes[GD.RandRange(0, _sizes.Length-1)];

		// Set the sprite texture and collision shape size based on the selected asteroid size
		_sprite.Texture = (Texture2D)ResourceLoader.Load(randomSize.SpritePath);
		var circleShape = (CircleShape2D)_collisionShape.Shape;
		circleShape.Radius = randomSize.Radius;

		// Get screen size for boundary checks
		_screenSize = GetViewportRect().Size;

		// Set a default random velocity to simulate movement if no direction is set initially
		_velocity = new Vector2(
			GD.RandRange(-100, 100),  // Random X speed between -100 and 100 pixels/sec
			GD.RandRange(-100, 100)   // Random Y speed between -100 and 100 pixels/sec
		);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// If a direction has been set, update the velocity based on the direction
		if (_direction != Vector2.Zero)
		{
			// Apply the direction to move the asteroid at a constant speed
			_velocity = _direction * Speed;
		}

		// Update the asteroid's position based on its velocity
		Position += _velocity * (float)delta;

		// Destroy the asteroid if it goes off-screen
		if (Position.X < 0 || Position.X > _screenSize.X ||
			Position.Y < 0 || Position.Y > _screenSize.Y)
		{
			QueueFree();
		}
	}
	
	// Handle collision with the player
	private void _on_body_entered(Node body)
	{
		if (body is Player player)
		{
			player.QueueFree(); // Destroy the player
			playfield.SetGameOver(true);
		}
	}
}
