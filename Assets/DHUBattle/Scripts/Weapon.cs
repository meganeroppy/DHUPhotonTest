using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DHU.Battle
{
	public class Weapon : MonoBehaviour
	{

		public Player Player;
		public Rigidbody Rigidbody;
		public Collider Collider;
		public GameObject HitEffect;

		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{
		}

		public void SetWeaponEnable(bool enable)
		{
			Collider.enabled = enable;
		}

		void OnTriggerEnter(Collider col)
		{
			/*
			var npc = col.GetComponent<NPC>();

			if (npc != null && npc.IsArrive) {
				npc.TakeDamage();

				var dir = npc.transform.position - Player.transform.position;
				dir = new Vector3(dir.x, 0, dir.z);
				StartCoroutine(npc.KnockBack(dir, 10f));

				var effect = Instantiate(HitEffect) as GameObject;
				effect.transform.position = col.ClosestPoint(transform.position);
				Destroy(effect, 3f);
			}
			*/
		}
	}
}