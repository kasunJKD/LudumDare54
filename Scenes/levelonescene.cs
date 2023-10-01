using Godot;
using System;

public partial class levelonescene : Node3D
{
	[Export]
    private AudioStream audioStream; // Exported variable to set the audio stream in the Inspector

    private AudioStreamPlayer audioPlayer;
	private bool hasPlayedSound = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        audioPlayer.Stream = audioStream; 
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void _on_e_demon_sounds_body_entered(Node3D body)
	{
		if (!hasPlayedSound && body.IsInGroup("Player")) // Replace "player" with the group name of the object you want to trigger the sound
        {
            PlaySound();
        }
	}

	private void PlaySound()
    {
        if (audioStream != null)
        {
            audioPlayer.Play();
            hasPlayedSound = true; // Set the flag to prevent further plays
        }
    }
	public void StartLevelCompleteScene()
    {
        var rootNode = GetTree().Root;
        // Replace "levelcomplete.tscn" with the correct path to your scene
        var scenePath = "res://Scenes/LevelComplete.tscn";
        
                    // Load the levelComplete.tscn scene.
        PackedScene levelCompleteScene = GD.Load<PackedScene>(scenePath);
        Node levelCompleteInstance = levelCompleteScene.Instantiate();

        // Add the new scene to the scene tree.
        rootNode.AddChild(levelCompleteInstance);
        // Optionally, remove the current scene (the one with the collision detection).
        QueueFree();
    }

	public void _on_e_exit_body_entered(Node3D body)
	{
		if (body.IsInGroup("Player")) // Replace "player" with the group name of the object you want to trigger the sound
        {
            StartLevelCompleteScene();
        }
	}

}
