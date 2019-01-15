using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFlamePowerup : Item {

    public int addPower = 1;
    protected override void ItemEffectToPlayer(BombSpawner player)
    {
        player.level += addPower;
    }
}
