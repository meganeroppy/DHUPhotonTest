using UnityEngine;

namespace DHU.Battle
{
	/// <summary>
	/// ジョイスティックの入力をUnityのWorld座標に変換するためのクラス
	/// </summary>
	public class JoystickToWorld
	{
		public static Transform Player;

		private static Camera targetCamera;
		public static Camera TargetCamera {
			get {
				if (targetCamera == null) {
					targetCamera = Camera.main;
				}
				return targetCamera;
			}
			set {
				targetCamera = value;
			}
		}

		private static Quaternion baseRotation;

		private static float prevVertical;
		private static float minusVerticalInputTime;

		/// <summary>
		/// ジョイスティックの入力をUnityのWorld座標に変換する
		/// </summary>
		/// <returns>The joystick to world space.</returns>
		public static Vector3 ConvertJoystickToWorldSpace()
		{
			Vector3 direction;

			float horizontal = Input.GetAxis("Horizontal");
			float vertical = Input.GetAxis("Vertical");

			// 下矢印キーを押している際に、振り向き動作をループさせないための処理
			if (Mathf.Abs(vertical) >= 1f) {
				if (Time.time - minusVerticalInputTime > 1f) {
					vertical = Mathf.Abs(vertical);
				}
			}

			Vector3 stickDirection;
			if (vertical >= 0) {
				// 通常動作時
				stickDirection = new Vector3(horizontal, 0, vertical);
				direction = TargetCamera.transform.rotation * stickDirection;
			} else {
				// 振り向き動作時
				stickDirection = new Vector3(-horizontal, 0, vertical);
				direction = baseRotation * stickDirection;
			}

			direction.y = 0; // Z方向は無視する
			direction.Normalize();

			if (prevVertical >= 0) {
				// 振り向き動作時のためのパラメータ設定
				baseRotation = Player.transform.rotation;
				minusVerticalInputTime = Time.time;
			}

			prevVertical = Input.GetAxis("Vertical");

			return direction;
		}

		public static Vector3 ConvertUIInputToWorldSpace(Vector2 movement)
		{
			Vector3 direction;

			float horizontal = movement.x;
			float vertical = movement.y;

			Vector3 stickDirection = new Vector3(horizontal, 0, vertical);
			direction = TargetCamera.transform.rotation * stickDirection;

			direction.y = 0; // Z方向は無視する
			direction.Normalize();

			return direction;
		}

		/// <summary>
		/// 速度と向きを求める
		/// </summary>
		/// <param name="movement">Movement.</param>
		/// <param name="root">Root.</param>
		/// <param name="speed">Speed.</param>
		/// <param name="direction">Direction.</param>
		public static void ComputeSpeedDirection(Vector2 movement, Transform root, ref float speed, ref float direction)
		{
			Vector3 worldDirection = Vector3.zero;

			if (movement.sqrMagnitude > 0) {
				worldDirection = ConvertUIInputToWorldSpace(movement);
			} else {
				worldDirection = ConvertJoystickToWorldSpace();
			}

			speed = Mathf.Clamp01(worldDirection.magnitude);
			if (speed > 0.01f) {
				Vector3 axis = Vector3.Cross(root.forward, worldDirection);
				direction = Vector3.Angle(root.forward, worldDirection) / 180.0f * (axis.y < 0 ? -1f : 1f);
			} else {
				direction = 0.0f;
			}
		}
	}
}