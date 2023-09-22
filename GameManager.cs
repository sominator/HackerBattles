using Godot;
using System;

public class GameManager : Node
{
	[Export]
	public PackedScene CardScene;
	
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
			Panel card = CardScene.Instance<Panel>();
			card.RectPosition = new Vector2((i * 150) + 25, 25);
			AddChild(card);
		}
	}

	[Signal]
	public delegate void CardsDrawn();
}
