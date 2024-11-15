using Godot;
using System;

public partial class Player : Node2D
{
	// Speed variables
	private float acceleration = 1000f; 
	private float maxSpeed = 300f;   
	private float deceleration = 500f;
	private Vector2 velocity = Vector2.Zero; 
	private Vector2 ScreenSize;
	private PackedScene bulletScene;
	private float bulletSpeed = 500f; 

	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		Position = ScreenSize/2;
		bulletScene = GD.Load<PackedScene>("res://src/bullet.tscn");
	}
	
	public void SetVelocityToZero(){
		velocity = Vector2.Zero;
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("spin_left"))
		{
			Rotation -= 0.05f;
		}

		if (Input.IsActionPressed("spin_right"))
		{
			Rotation += 0.05f;
		}
		if (Input.IsActionPressed("accelerate"))
		{
			velocity += new Vector2(0, -acceleration * (float)delta).Rotated(Rotation);
			if (velocity.Length() > maxSpeed) 
			{
				velocity = velocity.Normalized() * maxSpeed;
			}
		}

		if (Input.IsActionPressed("decelarate"))
		{
			velocity -= velocity.Normalized() * deceleration * (float)delta;
			if (velocity.Length() < 10f) 
			{
				velocity = Vector2.Zero;
			}
		}
		
		if (Input.IsActionJustPressed("fire"))
		{
			FireBullet();
		}

		Position += velocity * (float)delta; 
		Position = new Vector2(
		x: (Position.X + ScreenSize.X) % ScreenSize.X,
		y: (Position.Y + ScreenSize.Y) % ScreenSize.Y
	);
	}
	private void FireBullet()
	{
		if (bulletScene != null)
		{
			var bullet = (Node2D)bulletScene.Instantiate();

			bullet.Position = Position;

			Vector2 direction = new Vector2(0, -1).Rotated(Rotation); 

			bullet.Set("velocity", direction * bulletSpeed);

			GetParent().AddChild(bullet);
		}
	}
}
