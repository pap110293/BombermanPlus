using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapDestroyer : MonoBehaviour {

    public Tilemap tilemap;
    public Tile wallTile;
    public Tile destructableTile;
    public GameObject explosionPrefab;

    private GameObject[] bombSpawners;

    private void Awake()
    {
        bombSpawners = GameObject.FindGameObjectsWithTag("Player");
    }

    public void Explode(Vector2 worldPos, int level)
    {
        var originCell = tilemap.WorldToCell(worldPos);
        var originBomb = GetBombFromCell(originCell);

        if (originBomb != null)
        {
            originBomb.isExplose = true;
            BombManager.DestroyABomb(originBomb);
        }

        ExplodeCell(originCell);

        Vector3Int tempCell;

        // go up
        tempCell = originCell;
        for (int i = 1; i <= level; i++)
        {
            tempCell.x++;
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
        if (tile == wallTile)
            return false;

        if(tile == destructableTile)
        {
            tilemap.SetTile(cell, null); // remove the tile
            return false;
        }

        var player = CheckIsPlayerBeExplosed(cell);
        if(player != null)
        {
            player.Die();
        }

        var pos = tilemap.GetCellCenterWorld(cell);
        Instantiate(explosionPrefab, pos, Quaternion.identity);

        var bomb = GetBombFromCell(cell);
        if(bomb != null && bomb.isExplose == false)
        {
            bomb.isExplose = true;
            Explode(bomb.transform.position, bomb.GetLevel());
        }


        return true;
    }

    private Bomb GetBombFromCell(Vector3Int cell)
    {
        for (int i = 0; i < BombManager.bombs.Count; i++)
        {
            var bomb = BombManager.GetBomb(i);
            var bombPos = bomb.transform.position;
            var cellOfBombOn = tilemap.WorldToCell(bombPos);
            if(cell == cellOfBombOn)
            {
                return bomb;
            }
        }
        return null;
    }

    private BombSpawner CheckIsPlayerBeExplosed(Vector3Int cell)
    {
        for (int i = 0; i < bombSpawners.Length; i++)
        {
            var cellOfPlayerOn = tilemap.WorldToCell(bombSpawners[i].transform.position);
            if (cell == cellOfPlayerOn)
            {
                return bombSpawners[i].GetComponent<BombSpawner>();
            }
        }

        return null;
    }
}
