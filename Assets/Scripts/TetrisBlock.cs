using Photon.Pun;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class TetrisBlock : MonoBehaviourPun, IPunObservable {
    public Vector3 rotationPoint;
    private float previousTime; 
    public float fallTime = 0.8f;
    private float previousSwitch;
    private float switchTime = 0.1f;
    public static int height = 20;
    public static int width = 10;
    private static Transform[,] grid = new Transform[width, height];

    PhotonView view;

    private void Awake(){
        view = GetComponent<PhotonView>();
    }


    void Update() {
        if (view.IsMine){
            Movement();
        }
        if (!ValidMove()){
            CheckIfGrounded();
        }
    }

    private void CheckIfGrounded(){
        transform.position += new Vector3(0, 1, 0);

        view.RPC("TriggerAddToGrid", RpcTarget.All, transform.position);

        SpawnTetromino.Instance.NewTetromino();
        this.enabled = false;
    }

    private void Movement(){
        Rotate();
        Falling();
        LeftRight();
    }

    private void Rotate()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), 90);
            if (!ValidMove())
            {
                transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90);
            }
        }
    }

    private void Falling()
    {
        if (Time.time - previousTime > (Input.GetKey(KeyCode.S) ? fallTime / 10 : fallTime))
        {
            transform.position += new Vector3(0, -1, 0);
            previousTime = Time.time;
        }
    }

    private void LeftRight()
    {
        if (Time.time - previousSwitch < switchTime)
        {
            return;
        }

        Vector3 previousPos = transform.position;

        if (Input.GetKey(KeyCode.A))
        {
            transform.position += Vector3.left;
            previousSwitch = Time.time;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right;
            previousSwitch = Time.time;
        }

        if (!ValidMove())
        {
            transform.position = previousPos;
        }
    }

    [PunRPC]
    void TriggerAddToGrid(Vector3 pos){
        transform.position = pos;
        AddToGrid();
        CheckLines();
        this.enabled = false;
    }

    void CheckLines(){
        for (int i = height-1; i >= 0; i--){
            if (HasLine(i)){
                DeleteLine(i);
                RowDown(i);
            }
        }
    }

    bool HasLine(int line){
        for (int row = 0; row < width; row++){
            if (grid[row,line] is null){
                return false;
            }
        }
        return true;
    }

    void DeleteLine(int line) {
        for (int row = 0; row < width; row++){
            Destroy(grid[row,line].gameObject);
            grid[row, line] = null;
        }
    }

    void RowDown(int line){
        for (int row = line; row < height; row++){
            for (int column = 0; column < width; column++){
                if (grid[column, row] is not null){
                    grid[column, row - 1] = grid[column, row];
                    grid[column, row] = null;
                    grid[column, row - 1].transform.position -= new Vector3(0, 1, 0);
                }
            }
        }
    }

    void AddToGrid(){
        foreach (Transform children in transform){
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);

            grid[roundedX,roundedY] = children;
        }
    }

    bool ValidMove(){
        foreach (Transform children in transform){
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);

            if (roundedX < 0 || roundedX >= width || roundedY < 0 || roundedY >= height){
                return false;
            }

            if (grid[roundedX,roundedY] != null){
                return false;
            }
        }

        return true;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
        Debug.Log("TEST");
        if (stream.IsWriting){
            string json = JsonUtility.ToJson(grid);
            Debug.Log(json);
            stream.SendNext(json);

        }
        else if (stream.IsReading){
            string json = (string)stream.ReceiveNext();
            grid = JsonUtility.FromJson<Transform[,]>(json);

        }
    }
}
