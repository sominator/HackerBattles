using Godot;
using System;
using System.Collections.Generic;

public class GameManager : Node
{
	[Export]
	public PackedScene CardScene;

	[Export]
	public PackedScene CardBackScene;

	public int CardsInDropZone { get; set; }

	private List<Panel> opponentCards = new List<Panel>();

	public override void _Ready()
	{
		CardsInDropZone = 0;
		base._Ready();
	}
	private void DrawCards()
	{
		for (int i = 0; i < 5; i++)
		{
			Panel card = CardScene.Instance<Panel>();
			card.RectPosition = new Vector2((i * 150) + 25, 825);
			AddChild(card);
		}
		EmitSignal(nameof(CardsDrawn));
	}

	private void RenderCards()
	{
		for (int i = 0; i < 5; i++)
		{
			Panel card = CardBackScene.Instance<Panel>();
			card.RectPosition = new Vector2((i * 150) + 25, 25);
			AddChild(card);
			opponentCards.Add(card);
		}
	}

	private void RenderDrop()
	{
		if (opponentCards.Count > 0)
		{
			opponentCards[0].QueueFree();
			opponentCards.RemoveAt(0);
		}
		Panel card = CardScene.Instance<Panel>();
		card.RectPosition = new Vector2((CardsInDropZone * 50) + 25, 425);
		AddChild(card);
		CardsInDropZone++;
	}

	public void Drop()
	{
		CardsInDropZone++;
		EmitSignal(nameof(CardDropped));
	}

	[Signal]
	public delegate void CardsDrawn();

	[Signal]
	public delegate void CardDropped();
}
