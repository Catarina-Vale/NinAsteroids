using System;
using Godot;

public partial class Playfield : Node2D
{
	private Player _player;
	public Vector2 ScreenSize;
	private int score = 0;
	private Label scoreLabel;
	private bool isGameOver = false;

	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		_player = GetNode<Player>("Player");

		if (_player == null)
		{
			GD.PrintErr("Player node not found!");
		}
		scoreLabel = GetNode<Label>("ScoreLabel"); // Get the Label node
		UpdateScore();
	}
	
	public override void _Process(double delta)
	{
		
		if (GD.Randf() < 0.01 && !isGameOver) // Random probability to spawn asteroid
		{
			SpawnAsteroid();
		}
		if (Input.IsActionJustPressed("Reset"))
		{
			RestartGame();
		}
		if(isGameOver){
			ShowGameOver();
		}
	}

	private void SpawnAsteroid()
	{
		// Load the asteroid scene
		PackedScene asteroidScene = (PackedScene)ResourceLoader.Load("res://src/asteroid.tscn");

		if (asteroidScene == null)
		{
			GD.PrintErr("Asteroid scene could not be loaded!");
			return;
		}

		// Instantiate the asteroid
		Node2D asteroidNode = (Node2D)asteroidScene.Instantiate();

		if (asteroidNode == null)
		{
			GD.PrintErr("Failed to instantiate asteroid!");
			return;
		}

		// Cast it to Asteroid class
		Asteroid asteroid = asteroidNode as Asteroid;

		if (asteroid == null)
		{
			GD.PrintErr("Failed to cast asteroid node to Asteroid class!");
			return;
		}

		// Set the asteroid position (you can modify the position based on your logic)
		Vector2 spawnPos = GetRandomEdgePosition();
		asteroid.Position = spawnPos;

		// Calculate direction and set velocity
		Vector2 directionToPlayer = (_player.Position - spawnPos).Normalized();
		asteroid.SetDirection(directionToPlayer);

		// Add the asteroid to the scene
		AddChild(asteroid);
		asteroid.AddToGroup("asteroids");
	}

	private Vector2 GetRandomEdgePosition()
	{
		int edge = (int)GD.RandRange(0, 4);  // Randomly choose an edge (0 = top, 1 = bottom, 2 = left, 3 = right)

		switch (edge)
		{
			case 0:  // Top edge
				return new Vector2((float)GD.RandRange(0, ScreenSize.X), 0f);  // Random X, Y = 0
			case 1:  // Bottom edge
				return new Vector2((float)GD.RandRange(0, ScreenSize.X), (float)ScreenSize.Y);  // Random X, Y = screen height
			case 2:  // Left edge
				return new Vector2(0f, (float)GD.RandRange(0, ScreenSize.Y));  // X = 0, Random Y
			case 3:  // Right edge
				return new Vector2((float)ScreenSize.X, (float)GD.RandRange(0, ScreenSize.Y));  // X = screen width, Random Y
			default:
				return Vector2.Zero;  // This should never happen
		}
	}
	
	public void IncreaseScore(int points)
	{
		score += points;
		UpdateScore();
	}

	private void UpdateScore()
	{
		scoreLabel.Text = $"Score: {score}";
	}
	
	public void SetGameOver(bool status){
		isGameOver = status;
	}
	
	private void RestartGame()
	{
		score = 0;
		UpdateScore();
		if(isGameOver){
			var playerScene = (PackedScene)ResourceLoader.Load("res://src/Player.tscn");
			_player = (Player)playerScene.Instantiate();
			GetTree().Root.AddChild(_player);
			
			var gameOverLabels = GetTree().GetNodesInGroup("GOLAbel");
			foreach (var golabels in gameOverLabels)
			{
				golabels.QueueFree();
			}
		}
		// Reload the playfield or reset other game elements here
		isGameOver = false;
		var asteroids = GetTree().GetNodesInGroup("asteroids");
		foreach (var asteroid in asteroids)
		{
			asteroid.QueueFree();  // Remove the asteroid
		}
		// You can also respawn the player and other entities here
		_player.SetVelocityToZero();
		_player.Position = ScreenSize/2;
	}
	
	

	private void ShowGameOver()
	{
		var gameOverLabel = new Label();
		gameOverLabel.Text = "GAME OVER\nPress R to Restart";

		// Set the position of the label to be in the center of the screen
		gameOverLabel.Position = ScreenSize/2;

		// Add the label as a child to the scene (so it's visible)
		AddChild(gameOverLabel);
		gameOverLabel.AddToGroup("GOLAbel");
	}
}
