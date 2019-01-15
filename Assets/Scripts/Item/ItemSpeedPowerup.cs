using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpeedPowerup : Item {

    public float addSpeed = 1f;
    protected override void ItemEffectToPlayer(BombSpawner player)
    {
        player.moveSpeed += addSpeed;
    }
}
