using Photon.Pun;
using UnityEngine.SceneManagement;
using TMPro;

public class ConnectToServer : MonoBehaviourPunCallbacks {
    public TMP_InputField usernameInput;
    public TextMeshProUGUI buttonText;

    public void OnClickConnect(){
        if (usernameInput.text.Length >= 1){
            PhotonNetwork.NickName = usernameInput.text;
            buttonText.text = "Connecting...";
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.SerializationRate = 5;

            PhotonNetwork.ConnectUsingSettings();
            
        }
    }

    public override void OnConnectedToMaster(){
        SceneManager.LoadScene("Lobby");
    }
}
