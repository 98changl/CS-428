using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

public class PlayerData
{
    public int ID;
    public bool tagged;

    public PlayerData (int i, bool t)
    {
        this.ID = i;
        this.tagged = t;
    }
}

public class GameStateManager : MonoBehaviour, IOnEventCallback
{
    public List<PlayerData> playerData = new List<PlayerData>();
    public int index;

    public int matchLength = 120;
    public int currentMatchTime;
    public int scoreA;
    public int scoreB;

    private Text timerText;
    private Text player1;
    private Text player2;
    
    private Coroutine timerCoroutine;
    private Coroutine scoreCoroutine;
    private bool hostTagStatus;
    private bool joined;

    public enum EventCodes : byte
    {
        Wildcard,
        NewPlayer,
        RefreshTimer,
        UpdatePlayers,
        UpdateScore
    }

    // Start is called before the first frame update
    void Start()
    {
        joined = false;
        ValidateConnection();
        InitializeUI();
        InitializeTimer();
        InitializeScore();

        if (PhotonNetwork.IsMasterClient)
        {
            InitializeStartingPlayer();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Tagging tag = (Tagging)hostPlayer.GetComponent(typeof(Tagging));
        //UpdatePlayerModels();
        
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount > 1 && joined == false)
            {
                joined = true;
                InitializeSecondPlayer();
            }
            //CheckForTag();
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void ValidateConnection()
    {
        if (PhotonNetwork.IsConnected) return;
        SceneManager.LoadScene("MultiplayerMenu");
    }

    private void InitializeStartingPlayer()
    {
        GameObject player = GameObject.Find("PhotonPlayer(Clone)");

        PhotonView view = (PhotonView)player.GetComponent(typeof(PhotonView));
        Debug.Log(view.ViewID);

        Tagging tag = (Tagging)player.GetComponent(typeof(Tagging));

        tag.SetTag(true);
        hostTagStatus = tag.GetTag();

        Debug.Log("Get: " + tag.GetTag());
        Debug.Log("Set: " + hostTagStatus);
    }

    private void InitializeSecondPlayer()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            Tagging tag = (Tagging)player.GetComponent(typeof(Tagging));
            PhotonView view = (PhotonView)player.GetComponent(typeof(PhotonView));

            if (view.IsMine == false)
            {
                tag.SetTag(false);
                Debug.Log("Second: " + tag.GetTag());
            }
        }
        UpdatePlayers_Send();
    }

    private void InitializeUI()
    {
        timerText = GameObject.Find("Canvas/TimerText").GetComponent<Text>();
        player1 = GameObject.Find("Canvas/player1").GetComponent<Text>();
        player2 = GameObject.Find("Canvas/player2").GetComponent<Text>();
    }

    private void GameOver()
    {
        // update the timer
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        currentMatchTime = 0;
        RefreshTimerUI();

        // update the score
        if (scoreCoroutine != null) StopCoroutine(scoreCoroutine);


        // stop the game

    }
    /*
    public void NewPlayer_Send(PlayerData p)
    {
        object[] content = new object[2];

        content[0] = PhotonNetwork.LocalPlayer.ActorNumber;
        content[1] = false;

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent((byte)EventCodes.NewPlayer, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void NewPlayer_Read(object[] data)
    {
        PlayerData p = new PlayerData((int)data[0], (bool)data[1]);
        playerData.Add(p);
        UpdatePlayers_Send();
        UpdateScore_Send();
    }
    
    public void UpdatePlayerModels()
    {
        foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.name == "PhotonPlayer(Clone)")
            {
                PhotonView view = (PhotonView)player.GetComponent(typeof(PhotonView));
                Debug.Log(view.IsMine);
            }
        }
    }
    */

    #region UpdatePlayers

    public void CheckForTag()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            Tagging tag = (Tagging)player.GetComponent(typeof(Tagging));
            PhotonView view = (PhotonView)player.GetComponent(typeof(PhotonView));

            if (view.IsMine)
            {
                bool status = tag.GetTag();
                //Debug.Log("Status:" + status);
                if (hostTagStatus != status)
                {
                    UpdatePlayers_Send();
                    hostTagStatus = status;
                }
            }
        }
    }

    public void UpdatePlayers_Send()
    {
        object[] content = new object[PhotonNetwork.CurrentRoom.PlayerCount];

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            Tagging tag = (Tagging)player.GetComponent(typeof(Tagging));
            PhotonView view = (PhotonView)player.GetComponent(typeof(PhotonView));

            if (PhotonNetwork.IsMasterClient) // player 1
            {
                if (view.IsMine)
                {
                    content[0] = tag.GetTag();
                    if (tag.GetTag() == true)
                    {
                        content[1] = false;
                    }
                    else
                    {
                        content[1] = true;
                    }
                    //Debug.Log("Storing:" + content[0]);
                }
                else
                {
                    content[1] = tag.GetTag();
                    if (tag.GetTag() == true)
                    {
                        content[0] = false;
                    }
                    else
                    {
                        content[0] = true;
                    }
                    //Debug.Log("Storing 2:" + content[1]);
                }
            }
        }

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent((byte)EventCodes.UpdatePlayers, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void UpdatePlayers_Read(object[] data)
    {
        //Debug.Log("Receiving Data");
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            Tagging tag = (Tagging)player.GetComponent(typeof(Tagging));
            PhotonView view = (PhotonView)player.GetComponent(typeof(PhotonView));

            if (PhotonNetwork.IsMasterClient) // player 1
            {
                if (view.IsMine)
                {
                    tag.SetTag((bool)data[0]);
                }
                else
                {
                    tag.SetTag((bool)data[1]);
                }
            }
            else // player 2
            {
                if (view.IsMine)
                {
                    tag.SetTag((bool)data[1]);
                }
                else
                {
                    tag.SetTag((bool)data[0]);
                }
            }
        }
    }

    #endregion

    #region Timer

    private void RefreshTimerUI()
    {
        string seconds = currentMatchTime.ToString();
        timerText.text = seconds + " secs remaining";
    }

    private void InitializeTimer()
    {
        currentMatchTime = matchLength;
        RefreshTimerUI();
        
        if (PhotonNetwork.IsMasterClient)
        {
            timerCoroutine = StartCoroutine(Timer());
        }
    }

    public void RefreshTimer_Send()
    {
        object[] content = new object[] { currentMatchTime };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent((byte)EventCodes.RefreshTimer, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void RefreshTimer_Read(object[] data)
    {
        currentMatchTime = (int)data[0];
        RefreshTimerUI();
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(1f);
        currentMatchTime -= 1;

        if (currentMatchTime <= 0)
        {
            timerCoroutine = null;
            GameOver();
        }
        else
        {
            RefreshTimer_Send();
            timerCoroutine = StartCoroutine(Timer());
        }
    }

    #endregion

    #region Score

    private void UpdateScoreUI()
    {
        player1.text = "Player 1: " + scoreA.ToString() + " points";
        player2.text = "Player 2: " + scoreB.ToString() + " points";
    }

    private void InitializeScore()
    {
        scoreA = 0;
        scoreB = 0;
        UpdateScoreUI();

        if (PhotonNetwork.IsMasterClient)
        {
            scoreCoroutine = StartCoroutine(Score());
        }
    }

    public void UpdateScore_Send()
    {
        object[] content = new object[] { scoreA, scoreB };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        bool result = PhotonNetwork.RaiseEvent((byte)EventCodes.UpdateScore, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void UpdateScore_Read(object[] data)
    {
        scoreA = (int)data[0];
        scoreB = (int)data[1];
        UpdateScoreUI();
    }

    private IEnumerator Score()
    {
        yield return new WaitForSeconds(1f);

        // checks game start
        if (currentMatchTime <= 0)
        {
            scoreCoroutine = null;
            //GameOver();
        }
        else
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                // checks which player is tagged
                foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                {
                    if (player.name == "PhotonPlayer(Clone)")
                    {
                        Tagging tag = (Tagging)player.GetComponent(typeof(Tagging));
                        PhotonView view = (PhotonView)player.GetComponent(typeof(PhotonView));

                        if (PhotonNetwork.IsMasterClient) // player 1
                        {
                            if (view.IsMine)
                            {
                                if (tag.GetTag() == true)
                                {
                                    scoreB += 1;
                                }
                                else
                                {
                                    scoreA += 1;
                                }
                            }
                        }

                    }
                }
                UpdateScore_Send();
                UpdatePlayers_Send();
            }
            scoreCoroutine = StartCoroutine(Score());
        }
    }

    #endregion

    void IOnEventCallback.OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code >= 200) return;

        EventCodes e = (EventCodes)photonEvent.Code;
        object[] data = (object[])photonEvent.CustomData;
        //Debug.Log("Event" + data[0]);

        switch (e)
        {
            case EventCodes.NewPlayer:
                //NewPlayer_Read(data);
                break;
            case EventCodes.RefreshTimer:
                RefreshTimer_Read(data);
                break;
            case EventCodes.UpdatePlayers:
                UpdatePlayers_Read(data);
                break;
            case EventCodes.UpdateScore:
                UpdateScore_Read(data);
                break;
        }
    }
    
}
