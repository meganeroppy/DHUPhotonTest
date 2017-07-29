using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace DHU.Battle
{
	[RequireComponent(typeof(Animator))]
	public class Player : MonoBehaviour
	{
		public Animator Animator;
		public NavMeshAgent Agent;
		public Camera TargetCamera;

		public float BaseSpeed = 6f;
		public float BaseDirection = 180f;

		public Weapon RightFootWeapon;

		// キャラクターモーションを管理するためのクラス
		private Locomotion locomotion;

		public bool IsAttacking {
			get {
				return
					Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack01") ||
					Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack02") ||
					Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack03");
			}
		}

		public bool IsDying {
			get {
				return
					Animator.GetCurrentAnimatorStateInfo(0).IsName("Dying") ||
					Animator.GetCurrentAnimatorStateInfo(0).IsName("Death") ||
					Animator.GetCurrentAnimatorStateInfo(0).IsName("Reviving");
			}
		}

		public int Hp;
		public const int MAX_HP = 3;

		public bool IsArrive {
			get {
				return Hp > 0;
			}
		}

		public bool TrigerAttack {
			get; set;
		}

		public Vector2 InputMovement {
			get; set;
		}

		void Start()
		{
			if (Animator == null) {
				Animator = GetComponent<Animator>();
			}
			Animator.logWarnings = false;

			if (Agent == null) {
				Agent = GetComponent<NavMeshAgent>();
			}

			if (TargetCamera == null) {
				TargetCamera = Camera.main;
			}

			locomotion = new Locomotion(Animator);

			JoystickToWorld.Player = transform;
			JoystickToWorld.TargetCamera = TargetCamera;

			Hp = MAX_HP;

			InputMovement = Vector2.zero;
		}

		void Update()
		{
			if (Animator && TargetCamera != null && IsArrive && !IsDying) {
				var isAttacking = IsAttacking;

				Animator.SetBool("IsAttacking", isAttacking);

				// 攻撃の判定
				if (Input.GetButtonDown("Jump") || TrigerAttack) {
					//Animator.SetTrigger("Jump");
					SendMessage("TriggerAttack");
					if (Animator.GetNextAnimatorStateInfo(0).IsName("Attack01")) {
					//	LookNearstEnemy();
					}
				}

				float speed = 0;
				float direction = 0;

				if (!isAttacking) {
					SetWeaponEnable(0);

					InputMovement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

					// JoystickToWorldから入力値を速度と方向に変換する処理
					JoystickToWorld.ComputeSpeedDirection(InputMovement, transform, ref speed, ref direction);

					speed *= BaseSpeed;
					direction *= BaseDirection;
				}

				Move(speed, direction);

				// Locomotionクラスからキャラクターモーションを流すための処理
				locomotion.Do(speed, direction);
			}

			TrigerAttack = false;
			InputMovement = Vector2.zero;
		}

		private void Move(float speed, float direction)
		{
			if (speed > 0 && Agent.enabled) {
				// キャラクターの向きを変える処理
				Agent.transform.Rotate(Vector3.up * direction);
				// NavMeshAgentを使ってキャラクターを移動させる処理
				Agent.velocity = Agent.transform.forward * speed;// * speed / (speed + Mathf.Abs(direction));
			}
		}

		public void SetWeaponEnable(int enable)
		{
			RightFootWeapon.SetWeaponEnable(enable > 0);
		}

		/*
		// 一番近いNPCを向く
		public void LookNearstEnemy()
		{
			NPC npc = null;
			float distance = Mathf.Infinity;

			foreach (var tmpNpc in GameManager.Instance.ArrivalNpcList) {
				var tmpDistance = Vector3.Distance(transform.position, tmpNpc.transform.position);
				if (tmpDistance < distance) {
					npc = tmpNpc;
					distance = tmpDistance;
				}
			}

			if (npc != null && distance < 3f) {
				transform.rotation = Quaternion.LookRotation(
					npc.transform.position - transform.position,
					Vector3.up
				);
			}
		}
		*/

		public void TakeDamage(int damage = 1)
		{
			if (!IsArrive) {
				return;
			}

			Hp = Mathf.Max(Hp - damage, 0);

			if (!IsArrive) {
				StartCoroutine(DyingRoutine());
			}

			Handheld.Vibrate();
		}

		public IEnumerator DyingRoutine(float arrivalTime = 3f)
		{
			Animator.SetBool("IsDead", true);
			Animator.SetTrigger("Dead");

			yield return new WaitForSeconds(arrivalTime);

			Animator.SetBool("IsDead", false);

			Hp = MAX_HP;
		}
	}
}
