using UnityEngine;

public class HoverOrientation : MonoBehaviour
{
	public float Turn;
	public float MaxTorque;
	public float BrakingTorque;
	public float RollOnTurns;
	public float RollOnTurnsTorque;
	public float RollCompensationTorque;
	public float PitchCompensationTorque;
	public HoverEngine[] HoverEngines;
}
