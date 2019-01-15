using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Item : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        ItemEffectToPlayer(collision.GetComponent<BombSpawner>());
        Destroy(gameObject);
    }

    protected abstract void ItemEffectToPlayer(BombSpawner player);

    //[Command]
    //protected abstract void CmdExecute(GameObject player);

    //[ClientRpc]
    //protected abstract void RpcExecute(GameObject player);
}
