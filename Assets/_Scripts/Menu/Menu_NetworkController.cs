using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;

public struct LANgameInfo {
	public string ip, gameName, gameHost;
	public int ping, joinedPlayers, maxPlayers;
	public LANgameInfo(string newIp, string newGameName, string newGameHost,
	                   int newPing, int newJoinedPlayers, int newMaxPlayers)
	{
		ip = newIp; gameName = newGameName; gameHost = newGameHost;
		ping = newPing; joinedPlayers = newJoinedPlayers; maxPlayers = newMaxPlayers;
	}
	
}

public class Menu_NetworkController : MonoBehaviour {
	
	private RoomInfo[] roomsList;

	public Menu_GUIController GUIController;
	List<string> localNetworkAddresses;

	int pingReplyCount = 0;

	// Use this for initialization
	void Start () {
		//PhotonNetwork.PhotonServerSettings.ServerAddress = "192.168.1.107"; 
		//PhotonNetwork.PhotonServerSettings.ServerAddress = "0.0.0.0"; 
		//PhotonNetwork.ConnectUsingSettings("0.1");
		//QualitySettings.vSyncCount = 1;

		//Invoke("TestPing",0.1f);

		//Debug.Log (Network.player.ipAddress);
		//_MainController.hostIP = Network.player.ipAddress;
		//Debug.Log(Network.);
		//googPing = new Ping("192.168.1.108");
		//googPing.DestroyPing();
		//PingGoogle();
		//TryConnect();
		//InvokeRepeating("TryConnect",0,5);
		//Ping p = new Ping("192.168.1.110");
		//p.DestroyPing();
		//PingGoogle();
		//StartCoroutine(CheckLocalForPhoton());
		//for (int i = 100; i < 120; i++){
		//	StartCoroutine(TestPing(i));
		//}
		//TryConnect("192.168.1.112");

		//StartCoroutine(SendAsyncMethod());

		localNetworkAddresses = new List<string>();
		string ipPrefix = Network.player.ipAddress.Substring(0,Network.player.ipAddress.LastIndexOf('.') + 1);
		localNetworkAddresses.Add(Network.player.ipAddress);
		Debug.Log(Network.player.ipAddress + ": got reply");

		for (int i = 1; i < 256; i++){
			new Thread(() => 
			{
				TestPing(i,ipPrefix); 
			}).Start();
		}

		//TryConnect(Network.player.ipAddress);

	}

	void TestPing(int i, string ip){
		for (int j = 0; j < 5; j++){ //Five pings per IP
			System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
			System.Net.NetworkInformation.PingReply rep = p.Send(ip+i.ToString(),1000);
			if (rep.Status == System.Net.NetworkInformation.IPStatus.Success)
			{
				if (!localNetworkAddresses.Contains(rep.Address.ToString())){
					localNetworkAddresses.Add(rep.Address.ToString());
					Debug.Log(rep.Address + ": got reply");
				}
			}
			pingReplyCount++;
		}
	}

	/*
	void SendAsyncMethod(){
		for (int i = 100; i < 120; i++){
			System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
			ping.PingCompleted += new PingCompletedEventHandler(pingCompleted);
			//Send the pings asynchronously
			string ipPrefix = Network.player.ipAddress.Substring(0,Network.player.ipAddress.LastIndexOf('.') + 1);
			ping.SendAsync(ipPrefix+i.ToString(), 100);
			//yield return new WaitForSeconds(0.1f);
		}
	}

	public void pingCompleted(object sender, PingCompletedEventArgs e)
	{
		if (e.Reply.Status == IPStatus.Success)
		{
			Debug.Log("success " + e.Reply.Address);
		}
		else
		{
			Debug.Log("fail");
		}
	}*/
	
	public IEnumerator CheckLocalForPhoton(){
		//string ipPrefix = Network.player.ipAddress.Substring(0,Network.player.ipAddress.LastIndexOf('.') + 1);
		for (int i = 0; i < localNetworkAddresses.Count; i++){
			//Debug.Log(localNetworkAddresses[i]);
			TryConnect(localNetworkAddresses[i]);
			yield return new WaitForSeconds(3f);
			CutLocalConnection(localNetworkAddresses[i]);	
		}
	}

	public void TryConnect(string ip){
		PhotonNetwork.PhotonServerSettings.ServerAddress = ip;
		PhotonNetwork.ConnectUsingSettings("0.1");
		_MainController.hostIP = ip;
		QualitySettings.vSyncCount = 1;
	}

	void CutLocalConnection(string ip){
		if (PhotonNetwork.connected && roomsList.Length > 0){
			Debug.LogWarning("Connection successful: " + PhotonNetwork.PhotonServerSettings.ServerAddress);
			Debug.Log(roomsList[0].name+":"+PhotonNetwork.GetPing()+":"+roomsList[0].playerCount+"/"+roomsList[0].maxPlayers+":"+ip);
		} else {
			Debug.LogWarning("Disconnected from " + PhotonNetwork.PhotonServerSettings.ServerAddress);
		}
		PhotonNetwork.Disconnect();
	}
	
	// Update is called once per frame
	void Update () {
		//if (pingReplyCount >= 1275 && GUIController.JoinButton.interactable == false){
		//	GUIController.JoinButton.interactable = true;
		//}
		//if (connectedToIP){
		/*
			if (PhotonNetwork.room == null)
			{
				if (roomsList != null){
						for (int i = 0; i < roomsList.Length; i++)
						{
							if (_MainController.RoomJoined == true){
								Application.LoadLevel("Level");
								PhotonNetwork.JoinRoom(roomsList[i].name);
								_MainController.ImportMapObject("2");
								_MainController.RoomJoined = false;
							}
						}
					}
			}*/
		//}
	}

	void OnReceivedRoomListUpdate()
	{
		roomsList = PhotonNetwork.GetRoomList();
	}
}