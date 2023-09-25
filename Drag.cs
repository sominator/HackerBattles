using Godot;
using System;

public class Drag : Panel
{
	private bool _mouseIn = false;
	private bool _isDragging = false;
	private bool _isOverDropZone = false;
	private Vector2 _startPosition;
	private GameManager _gm;

	public override void _Ready()
	{
		_startPosition = RectPosition;
		_gm = GetParent<GameManager>();
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
			}
			if (Input.IsActionJustReleased("left_click"))
			{ 
				_isDragging = false;
				if (_isOverDropZone)
				{
					RectPosition = new Vector2((_gm.CardsInDropZone * 50) + 25, 425);
					_gm.Drop();
				}
				else
				{
					RectPosition = _startPosition;
				}
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
	
	private void OnArea2DEntered(object area)
	{
		_isOverDropZone = true;
	}
	
	private void OnArea2DExited(object area)
	{
		_isOverDropZone = false;
	}
}
