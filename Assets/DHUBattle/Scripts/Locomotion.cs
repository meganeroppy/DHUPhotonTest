using UnityEngine;

namespace DHU.Battle
{
	/// <summary>
	/// キャラクターのモーションを管理するためのクラス
	/// </summary>
	public class Locomotion
	{
		private Animator animator;

		private int speedId = 0;
		private int agularSpeedId = 0;
		private int directionId = 0;

		private float baseSpeedDampTime = 0.1f;
		private float baseAnguarSpeedDampTime = 0.25f;
		private float baseDirectionResponseTime = 0.2f;

		private Vector3 turnStartDirection;

		public Locomotion(Animator animator)
		{
			this.animator = animator;

			speedId = Animator.StringToHash("Speed");
			agularSpeedId = Animator.StringToHash("AngularSpeed");
			directionId = Animator.StringToHash("Direction");
		}

		public void Do(float speed, float direction)
		{
			AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

			bool inTransition = animator.IsInTransition(0);
			bool inIdle = state.IsName("Locomotion.Idle");
			bool inTurn = state.IsName("Locomotion.TurnOnSpot");
			bool inRun = state.IsName("Locomotion.Run");

			float angularSpeed = direction / baseDirectionResponseTime;

			float speedDampTime = inIdle ? 0 : baseSpeedDampTime;
			float angularSpeedDampTime = inRun || inTransition ? baseAnguarSpeedDampTime : 0;
			float directionDampTime = inTurn || inTransition ? 1000000 : 0;

			animator.SetFloat(speedId, speed, speedDampTime, Time.deltaTime);
			animator.SetFloat(agularSpeedId, angularSpeed, angularSpeedDampTime, Time.deltaTime);
			animator.SetFloat(directionId, direction, directionDampTime, Time.deltaTime);
		}
	}
}