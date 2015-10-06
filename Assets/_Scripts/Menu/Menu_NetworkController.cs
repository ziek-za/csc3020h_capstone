using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;

public struct LANgameInfo {
	public string ip, gameName, gameHost, serverName;
	public int ping, joinedPlayers, maxPlayers;
	public LANgameInfo(string newIp, string newGameName, string newGameHost, string newServerName,
	                   int newPing, int newJoinedPlayers, int newMaxPlayers)
	{
		ip = newIp; gameName = newGameName; gameHost = newGameHost; serverName = newServerName;
		ping = newPing; joinedPlayers = newJoinedPlayers; maxPlayers = newMaxPlayers;
	}

	public string ToString(){
		return ("\t" + gameName.PadRight(24) + gameHost.PadRight(24) + (joinedPlayers+"/"+maxPlayers).PadRight(24) + ping.ToString().PadRight(24) + ip);
	}
}

public struct OnlineGameInfo {
	public string region, gameName, gameHost, serverName;
	public int ping, joinedPlayers, maxPlayers;
	public OnlineGameInfo(string newRegion, string newGameName, string newGameHost, string newServerName,
	                   int newPing, int newJoinedPlayers, int newMaxPlayers)
	{
		region = newRegion; gameName = newGameName; gameHost = newGameHost; serverName = newServerName;
		ping = newPing; joinedPlayers = newJoinedPlayers; maxPlayers = newMaxPlayers;
	}
	
	public string ToString(){
		return ("\t" + gameName.PadRight(24) + gameHost.PadRight(24) + (joinedPlayers+"/"+maxPlayers).PadRight(24) + ping.ToString().PadRight(24) + region);
	}
	
}

public class Menu_NetworkController : MonoBehaviour {
	
	public RoomInfo[] roomsList;

	public Menu_GUIController GUIController;
	List<string> localNetworkAddresses;

	int pingReplyCount = 0;
	
	void Start () {
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

	public bool buttonClicked = false;

	public IEnumerator CheckLocalForPhoton(){
		for (int i = 0; i < localNetworkAddresses.Count; i++){
			//yield return new WaitForSeconds(2f);
			TryConnect(localNetworkAddresses[i]);
			yield return new WaitForSeconds(1f);
			CutLocalConnection(localNetworkAddresses[i]);	
		}
	}

	public void TryConnect(string ip){
		// -1 for online games
		if (!ip.Equals("-1")) {
			PhotonNetwork.PhotonServerSettings.ServerAddress = ip;
		}
		PhotonNetwork.ConnectUsingSettings("0.1");
		QualitySettings.vSyncCount = 1;
	}

	void CutLocalConnection(string ip){
		if (!buttonClicked){
			if (PhotonNetwork.connected && roomsList.Length > 0){
				Debug.LogWarning("Connection successful: " + PhotonNetwork.PhotonServerSettings.ServerAddress);
				LANgameInfo gameInfo = new LANgameInfo(ip,
					                                   roomsList[0].name.Substring(0,roomsList[0].name.LastIndexOf("|")),
					                                   roomsList[0].name.Substring(roomsList[0].name.LastIndexOf("|")+1),
					                                   roomsList[0].name,
					                                   PhotonNetwork.GetPing(),
					                                   roomsList[0].playerCount,
					                                   roomsList[0].maxPlayers);
				GUIController.listOfLANButtons.Add(gameInfo);
			} else {
				Debug.LogWarning("Disconnected from " + PhotonNetwork.PhotonServerSettings.ServerAddress);
			}
			PhotonNetwork.Disconnect();
			Debug.Log("Coroutine - "+PhotonNetwork.connectionState);
		}
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnReceivedRoomListUpdate()
	{
		roomsList = PhotonNetwork.GetRoomList();
		if (GUIController.internetGame && roomsList.Length > 0){
			GUIController.listOfOnlineButtons.Clear();

			for (int i = 0; i < roomsList.Length; i++)
			{
				OnlineGameInfo gameInfo = new OnlineGameInfo("eu",
				                                       roomsList[i].name.Substring(0,roomsList[i].name.LastIndexOf("|")),
				                                       roomsList[i].name.Substring(roomsList[i].name.LastIndexOf("|")+1),
				                                       roomsList[i].name,
				                                       PhotonNetwork.GetPing(),
				                                       roomsList[i].playerCount,
				                                       roomsList[i].maxPlayers);
				GUIController.listOfOnlineButtons.Add(gameInfo);
			}
		}
	}
}