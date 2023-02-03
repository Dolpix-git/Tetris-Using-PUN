using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Realtime;
using UnityEditor.Rendering;
using UnityEditor;
using System.Net.NetworkInformation;

public class LobbyManager : MonoBehaviourPunCallbacks{
    public TMP_InputField roomInputField;
    public GameObject lobbyPanel;
    public GameObject roomPanel;
    public TextMeshProUGUI roomName;

    public RoomItem roomItemPrefab;
    List<RoomItem> roomItems = new List<RoomItem>();
    public Transform contentObject;

    public float timeBetweenUpdates = 1.5f;
    float nextUpdateTime;

    public List<PlayerItem> playerItemsList = new List<PlayerItem>();
    public PlayerItem playerItemPrefab;
    public Transform playerItemParent;

    public GameObject playButton;

    private void Start(){
        PhotonNetwork.JoinLobby();
    }

    private void Update(){
        if (PhotonNetwork.IsMasterClient){
            playButton.SetActive(true);
        }
        else{
            playButton.SetActive(false);
        }
    }

    public void OnClickCreate(){
        if (roomInputField.text.Length >= 1){
            PhotonNetwork.CreateRoom(roomInputField.text, new RoomOptions() { MaxPlayers = 2} );
        }
    }

    public override void OnJoinedRoom(){
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList){
        UpdateRoomList(roomList);
    }

    void UpdateRoomList(List<RoomInfo> list){
        foreach (RoomItem items in roomItems) {
            Destroy(items.gameObject);
        }

        roomItems.Clear();

        foreach (RoomInfo room in list){
            RoomItem newRoom = Instantiate(roomItemPrefab, contentObject);
            newRoom.SetRoomName(room.Name);
            roomItems.Add(newRoom);
        }
    }

    public void JoinRoom(string roomName){
        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnClickLeaveRoom(){
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom(){
        lobbyPanel.SetActive(true);
        roomPanel.SetActive(false);
    }

    public override void OnConnectedToMaster(){
        PhotonNetwork.JoinLobby();
    }

    void UpdatePlayerList(){
        foreach (PlayerItem item in playerItemsList){
            Destroy(item.gameObject);
        }
        playerItemsList.Clear();

        if (PhotonNetwork.CurrentRoom == null){
            return;
        }

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players) {
            PlayerItem newPlayerItem = Instantiate(playerItemPrefab, playerItemParent);
            newPlayerItem.SetPlayerInfo(player.Value);
            playerItemsList.Add(newPlayerItem);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer){
        UpdatePlayerList();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer){
        UpdatePlayerList();
    }

    public void OnClickPlayButton(){
        PhotonNetwork.LoadLevel("Tetris");
    }
}
