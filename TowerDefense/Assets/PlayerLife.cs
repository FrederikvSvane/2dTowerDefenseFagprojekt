using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _hp;

    private Photon.Realtime.Player _player;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_player == null) return;
        _name.text = _player.NickName;
        _hp.text = "100";
    }
    
    public void SetPlayer(Photon.Realtime.Player player){
        _player = player;
    }
}
