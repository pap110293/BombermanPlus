using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Networking;

public class BombSpawner : NetworkBehaviour
{
    [HideInInspector]
    public Tilemap tilemap;
    public GameObject bombPrefab;
    public float moveSpeed = 1f;
    public int level = 1;
    public int numberOfBomb = 2;

    private Camera mainCamera;
    private MapDestroyer mapDestroyer;

    public override void OnStartLocalPlayer()
    {
        mainCamera = Camera.main;
        tilemap = GameObject.FindGameObjectWithTag("Tilemap").GetComponent<Tilemap>();
        mapDestroyer = FindObjectOfType<MapDestroyer>();
        InitPosition();
    }

    public override void OnStartServer()
    {
        if (isServer)
        {
            tilemap = GameObject.FindGameObjectWithTag("Tilemap").GetComponent<Tilemap>();
            mapDestroyer = FindObjectOfType<MapDestroyer>();
        }
    }

    private void InitPosition()
    {
        var cell = tilemap.WorldToCell(transform.position);
        var cellCenterPos = tilemap.GetCellCenterWorld(cell);
        transform.position = cellCenterPos;
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.Space))
            CraeteABomb();

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

    private void CraeteABomb()
    {
        var cell = tilemap.WorldToCell(transform.position);
        var existedBomb = mapDestroyer.GetBombFromCell(cell);
        if (existedBomb != null) return;
        var cellCenterPos = tilemap.GetCellCenterWorld(cell);
    }

    [Command]
    private void CmdCreateABomb(Vector3 pos)
    {
        var bomb = Instantiate(bombPrefab, pos, Quaternion.identity);
        NetworkServer.Spawn(bomb);
        RpcCreateABomb(bomb);
    }

    [ClientRpc]
    private void RpcCreateABomb(GameObject bomb)
    {
        bomb.GetComponent<Bomb>().SetLevel(level);
    }
}
