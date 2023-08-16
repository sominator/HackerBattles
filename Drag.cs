using Godot;
using System;

public class Drag : Panel
{
	private bool _mouseIn = false;
	private bool _isDragging = false;
	public override void _Ready()
	{
		
		
	}

	public override void _Process(float delta)
	{
		if (_mouseIn)
		{
			if (Input.IsActionPressed("left_click"))
			{
				_isDragging = true;	
				Vector2 _mousePosition = new Vector2(GetViewport().GetMousePosition());
				RectPosition = new Vector2(_mousePosition.x - 40, _mousePosition.y - 40);
				GetParent().MoveChild(this, GetParent().GetChildCount());

				GD.Print(GetParent().GetChildCount());
			}
			if (Input.IsActionJustReleased("left_click"))
			{ 
				_isDragging = false;
			}
		}
		base._Process(delta);
	}

	private void OnMouseEntered()
	{
		if (_isDragging) return;
		_mouseIn = true;
	}

	private void OnMouseExited()
	{
		_mouseIn = false;
	}
}
