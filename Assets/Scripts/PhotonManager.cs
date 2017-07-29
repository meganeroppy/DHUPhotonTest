using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonManager : Photon.MonoBehaviour {

	public string PlayerName = "";
	public byte Version = 1;
	public string LobbyName = "lobby-test";
	public string RoomName = "room-test";

	public bool IsAutoConnectPhoton = false;

	public GameObject PlayerPrefab;
	public Transform PlayersRoot;

	private bool isJoinedLobby = false;
	private bool isJoinedRoom = false;

	private TypedLobby typedLobby;

	// Use this for initialization
	void Start () {
		PhotonNetwork.autoJoinLobby = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!IsAutoConnectPhoton && PhotonNetwork.connectionState == ConnectionState.Disconnected) {
			PhotonNetwork.ConnectUsingSettings(Version + ".0");
		}
	}

	/// <summary>
	/// Photonへの接続
	/// </summary>
	public void OnConnectedToPhoton()
	{
		PhotonNetwork.playerName = PlayerName;

		Debug.Log("Connected to Photon.");
	}

	/// <summary>
	/// ロビーへの接続完了（マスターとして接続したとき）
	/// </summary>
	public void OnConnectedToMaster()
	{
		Debug.Log("Joind Lobby as Master");
		isJoinedLobby = true;

		// ロビーへ接続
		typedLobby = new TypedLobby();
		typedLobby.Name = LobbyName;
		PhotonNetwork.JoinLobby(typedLobby);
	}

	/// <summary>
	/// ロビーへの接続完了
	/// </summary>
	public void OnJoinedLobby()
	{
		Debug.Log("Joined Lobby : " + PhotonNetwork.lobby.Name + " (is master : " + PhotonNetwork.isMasterClient + ")");
		isJoinedLobby = true;

		// ルームへの接続
		RoomOptions options = new RoomOptions();
		options.IsVisible = true;
		options.MaxPlayers = 20;
		options.IsOpen = true;
		PhotonNetwork.JoinOrCreateRoom(RoomName, options, null);
	}

	public void OnCreatedRoom()
	{
		Debug.Log("Room Created :" + PhotonNetwork.room.Name);
	}

	public void OnJoinedRoom()
	{
		Debug.Log("Joined Room :" + PhotonNetwork.room.Name);

		var rand = Random.insideUnitCircle * 3;
		var position = new Vector3 (rand.x, 0, rand.y);

		PhotonNetwork.Instantiate(PlayerPrefab.name, position, Quaternion.identity, 0);
	}

	public void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		Debug.Log("Other Person Joined : " + newPlayer.ID + "(" + newPlayer.NickName + ")");
	}

	public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
	{
		Debug.Log("Other Person Leaved : " + otherPlayer.ID + "(" + otherPlayer.NickName + ")");
	}
}
