using Godot;
using System;

public class Login : Panel
{
	//keep track of room id line edit
	LineEdit _roomId;

	public override void _Ready()
	{
		//store room id line edit
		_roomId = (LineEdit)GetNode("LineEditRoomId");

	}

	//handle room creation, switch scenes, and destroy login screen
	private void OnLoginWithoutId()
	{
		Node _mainScene = ResourceLoader.Load<PackedScene>("res://Main.tscn").Instance();
		GetTree().Root.AddChild(_mainScene);
		QueueFree();
	}

	//handle login with existing room id, switch scenes, and destroy login screen
	private void OnLoginWithId()
	{
		Node _mainScene = ResourceLoader.Load<PackedScene>("res://Main.tscn").Instance();
		Node _client = _mainScene.GetNode("Client");
		_client.Set("roomId", _roomId.Text);
		GetTree().Root.AddChild(_mainScene);
		QueueFree();
	}

}
