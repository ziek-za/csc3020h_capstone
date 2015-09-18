using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;

public class Menu_NetworkController : MonoBehaviour {
	
	private RoomInfo[] roomsList;
	bool connectedToIP = false;

	public Menu_GUIController GUIController;
	List<string> localNetworkAddresses;

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

		for (int i = 1; i < 255; i++){
			new Thread(() => 
			{
				TestPing(i,ipPrefix); 
			}).Start();
		}

		TryConnect(Network.player.ipAddress);

	}

	void TestPing(int i, string ip){
		System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
		System.Net.NetworkInformation.PingReply rep = p.Send(ip+i.ToString(),1000);
		if (rep.Status == System.Net.NetworkInformation.IPStatus.Success)
		{
			Debug.Log(rep.Address + ": got reply");
			localNetworkAddresses.Add(rep.Address.ToString());
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
	
	IEnumerator CheckLocalForPhoton(){
		string ipPrefix = Network.player.ipAddress.Substring(0,Network.player.ipAddress.LastIndexOf('.') + 1);
		for (int i = 100; i < 120; i++){
			TryConnect((ipPrefix+i.ToString()));
			yield return new WaitForSeconds(0.5f);
			CutLocalConnection();
		}
		yield return 0;
	}

	public void TryConnect(string ip){
		PhotonNetwork.PhotonServerSettings.ServerAddress = ip;
		PhotonNetwork.ConnectUsingSettings("0.1");
		_MainController.hostIP = ip;
		QualitySettings.vSyncCount = 1;
	}

	void CutLocalConnection(){
		if (PhotonNetwork.connecting){
			PhotonNetwork.Disconnect();
			Debug.LogWarning("Disconnected from " + PhotonNetwork.PhotonServerSettings.ServerAddress);
		} else if (PhotonNetwork.connected){
			PhotonNetwork.Disconnect();
			Debug.LogWarning("Connection successful: " + PhotonNetwork.PhotonServerSettings.ServerAddress);
		}
	}
	
	// Update is called once per frame
	void Update () {
		//if (connectedToIP){
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
			}
		//}
	}

	void OnReceivedRoomListUpdate()
	{
		roomsList = PhotonNetwork.GetRoomList();
		GUIController.HostButton.interactable = true;
		if (roomsList.Length > 0) {
			GUIController.JoinButton.interactable = true;
		}
		//CancelInvoke("TryConnect");
	}
}