using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombManager : MonoBehaviour
{
    [HideInInspector]
    private List<Bomb> bombs = new List<Bomb>();

    public void InputBomb(Bomb bomb)
    {
        bombs.Add(bomb);
    }

    public void RemoveBomb(Bomb bomb)
    {
        bombs.Remove(bomb);
    }

    public Bomb GetBomb(int index)
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

    public int Count()
    {
        return bombs.Count;
    }
}
