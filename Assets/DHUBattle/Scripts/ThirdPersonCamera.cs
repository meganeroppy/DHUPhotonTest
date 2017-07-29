//
// Unityちゃん用の三人称カメラ
// 
// 2013/06/07 N.Kobyasahi
//
using UnityEngine;

namespace unitychan
{
	public class ThirdPersonCamera : MonoBehaviour
	{
		public float smooth = 3f;       // カメラモーションのスムーズ化用変数

		[SerializeField]
		private Transform standardPos;	// the usual position for the camera, specified by a transform in the game

		// スムーズに繋がない時（クイック切り替え）用のブーリアンフラグ
		bool bQuickSwitch = false; // Change Camera Position Quickly

		void Start()
		{
		}

		void Update()
		{
			// 各参照の初期化
			// UnityEditor上でこのクラスが正常に動作するように設定をする
			if (standardPos == null) {
				foreach (var p in FindObjectsOfType<PhotonView>()) {
					if (p.isMine) {
						foreach (var t in p.GetComponentsInChildren<Transform>()) {
							if (t.name == "CamPos") {
								standardPos = t;
								break;
							}
						}
						break;
					}
				}

				if (standardPos != null) {
					//カメラをスタートする
					transform.position = standardPos.position;
					transform.forward = standardPos.forward;
				}
			}
		}

		void FixedUpdate()  // このカメラ切り替えはFixedUpdate()内でないと正常に動かない
		{
			if (standardPos != null) {
				setCameraPositionNormalView();
			}
		}

		void setCameraPositionNormalView()
		{
			if (bQuickSwitch == false) {
				// the camera to standard position and direction
				transform.position = Vector3.Lerp(transform.position, standardPos.position, Time.fixedDeltaTime * smooth);
				transform.forward = Vector3.Lerp(transform.forward, standardPos.forward, Time.fixedDeltaTime * smooth);
			} else {
				// the camera to standard position and direction / Quick Change
				transform.position = standardPos.position;
				transform.forward = standardPos.forward;
				bQuickSwitch = false;
			}
		}
	}
}
