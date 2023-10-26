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

	//keep track of player cards in deck
	public List<Card> PlayerCardsInDeck = new List<Card>();

	//keep track of opponent cards in deck
	public List<Card> OpponentCardsInDeck = new List<Card>();

	//keep track of cards in play
	private List<Card> PlayerCardsInPlay = new List<Card>();

	//store center of viewscreen as origin point
	private Vector2 _origin = new Vector2(960, 440);
	
	public override void _Ready()
	{
		CardsInDropZone = 0;

		base._Ready();
	}

	//render player deck, giving each card an ID and cardType and adding it to PlayerCardsInDeck
	private void RenderPlayerDeck(string[] playerDeck)
	{
		for (int i = 0; i < playerDeck.Length; i++)
		{
			Card card = GetScene(playerDeck[i]).Instance<Card>();
			card.RectPosition = new Vector2(_origin.x + 340, _origin.y + 385);
			card.ID = i;
			card.CardType = "PlayerCard";
			card.IsDraggable = true;
			PlayerCardsInDeck.Add(card);
			AddChild(card);
		}
	}

	//render opponent deck, giving each card an ID and cardType
	private void RenderOpponentDeck(string[] opponentDeck)
	{
		for (int i = 0; i < opponentDeck.Length; i++)
		{
			Card card = GetScene(opponentDeck[i]).Instance<Card>();
			card.RectPosition = new Vector2(_origin.x + 340, _origin.y - 415);
			card.ID = i;
			card.CardType = "OpponentCard";
			card.IsDraggable = false;
			OpponentCardsInDeck.Add(card);
			AddChild(card);
		}
	}

	//deal five cards from player deck, adding each card to PlayerCardsInPlay and removing it from PlayerCardsInDeck
	private void DealPlayerCards()
	{
		for (int i = 0; i < 5; i++)
		{
			if (PlayerCardsInDeck[i] == null) return;

			Card card = PlayerCardsInDeck[i];
			card.RectPosition = new Vector2((i * 75) + 25, 825);
			PlayerCardsInPlay.Add(card);
			EmitSignal(nameof(PlayerCardMoved), card.ID, card.RectPosition);
		}
		PlayerCardsInDeck.RemoveRange(0, 5);
	}

	//handle card being dropped
	public void MoveCard()
	{
		EmitSignal(nameof(PlayerCardMoved));
	}

	//handle drop signal from client object
	private void RenderOpponentMovedCard(int ID, int posX, int posY)
	{
		Card card = OpponentCardsInDeck.Find(x => x.ID == ID);
		card.RectPosition = new Vector2(posX + 50, posY + 50);
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

	//signal to let client object know card has been dropped
	[Signal]
	public delegate void PlayerCardMoved(int ID, Vector2 position);
}
