using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public class GameManager : Node
{
    #region CardScenes
    //expose cards and card back in editor
    [Export]
	public PackedScene BooleanScene;
	[Export]
	public PackedScene DefragScene;
	[Export]
	public PackedScene DoubleScene;
	[Export]
	public PackedScene EchoScene;
	[Export]
	public PackedScene FirewallScene;
	[Export]
	public PackedScene FloatScene;
	[Export]
	public PackedScene GlitchScene;
	[Export]
	public PackedScene HandshakeScene;
	[Export]
	public PackedScene HostScene;
	[Export]
	public PackedScene PingScene;
	[Export]
	public PackedScene ProbeScene;
	[Export]
	public PackedScene ReInitializeScene;
	[Export]
	public PackedScene ScrapeScene;
	[Export]
	public PackedScene SpliceScene;
	[Export]
	public PackedScene TurnKeyScene;
	[Export]
	public PackedScene CardBackScene;
    #endregion

    //keep track of number of cards in dropzone
    public int CardsInDropZone { get; set; }

	//keep track of number of opponent card backs to render
	private List<Panel> opponentCards = new List<Panel>();

	
	public override void _Ready()
	{
		CardsInDropZone = 0;

		base._Ready();
	}

	//deal cards based on shuffle array received from client
	public void DealCards(string[] data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			Panel card = GetScene(data[i]).Instance<Panel>();
			card.RectPosition = new Vector2((i * 75) + 25, 825);
			AddChild(card);
		}
	}

	//determine PackedScene to instance based on string name
	private PackedScene GetScene(string cardName)
	{
		switch (cardName)
		{
			case "boolean":
				return BooleanScene;
			case "defrag":
				return DefragScene;
			case "double":
				return DoubleScene;
			case "echo":
				return EchoScene;
			case "firewall":
				return FirewallScene;
			case "float":
				return FloatScene;
			case "glitch":
				return GlitchScene;
			case "handshake":
				return HandshakeScene;
			case "host":
				return HostScene;
			case "ping":
				return PingScene;
			case "probe":
				return ProbeScene;
			case "reInitialize":
				return ReInitializeScene;
			case "scrape":
				return ScrapeScene;
			case "splice":
				return SpliceScene;
			case "turnkey":
				return TurnKeyScene;
			default: return null;
		}
			
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
