using Godot;
using System;

public partial class Bullet : Area2D
{
	public Vector2 velocity = Vector2.Zero;
	private Vector2 ScreenSize;
	private Playfield playfield;

	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		playfield = GetTree().Root.GetNode<Playfield>("Playfield");
	}

	public override void _Process(double delta)
	{
		Position += velocity * (float)delta;
		if (Position.X < 0 || Position.X > ScreenSize.X ||
			Position.Y < 0 || Position.Y > ScreenSize.Y)
		{
			QueueFree(); // Remove the bullet if it goes off-screen
		}
	}
	
	private void _on_area_entered(Area2D area)
	{
		if (area is Asteroid asteroid)
		{
			asteroid.QueueFree(); // Destroy the asteroid
			QueueFree(); // Destroy the bullet
			
			playfield.IncreaseScore(1); // Increment score
		}
	}
}
