using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnTetromino : MonoBehaviour{
    [SerializeField]private GameObject[] tetrominoes;

    public static SpawnTetromino Instance { get; private set; }
    private void Awake(){
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this){
            Destroy(this);
        }
        else{
            Instance = this;
        }
    }

    private void Start(){
        NewTetromino();
    }

    public void NewTetromino(){
        PhotonNetwork.Instantiate(tetrominoes[Random.Range(0,tetrominoes.Length)].name, transform.position,Quaternion.identity);
    }
}
