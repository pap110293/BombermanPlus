﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Tilemaps;

public class MapManager : NetworkBehaviour
{

    public Tilemap tilemap;
    public Tile wallTile;
    public Tile destructableTile;
    public bool isAutoGeneratedMap = true;
    [Range(40, 70)]
    public int density = 50;
    public GameObject explosionPrefab;
    public SyncListCell DestructablTileOnMap = new SyncListCell();

    private ItemManager itemManager;

    private const int ROW = 11;
    private const int COL = 15;

    private void Start()
    {
        itemManager = FindObjectOfType<ItemManager>();
    }

    public override void OnStartClient()
    {
        if (DestructablTileOnMap.Count == 0)
            InitDestructableOnTheMap();
        CreateMap();
    }

    public void CreateMap()
    {
        for (int i = 0; i < DestructablTileOnMap.Count; i++)
        {
            var cell = Vector3Int.FloorToInt(DestructablTileOnMap[i].cell);
            tilemap.SetTile(cell, destructableTile);
        }
    }

    [Command]
    public void CmdRebuildTheMap()
    {
        RpcRebuildTheMap();
    }

    [ClientRpc]
    private void RpcRebuildTheMap()
    {
        ClearTheMap();
        CreateMap();
    }

    public void ClearTheMap()
    {
        var startCellTrans = FindObjectOfType<NetworkStartPosition>().transform;
        var startCell = tilemap.WorldToCell(startCellTrans.position);
        for (int i = 0; i < ROW; i++)
        {
            for (int j = 0; j < COL; j++)
            {
                var tempCell = new Vector3Int(startCell.x + j, startCell.y + i, startCell.z);
                Tile tile = tilemap.GetTile<Tile>(tempCell);
                if (tile == destructableTile)
                    tilemap.SetTile(tempCell, null);
            }
        }
    }

    public void InitDestructableOnTheMap()
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
                if (tile != wallTile)
                    listValidCell.Add(tempCell);
            }
        }

        DestructablTileOnMap.Clear();
        if (isAutoGeneratedMap)
        {
            for (int i = 0; i < listValidCell.Count * density / 100; i++)
            {
                int index = Random.Range(0, listValidCell.Count);
                var tempCell = listValidCell[index];
                DestructablTileOnMap.Add(new StructCell { cell = tempCell });
                listValidCell.Remove(tempCell);
            }
        }
    }

    public void Explode(Vector2 worldPos, int level)
    {
        Vector3Int originCell = tilemap.WorldToCell(worldPos);
        Bomb originBomb = GetBombFromCell(originCell);

        if (originBomb != null)
        {
            originBomb.isExplosed = true;
            originBomb.GetComponent<SpriteRenderer>().enabled = false;
            originBomb.GetComponent<Collider2D>().enabled = false; ;
        }

        ExplodeCell(originCell);

        Vector3Int tempCell;

        // go up
        tempCell = originCell;
        for (int i = 1; i <= level; i++)
        {
            tempCell.y++;
            if (ExplodeCell(tempCell) == false)
                break;
        }

        // go down
        tempCell = originCell;
        for (int i = 1; i <= level; i++)
        {
            tempCell.y--;
            if (ExplodeCell(tempCell) == false)
                break;
        }

        // go left
        tempCell = originCell;
        for (int i = 1; i <= level; i++)
        {
            tempCell.x--;
            if (ExplodeCell(tempCell) == false)
                break;
        }

        // go right
        tempCell = originCell;
        for (int i = 1; i <= level; i++)
        {
            tempCell.x++;
            if (ExplodeCell(tempCell) == false)
                break;
        }
    }

    private void RemoveTile(Vector3Int cell)
    {
        Vector3 v3Cell = cell;
        for (int i = 0; i < DestructablTileOnMap.Count; i++)
        {
            if (DestructablTileOnMap[i].cell == v3Cell)
            {
                DestructablTileOnMap.Remove(DestructablTileOnMap[i]);
                break;
            }
        }
    }

    private bool ExplodeCell(Vector3Int cell)
    {
        Tile tile = tilemap.GetTile<Tile>(cell);
        Vector3 cellPosition = tilemap.GetCellCenterWorld(cell);

        if (tile == wallTile)
            return false;

        Instantiate(explosionPrefab, cellPosition, Quaternion.identity);

        if (tile == destructableTile)
        {
            itemManager.CmdSpawRandomItem(cellPosition); // create random item
            tilemap.SetTile(cell, null); // remove the tile
            RemoveTile(cell);
            return false;
        }

        Item item = GetItemOnCell(cell);
        if (item != null) // if hit a item
        {
            Destroy(item.gameObject);
        }

        BombSpawner player = GetPlayerOnCell(cell);
        if (player != null) // if hit a player
        {
            player.Die();
        }

        Bomb bomb = GetBombFromCell(cell);
        if (bomb != null && bomb.isExplosed == false) // if hit an unexplosed bomb
        {
            bomb.isExplosed = true;
            Explode(bomb.transform.position, bomb.GetLevel());
        }

        return true;
    }

    public Bomb GetBombFromCell(Vector3Int cell)
    {
        Bomb[] bombs = FindObjectsOfType<Bomb>();

        for (int i = 0; i < bombs.Length; i++)
        {
            Vector3 bombPos = bombs[i].transform.position;
            Vector3Int cellOfBombOn = tilemap.WorldToCell(bombPos);
            if (cell == cellOfBombOn)
            {
                return bombs[i];
            }
        }
        return null;
    }

    public BombSpawner GetPlayerOnCell(Vector3Int cell)
    {
        BombSpawner[] bombSpawners = FindObjectsOfType<BombSpawner>();
        for (int i = 0; i < bombSpawners.Length; i++)
        {
            Vector3Int cellOfPlayerOn = tilemap.WorldToCell(bombSpawners[i].transform.position);
            if (cell == cellOfPlayerOn)
            {
                return bombSpawners[i];
            }
        }

        return null;
    }

    public Item GetItemOnCell(Vector3Int cell)
    {
        var allItem = itemManager.GetItems();
        for (int i = 0; i < allItem.Length; i++)
        {
            Vector3Int cellOfItemOn = tilemap.WorldToCell(allItem[i].transform.position);
            if (cell == cellOfItemOn)
            {
                return allItem[i];
            }
        }
        return null;
    }

    public struct StructCell
    {
        public Vector3 cell;
    }

    public class SyncListCell : SyncListStruct<StructCell> { }
}
