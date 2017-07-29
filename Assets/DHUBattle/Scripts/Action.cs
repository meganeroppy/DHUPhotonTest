using UnityEngine;
using UnityEngine.AI;

namespace DHU.Battle
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(CharacterController))]
	public class Action : MonoBehaviour
	{
		public bool Slide;              // 障害物の下をスライディングする
		public bool Vault;              // 障害物を飛び越える
		public bool DeactivateCollider; // アクション中は衝突判定を切る
		public bool MatchTarget;        // アクション中はターゲットにマッチングする

		private const float vaultMatchTargetStart = 0.40f;
		private const float vaultMatchTargetStop = 0.51f;
		private const float slideMatchTargetStart = 0.11f;
		private const float slideMatchTargetStop = 0.40f;

		private Animator animator;
		private CharacterController controller;
		private NavMeshAgent agent;

		Vector3 target = new Vector3();

		public Player Player { set; get; }

		void Start()
		{
			animator = GetComponent<Animator>();
			controller = GetComponent<CharacterController>();
			agent = GetComponent<NavMeshAgent>();
			Player = GetComponent<Player>();
		}

		void Update()
		{
			if (animator) {
				// スライディング
				if (Slide) {
					ProcessSlide();
				}

				// 飛び越え
				if (Vault) {
					ProcessVault();
				}

				// 衝突判定操作
				if (DeactivateCollider && !Player.IsAttacking) {
					controller.enabled = animator.GetFloat("Collider") > 0.5f;
					// スライディング・飛び越えモーション時に上手くキャラクター座標を移動させるための処理
					animator.applyRootMotion = !controller.enabled;
				} else {
					Player.Animator.applyRootMotion = Player.IsAttacking;
				}

				// NavMeshの衝突判定をCharacterControllerに合わせる
				if (agent != null) {
					// NavMeshの衝突判定とスライディング・飛び越えの動作を競合させないための処理
					agent.enabled = controller.enabled;
				}

				ProcessMatchTarget();
			}
		}

		void ProcessSlide()
		{
			bool slide = false;
			RaycastHit hitInfo;
			Vector3 dir = transform.TransformDirection(Vector3.forward);

			// 走行中の場合、前方の潜れる高さに障害物があるかどうか判定をかける
			if (animator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion.Run")) {
				if (Physics.Raycast(transform.position + new Vector3(0, 1.5f, 0), dir, out hitInfo, 10f)) {
					// いったんTagで障害物判定
					if (hitInfo.collider.tag == "Obstacle") {
						target = transform.position + 1.25f * hitInfo.distance * dir;
						slide = (hitInfo.distance < 6f);
					}
				}
			}

			animator.SetBool("Slide", slide);
		}

		void ProcessVault()
		{
			bool vault = false;
			RaycastHit hitInfo;
			Vector3 dir = transform.TransformDirection(Vector3.forward);

			// 走行中の場合、前方の飛び越えられる高さに障害物があるかどうか判定をかける
			if (animator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion.Run")) {
				if (Physics.Raycast(transform.position + new Vector3(0, 0.3f, 0), dir, out hitInfo, 10f)) {
					// いったんTagで障害物判定
					if (hitInfo.collider.tag == "Obstacle") {
						target = hitInfo.point;
						target.y = hitInfo.collider.bounds.center.y + 0.5f * GetComponent<Collider>().bounds.extents.y + 0.075f;

						vault = (hitInfo.distance < 4.5f && hitInfo.distance > 4f);
					}
				}
			}

			animator.SetBool("Vault", vault);
		}

		void ProcessMatchTarget()
		{
			// なんらかの遷移中の場合はなにもしない
			if (animator.IsInTransition(0)) {
				return;
			}

			AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
			// 飛び越えている場合
			if (info.IsName("Base Layer.Vault")) {
				if (MatchTarget) {
					animator.MatchTarget(
						target,
						new Quaternion(),
						AvatarTarget.LeftHand,
						new MatchTargetWeightMask(Vector3.one, 0),
						vaultMatchTargetStart,
						vaultMatchTargetStop
					); // start and stop time 
				}
			}
			// 潜っている場合
			else if (info.IsName("Base Layer.Slide")) // always do match targeting.
			{
				animator.MatchTarget(
					target,
					new Quaternion(),
					AvatarTarget.Root,
					new MatchTargetWeightMask(
						new Vector3(1, 0, 1),
						0
					),
					slideMatchTargetStart,
					slideMatchTargetStop
				);
			}
		}
	}
}