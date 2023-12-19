using Godot;
using System;
using System.Diagnostics.PerformanceData;

public class UIManager : Node
{
	//store instructions and player disconnected scenes
	[Export]
	public PackedScene InstructionsScene;

	[Export]
	public PackedScene DisconnectScene;
	
	Panel _instructionPanel = null;
	Panel _disconnectedPanel = null;

	//keep track of UI spinboxes and option buttons
	SpinBox _spinBoxOpponentBP;
	SpinBox _spinBoxOpponentVariables;

	OptionButton _optionOpponentSpecialization;
	OptionButton _optionObjective;

	//store themes and panels
	Theme _lightTheme = (Theme)GD.Load("res://theme/sci-fi-theme-light.tres");
	Theme _darkTheme = (Theme)GD.Load("res://theme/sci-fi-theme-dark.tres");
	Panel _waitingPanel;
	Panel _uiPanel;
	Panel _playerSocketsPanel;
	Panel _opponentSocketsPanel;
	Panel _cardContainerPanel;

	//keep track of room id line edit
	LineEdit _roomIdLineEdit;

	public override void _Ready()
	{
		//store UI spinboxes, option buttons, and panels
		_spinBoxOpponentBP = (SpinBox)GetNode("../UI/SpinBoxOpponentBP");
		_spinBoxOpponentVariables = (SpinBox)GetNode("../UI/SpinBoxOpponentVariables");
		_optionOpponentSpecialization = (OptionButton)GetNode("../UI/OptionOpponentSpecialization");
		_optionObjective = (OptionButton)GetNode("../UI/OptionObjective");
		_waitingPanel = (Panel)GetNode("../PopupWaiting");
		_uiPanel = (Panel)GetNode("../UI");
		_playerSocketsPanel = (Panel)GetNode("../UI/PlayerSockets");
		_opponentSocketsPanel = (Panel)GetNode("../UI/OpponentSockets");
		_cardContainerPanel = (Panel)GetNode("../GameManager/CardContainer");

		//store room id line edit
		_roomIdLineEdit = (LineEdit)GetNode("../UI/LineEditRoomId");
	}

	public override void _Process(float delta)
	{
		//handle rendering instruction panel based on escape key
		if (Input.IsActionJustPressed("escape"))
		{
			if (_instructionPanel == null)
			{
				_instructionPanel = InstructionsScene.Instance<Panel>();
				GetNode("../GameManager/CardContainer").AddChild(_instructionPanel);
			}
			else
			{
				_instructionPanel.QueueFree();
				_instructionPanel = null;
			}
			
		}

		base._Process(delta);
	}

	//handle update room id signal from client object
	private void UpdateRoomId(string roomId)
	{
		//update game room id
		_roomIdLineEdit.Text = roomId;

		//update room id for waiting panel
		LineEdit _waitingPanelRoomId = (LineEdit)_waitingPanel.GetNode("LineEditRoomId");
		_waitingPanelRoomId.Text = roomId;
	}

	//handle clients connected signal from client object
	private void OnClientsConnected()
	{
		//destroy waiting panel
		_waitingPanel.QueueFree();
	}

	//handle client disconnected signal from client object
	private void OnClientDisconnected()
	{
		_disconnectedPanel = DisconnectScene.Instance<Panel>();
		GetNode("../GameManager/CardContainer").AddChild(_disconnectedPanel);
	}

	//handle update opponent BP signal from client object
	private void UpdateOpponentBP(float value)
	{
		_spinBoxOpponentBP.Value = value;
	}

	//handle update opponent variables signal from client object
	private void UpdateOpponentVariables(float value)
	{
		_spinBoxOpponentVariables.Value = value;
	}

	//handle update objective signal from client object
	private void UpdateObjective(int index)
	{
		_optionObjective.Select(index);
	}

	//handle update opponent specialization signal from client object
	private void UpdateOpponentSpecialization(int index)
	{
		_optionOpponentSpecialization.Select(index);
	}


	//let client object know about player BP spinbox change
	private void OnSpinBoxPlayerBPValueChanged(float value)
	{
		EmitSignal(nameof(PlayerBPChanged), value);
	}

	//let client object know about player variables spinbox change
	private void OnSpinBoxPlayerVariablesValueChanged(float value)
	{
		EmitSignal(nameof(PlayerVariablesChanged), value);
	}

	//let client object know about objective option button change
	private void OnOptionObjectiveItemSelected(int index)
	{
		EmitSignal(nameof(ObjectiveChanged), index);
	}

	//let client object know about player specialization option button change
	private void OnOptionPlayerSpecializationItemSelected(int index)
	{
		EmitSignal(nameof(PlayerSpecializationChanged), index);
	}

	//handle toggling of light and dark modes
	private void OnModeToggled(bool button_pressed)
	{
		if (button_pressed)
		{
			//toggle panels to dark mode
			_uiPanel.Theme = _darkTheme;
			_playerSocketsPanel.Theme = _darkTheme;
			_opponentSocketsPanel.Theme = _darkTheme;
			_cardContainerPanel.Theme = _darkTheme;
		}
		else
		{
			//toggle panels to light mode
			_uiPanel.Theme = _lightTheme;
			_playerSocketsPanel.Theme = _darkTheme;
			_opponentSocketsPanel.Theme = _darkTheme;
			_cardContainerPanel.Theme = _lightTheme;
		}
	}

	[Signal]
	public delegate void PlayerBPChanged(float value);

	[Signal]
	public delegate void PlayerVariablesChanged(float value);

	[Signal]
	public delegate void ObjectiveChanged(int index);

	[Signal]
	public delegate void PlayerSpecializationChanged(int index);

}
