using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Realtime;
//using static Photon.Realtime.IOnEventCallback;
using Photon.Pun;

public class GameStateManager : MonoBehaviour, IOnEventCallback
{
    private Text timerText;
    public int matchLength = 120;
    public int currentMatchTime;
    private Coroutine timerCoroutine;

    public enum EventCodes : byte
    {
        Wildcard,
        RefreshTimer
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeUI();
        InitializeTimer();
    }

    // Update is called once per frame
    void Update()
    {
        RefreshTimerUI();
    }

    private void InitializeUI()
    {
        timerText = GameObject.Find("Canvas/TimerText").GetComponent<Text>();
    }

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

    private void GameOver()
    {
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        currentMatchTime = 0;
        RefreshTimerUI();
    }

    public void RefreshTimer_Send()
    {
        object[] content = new object[] { currentMatchTime };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        bool result = PhotonNetwork.RaiseEvent((byte)EventCodes.RefreshTimer, content, raiseEventOptions, SendOptions.SendReliable);
        //Debug.Log(result);
    }

    public void RefreshTimer_Read(object[] data)
    {
        Debug.Log("Receiving");
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

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
        //PhotonNetwork.OnEventCall += this.OnEventRaised;
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    void IOnEventCallback.OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code >= 200) return;

        EventCodes e = (EventCodes)photonEvent.Code;
        
        object[] data = (object[])photonEvent.CustomData;
        Debug.Log("Event" + data[0]);

        switch (e)
        {
            case EventCodes.RefreshTimer:
                Debug.Log("Receive");
                RefreshTimer_Read(data);
                break;
        }
    }
    
}
