using Godot;
using System;
public class Card : Panel
{
	//ID to track card instance
	public int ID { get; set; }

	//string to track card type ("PlayerCard" or "OpponentCard")
	public string CardType { get; set; }

	//vector to track card start position
	public Vector2 StartPosition { get; set; }

	//bool to check whether card is draggable
	public bool IsDraggable { get; set; }

	//create private variables to store initial data
	private bool _mouseIn = false;
	private bool _isDragging = false;
	private Panel _overSocket = null;
	private GameManager _gm;

	public override void _Ready()
	{
		//locate GameManager and set start position
		_gm = GetParent<GameManager>();
		StartPosition = RectPosition;
	}

	public override void _Process(float delta)
	{
		if (_mouseIn)
		{
			if (!IsDraggable) return;
			
			//handle dragging and render card over other game objects
			if (Input.IsActionPressed("left_click"))
			{
				_isDragging = true;	
				Vector2 _mousePosition = new Vector2(GetViewport().GetMousePosition());
				RectPosition = new Vector2(_mousePosition.x - 40, _mousePosition.y - 40);
				GetParent().MoveChild(this, GetParent().GetChildCount());
			}
			//handle dropping or return card to start position if not over dropzone
			if (Input.IsActionJustReleased("left_click"))
			{ 
				_isDragging = false;
				if (_overSocket != null)
				{
					RectPosition = _overSocket.RectPosition;
					_gm.DroppedCard(ID, _overSocket.Name);
				}
				else
				{
					RectPosition = StartPosition;
				}
			}
		}
		base._Process(delta);
	}

	//handle mouse enter signal
	private void OnMouseEntered()
	{
		if (_isDragging) return;
		_mouseIn = true;
	}
	//handle mouse exit signal
	private void OnMouseExited()
	{
		_mouseIn = false;
	}
	//handle enter collision with dropzone signal
	private void OnArea2DEntered(object area)
	{
		Area2D socket = area as Area2D;
		_overSocket = (Panel)socket.GetParent();
		GD.Print(socket.GetParent().Name);
	}
	//handle exit collision with dropzone signal
	private void OnArea2DExited(object area)
	{
		GD.Print("End colliding");
		_overSocket = null;
	}
}
