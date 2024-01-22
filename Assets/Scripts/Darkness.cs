using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Darkness : MonoBehaviour
{
    public GameObject darkness;
    public Transform upperLeft, lowerRight;
    static Vector2 upLeft;
    public static List<List<GameObject>> tile = new List<List<GameObject>>();

    int height, width;

    public static Vector2 translate(Vector2 pos)
    {
        return new Vector2(pos.x - upLeft.x, upLeft.y - pos.y);
    }

    public static bool inShadow(Vector2 pos)
    {
        pos = translate(pos);

        if (pos.y < 0 || pos.y >= tile.Count || pos.x < 0 || pos.x >= tile[0].Count) return false;

        return tile[(int)pos.y][(int)pos.x].activeSelf;
    }

    public static void checkLight()
    {
        for (int i = 0; i < tile.Count; i++)
        {
            for (int j = 0; j < tile[0].Count; j++)
            {
                tile[i][j].SetActive(true);
            }
        }

        foreach (Candle candle in Entities.candles)
        {
            Vector2 pos = translate(candle.transform.position);
            for (int i = -1; i < 2; i++)
            {
                int y = (int)(pos.y + i);
                if (y < 0 || y >= tile.Count)
                {
                    continue;
                }

                for (int j = -1; j < 2; j++)
                {
                    int x = (int)(pos.x + j);
                    if (x < 0 || x >= tile[0].Count)
                    {
                        continue;
                    }

                    tile[y][x].SetActive(false);
                }
            }
        }
    }

    public void Start()
    {
        upLeft = upperLeft.position;

        height = (int)(upLeft.y - lowerRight.position.y);
        width = (int)(lowerRight.position.x - upLeft.x);

        for(int i = 0; i <= height; i++)
        {
            tile.Add(new List<GameObject>());
            for(int j = 0; j <= width; j++)
            {
                tile[i].Add(Instantiate(darkness, new Vector2(upLeft.x + j, upLeft.y - i), Quaternion.identity, transform));
            }
        }
    }
}
