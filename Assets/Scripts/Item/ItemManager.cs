using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ItemManager : NetworkBehaviour {

    public GameObject[] items;

	[Command]
    public void CmdSpawItem(Vector3 position)
    {
        var randomItem = items[Random.Range(0, items.Length)];
        var item = Instantiate(randomItem, position, Quaternion.identity);
        NetworkServer.Spawn(item);
    }
}
