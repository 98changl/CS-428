using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyStarter : MonoBehaviourPunCallbacks
{
    public Text error;

    public InputField newRoom;
    public InputField joinRoom;
    public Button createButton;
    public Button JoinButton;

    private int maxPlayerCount = 2;

    private void Start()
    {
        error.enabled = false;
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void Join()
    {
        PhotonNetwork.JoinRoom(joinRoom.text);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        error.text = "Failed to join.";
        error.enabled = true;
    }

    public void RoomCreate()
    {
        RoomOptions roomSpecs = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = (byte)maxPlayerCount
        };

        PhotonNetwork.CreateRoom(newRoom.text, roomSpecs);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        error.text = "Failed to create room";
        error.enabled = true;
    }

    public void BackButton()
    {
        SceneManager.LoadScene(sceneBuildIndex: 0);
    }

}
