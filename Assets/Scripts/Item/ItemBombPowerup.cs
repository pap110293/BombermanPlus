using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ItemBombPowerup : Item
{
    public int addBomb = 1;

    protected override void ItemEffectToPlayer(BombSpawner player)
    {
        player.maxNumberOfBomb += addBomb;
    }
}
