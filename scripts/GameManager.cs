using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public class GameManager : Node
{
	//expose cards and card back in editor
	[Export]
	public PackedScene PingScene;

	[Export]
	public PackedScene BooleanScene;

	[Export]
	public PackedScene CardBackScene;

	//keep track of number of cards in dropzone
	public int CardsInDropZone { get; set; }

	//keep track of number of opponent card backs to render
	private List<Panel> opponentCards = new List<Panel>();

	//store array of cards to shuffle
	private Array<string> _cards = new Array<string> { "boolean", "defrag", "double", "echo", "firewall", "float", "glitch", "handshake", "host", "ping", "probe", "reInitialize", "scrape", "splice", "turnkey" };

	public override void _Ready()
	{
		CardsInDropZone = 0;

		//seed randomizer
		GD.Randomize();
		GD.Print(_cards);

		base._Ready();
	}

	public void DealCards()
	{
		//shuffle card array
		_cards.Shuffle();
		GD.Print(_cards);
	}

	//draw cards and emit signal to client object that cards have been drawn
	private void OnDrawCardsButtonDown()
	{
		for (int i = 0; i < 5; i++)
		{
			Panel card = PingScene.Instance<Panel>();
			card.RectPosition = new Vector2((i * 150) + 25, 825);
			AddChild(card);
		}
		EmitSignal(nameof(CardsDrawn));
	}

	//render opponent cards upon receiving signal from client object
	private void RenderOpponentCardBacks()
	{
		for (int i = 0; i < 5; i++)
		{
			Panel card = CardBackScene.Instance<Panel>();
			card.RectPosition = new Vector2((i * 150) + 25, 25);
			AddChild(card);
			opponentCards.Add(card);
		}
	}

	//handle drop signal from client object
	private void RenderOpponentCardDrop()
	{
		if (opponentCards.Count > 0)
		{
			opponentCards[0].QueueFree();
			opponentCards.RemoveAt(0);
		}
		Panel card = PingScene.Instance<Panel>();
		card.RectPosition = new Vector2((CardsInDropZone * 50) + 25, 425);
		AddChild(card);
		CardsInDropZone++;
	}

	//handle card being dropped
	public void Drop()
	{
		CardsInDropZone++;
		EmitSignal(nameof(CardDropped));
	}

	//signal to let client object know cards have been drawn
	[Signal]
	public delegate void CardsDrawn();

	//signal to let client object know card has been dropped
	[Signal]
	public delegate void CardDropped();
}
