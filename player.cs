using Godot;
using System;

public partial class player : CharacterBody3D
{

	public const float Speed = 5.0f;
	public const float CrouchedSpeed = 3.0f;
	public const float JumpVelocity = 4.5f;
	public const float Sensitivity = 3.0f;
	public bool IsCrouched;
	public bool FlashlightOut;
	private LightDetect LightDetectObject;
	public double LightValue;
	public double NoiseValue;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	private AudioStreamPlayer footAudioPlayer;
	private AudioStreamPlayer jumpAudioPlayer;
	private bool wasInAirLastFrame;

	private Surface surfaceInit;

	public override void _Ready()
	{
		base._Ready();
		Input.MouseMode = Input.MouseModeEnum.Captured;
		LightDetectObject = GetNode<LightDetect>("LightDetect");
		surfaceInit = new Surface();
		surfaceInit.SurfaceResource = ResourceLoader.Load<SurfaceResource>("res://Sounds/Wood.tres");

		footAudioPlayer = GetNode<AudioStreamPlayer>("Footsteps");
		jumpAudioPlayer = GetNode<AudioStreamPlayer>("Jump");
	}

	public override void _PhysicsProcess(double delta)
	{
		NoiseValue = 0;
		Vector3 velocity = Velocity;
		PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;
		var surfaceResult = spaceState.IntersectRay(new PhysicsRayQueryParameters3D() {
			From = new Vector3(Position.X, Position.Y + 1, Position.Z),
			To = new Vector3(Position.X, Position.Y - 1, Position.Z),
										Exclude = new Godot.Collections.Array<Rid> { this.GetRid() }
			});
		Surface surface = surfaceInit;
		if (surfaceResult.Count > 0)
		{
			if(surfaceResult.ContainsKey("collider")){
				if(surfaceResult["collider"].AsGodotObject() is Surface)
				{
					surface = (Surface)surfaceResult["collider"];
				}
			}
		}

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if(IsOnFloor())
		{
			if(wasInAirLastFrame)
			{
				jumpAudioPlayer.Stream = surface.SurfaceResource.JumpLandStreamWAV;
				jumpAudioPlayer.Play();
				NoiseValue = surface.SurfaceResource.JumpLandNoiseLevel;
			}
			if (Input.IsActionJustPressed("ui_accept"))
			{
				velocity.Y = JumpVelocity;
			}
		}

		LightValue = LightDetectObject.LightLevel;
		GD.Print(LightValue);

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBackward");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		float speed = Speed;
		if(Input.IsActionPressed("Crouch"))
		{
			if(!IsCrouched)
			{
				GetNode<AnimationPlayer>("AnimationPlayer").Play("Crouch");
				IsCrouched = true;
			}
			
		}else
		{
			if(IsCrouched)
			{
				//PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;
				var result = spaceState.IntersectRay(new PhysicsRayQueryParameters3D() {
					From = Position, To = new Vector3(Position.X, Position.Y + 2, Position.Z),
											 Exclude = new Godot.Collections.Array<Rid> { this.GetRid() }
					});
				if (result.Count <= 0)
				{
					GetNode<AnimationPlayer>("AnimationPlayer").Play("UnCrouch");
					IsCrouched = false;	
				}
			}
		}
		if(IsCrouched)
		{
			speed = CrouchedSpeed;
		}

		if(Input.IsActionJustPressed("Flashlight"))
		{
			if(FlashlightOut)
			{
				GetNode<AnimationPlayer>("AnimationPlayer").Play("FlashLightHide");
			}
			else
			{
				GetNode<AnimationPlayer>("AnimationPlayer").Play("FlashLight");
			}
			FlashlightOut = !FlashlightOut;
		}


		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * speed;
			velocity.Z = direction.Z * speed;
			if(IsOnFloor())
			{

			
			if(IsCrouched)
			{
				NoiseValue = surface.SurfaceResource.NoiseLevel / 3;
			}
			else{
				NoiseValue = surface.SurfaceResource.NoiseLevel;
			}
			if(!footAudioPlayer.Playing)
			{
				if(IsCrouched){
					footAudioPlayer.Stream = surface.SurfaceResource.SneakStreamWAV;
				}
				else 
				{
					footAudioPlayer.Stream = surface.SurfaceResource.WalkStreamWAV;
				}
				footAudioPlayer.Play();
			} else if(!IsOnFloor())
			{
				footAudioPlayer.Stop();
			}
			}
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, speed);
			if(footAudioPlayer.Playing && !IsOnFloor())
			{
				footAudioPlayer.Stop();
			}
		}

		Velocity = velocity;
		wasInAirLastFrame = !IsOnFloor();
		MoveAndSlide();
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if(@event is InputEventMouseMotion)
		{
			InputEventMouseMotion motion = @event as InputEventMouseMotion;
			Rotation = new Vector3(Rotation.X, Rotation.Y - motion.Relative.X/1000 * Sensitivity, Rotation.Z);
			Camera3D camera = GetNode<Camera3D>("Camera3D");
			camera.Rotation = new Vector3(Mathf.Clamp(camera.Rotation.X - motion.Relative.Y/1000 * Sensitivity, -2 ,2), camera.Rotation.Y, camera.Rotation.Z);
		} 
	}
}
