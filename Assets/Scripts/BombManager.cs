using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombManager : MonoBehaviour {

    public static List<Bomb> bombs = new List<Bomb>();

    public static Bomb CreateABomb(GameObject bombPrefab, Vector3 position)
    {
        var bombGameobject = Instantiate(bombPrefab, position, Quaternion.identity);
        var bomb = bombGameobject.GetComponent<Bomb>();
        bombs.Add(bomb);
        return bomb;
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
