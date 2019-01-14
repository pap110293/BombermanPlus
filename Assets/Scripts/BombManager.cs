using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombManager : MonoBehaviour {

    public static List<Bomb> bombs = new List<Bomb>();

    public static void InputABomb(Bomb bomb)
    {
        bombs.Add(bomb);
    }

    public static void DestroyABomb(Bomb bomb)
    {
        bombs.Remove(bomb);
        Destroy(bomb.gameObject);
    }

    public static Bomb GetBomb(int index)
    {
        try
        {
            return bombs[index];
        }
        catch (System.Exception)
        {

            return null;
        }
    }
}
