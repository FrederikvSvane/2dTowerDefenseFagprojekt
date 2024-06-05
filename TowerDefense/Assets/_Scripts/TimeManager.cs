using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;
using System;


public class TimeManager : MonoBehaviour, IPunInstantiateMagicCallback
{
    public TextMeshProUGUI _timeText;
    public int _timeMinutes = 0;
    public float _timeSeconds;
    public int _timeHours;
    public float _timeMultiplier = 1;
    private bool _isLongerThanHour = false;

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        _timeText = GameObject.Find("TimeStamp").GetComponent<TextMeshProUGUI>();
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("TimeManager initialized: " + _timeText.text);

    }

    // Update is called once per frame
    void Update()
    {
        _timeSeconds += Time.deltaTime * _timeMultiplier;        
        string niceTime = !_isLongerThanHour ? string.Format("{0:00}:{1:00}", _timeMinutes, Math.Round(_timeSeconds, 0)) : string.Format("{0:00}:{1:00}:{2:00}", _timeHours, _timeMinutes, Math.Round(_timeSeconds, 0));
        if (_timeSeconds >= 60)
        {
            _timeMinutes++;
            _timeSeconds = 0;
        }

        if(_timeMinutes >= 60)
        {
            _timeHours++;
            _timeMinutes = 0;
            _isLongerThanHour = true;
        }

        _timeText.text = niceTime;

    }
}
