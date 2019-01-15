using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;

public class BombSpawner : NetworkBehaviour
{
    [HideInInspector]
    public Tilemap tilemap;
    public GameObject bombPrefab;
    public float moveSpeed = 1f;
    public int level = 1;
    public int maxNumberOfBomb = 2;

    private MapDestroyer mapDestroyer;

    public override void OnStartLocalPlayer()
    {
        tilemap = GameObject.FindGameObjectWithTag("Tilemap").GetComponent<Tilemap>();
        mapDestroyer = FindObjectOfType<MapDestroyer>();
        InitPosition();
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
        var myBombs = FindObjectsOfType<Bomb>().Where(i => i.Owner == this);
        if (existedBomb != null || myBombs.Count() >= maxNumberOfBomb) return;
        var cellCenterPos = tilemap.GetCellCenterWorld(cell);
        CmdCreateABomb(cellCenterPos);
    }

    [Command]
    private void CmdCreateABomb(Vector3 pos)
    {
        var bomb = Instantiate(bombPrefab, pos, Quaternion.identity);
        NetworkServer.Spawn(bomb);
        RpcCreateABomb(bomb);
    }

    [ClientRpc]
    private void RpcCreateABomb(GameObject bombObj)
    {
        var bomb = bombObj.GetComponent<Bomb>();
        bomb.Owner = this;
        bomb.SetLevel(level);
    }
}
