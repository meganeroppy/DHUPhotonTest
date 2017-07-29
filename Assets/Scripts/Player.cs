using UnityEngine;

public class Player : Photon.MonoBehaviour
{
	public PhotonManager PhotonManager { get; set; }

	private Vector3 targetPosition;

	private Vector3 targetRotation;

	private Vector3 targetScale;

	bool isFirstTime;

	private Animator anim;

	void Awake()
	{
		anim = GetComponent<Animator> ();
	}

	// Use this for initialization
	void Start()
	{
		PhotonPlayer player = PhotonPlayer.Find(photonView.ownerId);
		gameObject.name += "(" + player.NickName + ")";

		if (PhotonManager == null) {
			PhotonManager = FindObjectOfType<PhotonManager>();
		}
		transform.SetParent(PhotonManager.PlayersRoot);

		targetPosition = transform.position;
		targetRotation = transform.rotation.eulerAngles;
		targetScale = transform.localScale;

		if (photonView.isMine) {
			photonView.RPC("RequestUpdateTransform", PhotonTargets.Others);
		}

		targetPosition = transform.position;
		targetRotation = transform.rotation.eulerAngles;
		targetScale = transform.localScale;

		isFirstTime = true;

		// じぶん以外からはスクリプトを削除
		if (!photonView.isMine) {
			var p = GetComponent<DHU.Battle.Player> ();
			if (p != null) {
				Destroy (p);
			}
		}

	}

	// Update is called once per frame
	void Update()
	{
		if (!photonView.isOwnerActive && gameObject != null) {
			PhotonNetwork.Destroy(gameObject);
		}

		if (!photonView.isMine) {
			
			if (Vector3.Distance(transform.position, targetPosition) > 0.1f) {
				transform.position = Vector3.Lerp(transform.position, targetPosition, 0.3f);
			} else {
				transform.position = targetPosition;
			}

			if (Vector3.Distance(transform.rotation.eulerAngles, targetRotation) > 0.1f) {
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRotation), 0.3f);
			} else {
				transform.rotation = Quaternion.Euler(targetRotation);
			}

			if (Vector3.Distance(transform.localScale, targetScale) > 0.1f) {
				transform.localScale = Vector3.Lerp(transform.localScale, targetScale, 0.3f);
			} else {
				transform.localScale = targetScale;
			}

		}
	}

	/// <summary>
	/// UDPで送信しているため確実に届く保証はない
	/// １フレームで元に戻ってしまうアニメータのトリガー系は確実に届けるため別にRPCで送信する
	/// 
	/// </summary>
	/// <param name="stream">Stream.</param>
	/// <param name="info">Info.</param>
	void OnPhotonSerializeView( PhotonStream stream, PhotonMessageInfo info )
	{
		if (stream.isWriting) 
		{
			// じぶんの時は送る
			stream.SendNext (transform.position);
			stream.SendNext (transform.rotation.eulerAngles);
			stream.SendNext (transform.localScale);

			// アニメータのパラメータを送信
			stream.SendNext (anim.GetFloat ("Speed"));
			stream.SendNext (anim.GetFloat ("AngularSpeed"));
			stream.SendNext (anim.GetFloat ("Direction"));
			stream.SendNext (anim.GetBool ("Slide"));
			stream.SendNext (anim.GetFloat ("Wounded"));
			stream.SendNext (anim.GetBool ("Vault"));
			stream.SendNext (anim.GetBool ("IsDead"));
			stream.SendNext (anim.GetFloat ("Collider"));
			stream.SendNext (anim.GetBool ("HoldLog"));
			stream.SendNext (anim.GetBool ("Shoot"));
			stream.SendNext (anim.GetBool ("IsAttacking"));
			}
		else
		{
			// じぶん以外の時は取得
			targetPosition = (Vector3)stream.ReceiveNext ();
			targetRotation = (Vector3)stream.ReceiveNext ();
			targetScale = (Vector3)stream.ReceiveNext ();

			// アニメータのパラメータを取得
			anim.SetFloat("Speed", (float)stream.ReceiveNext());
			anim.SetFloat("AngularSpeed", (float)stream.ReceiveNext());
			anim.SetFloat("Direction", (float)stream.ReceiveNext());
			anim.SetBool("Slide", (bool)stream.ReceiveNext());
			anim.SetFloat("Wounded", (float)stream.ReceiveNext());
			anim.SetBool("Vault", (bool)stream.ReceiveNext());
			anim.SetBool("IsDead", (bool)stream.ReceiveNext());
			anim.SetFloat("Collider", (float)stream.ReceiveNext());
			anim.SetBool("HoldLog", (bool)stream.ReceiveNext());
			anim.SetBool("Shoot", (bool)stream.ReceiveNext());
			anim.SetBool("IsAttacking", (bool)stream.ReceiveNext());

			// 初回のみ
			if (isFirstTime) 
			{
				transform.position = targetPosition;
				transform.rotation = Quaternion.Euler (targetRotation);
				transform.localScale = targetScale;

				isFirstTime = false;
			}
		}
	}

	[PunRPC]
	public void GetTriggerAttack()
	{
		anim.SetTrigger ("Attack");
	}

	public void TriggerAttack()
	{
		photonView.RPC ("GetTriggerAttack", PhotonTargets.Others);
		anim.SetTrigger ("Attack");
	}
}