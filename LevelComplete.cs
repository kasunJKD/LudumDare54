using Godot;
using System;

public partial class LevelComplete : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		 // Show the cursor.
        Input.MouseMode = Input.MouseModeEnum.Visible;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void _on_quit_button_down()
	{
		GetTree().Quit();
	}

	public void _on_main_menu_button_down()
	{
		 var rootNode = GetTree().Root;
        // Replace "levelcomplete.tscn" with the correct path to your scene
        var scenePath = "res://Scenes/main_menu.tscn";
        
                    // Load the levelComplete.tscn scene.
        PackedScene levelCompleteScene = GD.Load<PackedScene>(scenePath);
        Node levelCompleteInstance = levelCompleteScene.Instantiate();

        // Add the new scene to the scene tree.
        rootNode.AddChild(levelCompleteInstance);
        // Optionally, remove the current scene (the one with the collision detection).
        QueueFree();
	}
}
