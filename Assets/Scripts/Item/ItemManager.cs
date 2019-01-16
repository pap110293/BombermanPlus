﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ItemManager : NetworkBehaviour
{
    [Range(0f,100f)]
    public float dropRate;
    public GameObject[] items;
    [Command]
    public void CmdSpawRandomItem(Vector3 position)
    {
        if (Random.value < dropRate / 100f) // drop tate of item
        {
            var randomItem = items[Random.Range(0, items.Length)]; // random item
            var item = Instantiate(randomItem, position, Quaternion.identity);
            NetworkServer.Spawn(item);
        }
    }
}
