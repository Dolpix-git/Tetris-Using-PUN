using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerItem : MonoBehaviour{
    public TextMeshProUGUI playerName;

    public void SetPlayerInfo(Player player){
        playerName.text = player.NickName;
    }
}
