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
			AddChild(card);
		}
	}

}
