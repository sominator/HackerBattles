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

	//keep track of player cards in deck
	public List<Card> PlayerCardsInDeck = new List<Card>();

	//keep track of opponent cards in deck
	public List<Card> OpponentCardsInDeck = new List<Card>();

	//keep track of cards in play
	private List<Card> PlayerCardsInPlay = new List<Card>();

	//keep track of player sockets
	public List<Panel> PlayerSockets = new List<Panel>();

	//keep track of opponent sockets
	public List<Panel> OpponentSockets = new List<Panel>();
	
	public override void _Ready()
	{
		//store player and opponent sockets
		Node _playerSockets = GetNode("../PlayerSockets");
		for (int i = 0; i < _playerSockets.GetChildCount(); i++)
		{
			PlayerSockets.Add((Panel)_playerSockets.GetChild(i));
		}

		Node _opponentSockets = GetNode("../OpponentSockets");
		for (int i = 0; i < _opponentSockets.GetChildCount(); i++)
		{
			OpponentSockets.Add((Panel)_opponentSockets.GetChild(i));
		}

		base._Ready();
	}

	//render player deck, giving each card an ID and cardType and adding it to PlayerCardsInDeck
	private void RenderPlayerDeck(string[] playerDeck)
	{
		for (int i = 0; i < playerDeck.Length; i++)
		{
			Card card = GetScene(playerDeck[i]).Instance<Card>();
			card.RectPosition = new Vector2(1300, 825);
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
			card.RectPosition = new Vector2(1300, 25);
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
			card.StartPosition = card.RectPosition;
			PlayerCardsInPlay.Add(card);
			EmitSignal(nameof(PlayerCardMoved), card.ID, new Vector2((i * 75) + 25, 0));
		}
		PlayerCardsInDeck.RemoveRange(0, 5);
	}

	//handle card being dropped in socket and send a message to render it in the opposing socket
	public void DroppedCard(int ID, string socketName)
	{
		Panel _socket;

		switch (socketName)
		{
			case "PlayerSocket1":
				_socket = OpponentSockets[0];
				break;
			case "PlayerSocket2":
				_socket = OpponentSockets[1];
				break;
			case "PlayerSocket3":
				_socket = OpponentSockets[2];
				break;
			case "PlayerSocket4":
				_socket = OpponentSockets[3];
				break;
			case "PlayerSocket5":
				_socket = OpponentSockets[4];
				break;
			case "OpponentSocket1":
				_socket = PlayerSockets[0];
				break;
			case "OpponentSocket2":
				_socket = PlayerSockets[1];
				break;
			case "OpponentSocket3":
				_socket = PlayerSockets[2];
				break;
			case "OpponentSocket4":
				_socket = PlayerSockets[3];
				break;
			case "OpponentSocket5":
				_socket = PlayerSockets[4];
				break;
			default:
				_socket = null;
				break;
		}
		EmitSignal(nameof(PlayerCardMoved), ID, _socket.RectPosition);
	}

	//handle drop signal from client object
	private void RenderOpponentMovedCard(int ID, int posX, int posY)
	{
		Card card = OpponentCardsInDeck.Find(x => x.ID == ID);
		card.RectPosition = new Vector2(posX, posY);
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
