using Godot;
using System;

public class Login : Panel
{
	LineEdit _roomId;
	public override void _Ready()
	{
		_roomId = (LineEdit)GetNode("LineEdit");

	}

	private void OnLoginWithoutId()
	{
		Node _mainScene = ResourceLoader.Load<PackedScene>("res://Main.tscn").Instance();
		GetTree().Root.AddChild(_mainScene);
		QueueFree();
	}


	private void OnLoginWithId()
	{
		Node _mainScene = ResourceLoader.Load<PackedScene>("res://Main.tscn").Instance();
		Node _client = _mainScene.GetNode("Client");
		_client.Set("roomId", _roomId.Text);
		GetTree().Root.AddChild(_mainScene);
		QueueFree();
	}

}
