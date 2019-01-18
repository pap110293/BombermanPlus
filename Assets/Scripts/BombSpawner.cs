using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;

public class BombSpawner : NetworkBehaviour
{
    public static int playerNumber = 0;
    [SyncVar(hook = "onCurrentPlayerNumberChanged")]
    public int currenPlayerNumber = 0;

    public Tilemap tilemap;
    public GameObject bombPrefab;
    public float moveSpeed = 1f;
    public int level = 1;
    public int maxNumberOfBomb = 2;

    private MapManager mapManager;

    private const int ROW = 11;
    private const int COL = 15;

    public override void OnStartLocalPlayer()
    {
        CmdLogIn();
        tilemap = GameObject.FindGameObjectWithTag("Tilemap").GetComponent<Tilemap>();
        mapManager = FindObjectOfType<MapManager>();
        mapManager.CreateRandomMap();
        InitPosition();
    }

    [Command]
    private void CmdLogIn()
    {
        playerNumber++;
        currenPlayerNumber = playerNumber;
    }

    private void onCurrentPlayerNumberChanged(int newPlayerNumber)
    {
        this.currenPlayerNumber = newPlayerNumber;
    }

    private void InitPosition()
    {
        var startCellTrans = FindObjectOfType<NetworkStartPosition>().transform;
        var startCell = tilemap.WorldToCell(startCellTrans.position);
        var listValidCell = new List<Vector3Int>();
        for (int i = 0; i < ROW; i++)
        {
            for (int j = 0; j < COL; j++)
            {
                var tempCell = new Vector3Int(startCell.x + j, startCell.y + i, startCell.z);
                Tile tile = tilemap.GetTile<Tile>(tempCell);
                if (tile == null)
                    listValidCell.Add(tempCell);
            }
        }
        var randomCell = listValidCell[Random.Range(0, listValidCell.Count)];
        var cellCenterPos = tilemap.GetCellCenterWorld(randomCell);
        transform.position = cellCenterPos;
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.Space))
            CreateABomb();

        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        if (Mathf.Abs(x) + Mathf.Abs(y) > 0)
            Move(x, y);
    }

    public void Die()
    {
        // TODO: player die
        Debug.Log("die");
    }

    private void Move(float horizontal, float vertical)
    {
        Vector3 newPos = transform.position;
        newPos.x += horizontal * moveSpeed * Time.deltaTime;
        newPos.y += vertical * moveSpeed * Time.deltaTime;
        transform.position = newPos;

        // TODO: need other way to move
    }

    public void CreateABomb(Vector3 position)
    {
        CmdCreateABomb(position, level, currenPlayerNumber);
    }

    public void CreateABomb(Vector3Int cell)
    {
        CmdCreateABomb(tilemap.GetCellCenterWorld(cell), level, currenPlayerNumber);
    }

    public void CreateABomb()
    {
        // check can player put a bomb
        var cell = tilemap.WorldToCell(transform.position);
        var existedBomb = mapManager.GetBombFromCell(cell);
        var myBombs = FindObjectsOfType<Bomb>().Where(i => i.playerNumber == currenPlayerNumber);
        if (existedBomb != null || myBombs.Count() >= maxNumberOfBomb) return;
        // end check

        var cellCenterPos = tilemap.GetCellCenterWorld(cell);
        CmdCreateABomb(cellCenterPos, level, currenPlayerNumber);
    }

    [Command]
    private void CmdCreateABomb(Vector3 pos, int bombLevel, int playerNumber)
    {
        var bombObj = Instantiate(bombPrefab, pos, Quaternion.identity);
        NetworkServer.Spawn(bombObj);
        RpcCreateABomb(bombObj, bombLevel, playerNumber);
    }

    [ClientRpc]
    private void RpcCreateABomb(GameObject bombObj, int bombLevel, int playerNumber)
    {
        // set some value for the bomb affter be created
        var bomb = bombObj.GetComponent<Bomb>();
        bomb.playerNumber = playerNumber;
        bomb.SetLevel(bombLevel);
    }
}
