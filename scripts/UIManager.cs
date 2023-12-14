using Godot;
using System;

public class UIManager : Node
{
	//keep track of UI spinboxes and option buttons
	SpinBox _spinBoxOpponentBP;
	SpinBox _spinBoxOpponentVariables;

	OptionButton _optionOpponentSpecialization;
	OptionButton _optionObjective;

	public override void _Ready()
	{
		//store UI spinboxes and option buttons
		_spinBoxOpponentBP = (SpinBox)GetNode("../UI/SpinBoxOpponentBP");
		_spinBoxOpponentVariables = (SpinBox)GetNode("../UI/SpinBoxOpponentVariables");
		_optionOpponentSpecialization = (OptionButton)GetNode("../UI/OptionOpponentSpecialization");
		_optionObjective = (OptionButton)GetNode("../UI/OptionObjective");
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

	[Signal]
	public delegate void PlayerBPChanged(float value);

	[Signal]
	public delegate void PlayerVariablesChanged(float value);

	[Signal]
	public delegate void ObjectiveChanged(int index);

	[Signal]
	public delegate void PlayerSpecializationChanged(int index);

}
