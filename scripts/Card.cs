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

	//create label and sprite variables for card flipping
	public Label CardName;
	public Label CardText;
	public Sprite CardBack;

	//create private variables to store initial data
	private bool _mouseIn = false;
	private bool _isDragging = false;
	private bool _isDropped = false;
	private Panel _overSocket = null;
	private Card _zoomCard = null;
	private GameManager _gm;

	public override void _Ready()
	{
		//locate GameManager and set start position
		_gm = GetParent().GetParent<GameManager>();
		StartPosition = RectPosition;

		//store label and sprite for card flipping
		CardName = (Label)GetNode("Name");
		CardText = (Label)GetNode("Text");
		CardBack = (Sprite)GetNode("Sprite");

		//make "card back" visible
		CardName.Visible = false;
		CardText.Visible = false;
		CardBack.Visible = true;
	}

	public override void _Process(float delta)
	{
		if (_mouseIn)
		{
			//handle card Zooming
			if (_zoomCard == null && CardName.Visible)
			{
				_zoomCard = (Card)Duplicate();
				GetParent().AddChild(_zoomCard);
				_zoomCard.RectPosition = new Vector2(1275, 275);
				_zoomCard.RectScale = new Vector2(_zoomCard.RectScale * 2);
				CollisionShape2D _cardCollider = (CollisionShape2D)_zoomCard.GetNode("Area2D/CollisionShape2D");
				_cardCollider.SetDeferred("disabled", true);
				_zoomCard.FlipForMe();
			}

			//handle manual card flipping
			if (Input.IsActionPressed("shift"))
			{
				if (Input.IsActionJustPressed("left_click"))
				{
					if (_isDropped)
					{
						FlipForAll();
					}
					else
					{
						FlipForMe();
					}
					
				}
			}

			if (!IsDraggable) return;
			
			//handle dragging and render card over other game objects
			if (Input.IsActionPressed("left_click"))
			{
				CleanUpZoomCard();
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

					HandleDrop();
				}
				else
				{
					RectPosition = StartPosition;
				}
			}
		}
		base._Process(delta);
	}

	//flip the card locally
	public void FlipForMe()
	{
		//change visibility of label and sprite
		if (CardName.Visible)
		{
			CardName.Visible = false;
			CardText.Visible = false;
			CardBack.Visible = true;
		}
		else
		{
			CardName.Visible = true;
			CardText.Visible = true;
			CardBack.Visible = false;
		}
		
	}

	//flip the card locally and let the client know to tell the server
	public void FlipForAll()
	{
		FlipForMe();
		_gm.FlippedCard(ID);
	}

	//handle automatic card flipping based on whether the card has been played or flipped
	public void HandleDrop()
	{
		if (!_isDropped && CardName.Visible)
		{
			_gm.FlippedCard(ID);
		}
		_isDropped = true;
	}

	public void CleanUpZoomCard()
	{
		if (_zoomCard != null)
		{
			_zoomCard.QueueFree();
			_zoomCard = null;
		}
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
		CleanUpZoomCard();
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
