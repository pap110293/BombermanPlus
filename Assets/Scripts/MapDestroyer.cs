using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Tilemaps;

public class MapDestroyer : MonoBehaviour {

    public Tilemap tilemap;
    public Tile wallTile;
    public Tile destructableTile;
    public GameObject explosionPrefab;

    private ItemManager itemManager;

    private void Start()
    {
        itemManager = FindObjectOfType<ItemManager>();
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

    private bool ExplodeCell(Vector3Int cell)
    {
        Tile tile = tilemap.GetTile<Tile>(cell);
        Vector3 cellPosition = tilemap.GetCellCenterWorld(cell);

        if (tile == wallTile)
            return false;

        if(tile == destructableTile)
        {
            itemManager.CmdSpawRandomItem(cellPosition);
            tilemap.SetTile(cell, null); // remove the tile
            return false;
        }

        BombSpawner player = GetPlayerOnCell(cell);
        if(player != null)
        {
            player.Die();
        }

        Instantiate(explosionPrefab, cellPosition, Quaternion.identity);

        Bomb bomb = GetBombFromCell(cell);
        if(bomb != null && bomb.isExplosed == false)
        {
            bomb.isExplosed = true;
            Explode(bomb.transform.position, bomb.GetLevel());
        }


        return true;
    }

    public Bomb GetBombFromCell(Vector3Int cell)
    {
        GameObject[] bombs = GameObject.FindGameObjectsWithTag("Bomb");

        for (int i = 0; i < bombs.Length; i++)
        {
            GameObject bomb = bombs[i];
            Vector3 bombPos = bomb.transform.position;
            Vector3Int cellOfBombOn = tilemap.WorldToCell(bombPos);
            if(cell == cellOfBombOn)
            {
                return bomb.GetComponent<Bomb>();
            }
        }
        return null;
    }

    public BombSpawner GetPlayerOnCell(Vector3Int cell)
    {
        GameObject[] bombSpawners = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < bombSpawners.Length; i++)
        {
            Vector3Int cellOfPlayerOn = tilemap.WorldToCell(bombSpawners[i].transform.position);
            if (cell == cellOfPlayerOn)
            {
                return bombSpawners[i].GetComponent<BombSpawner>();
            }
        }

        return null;
    }
}
