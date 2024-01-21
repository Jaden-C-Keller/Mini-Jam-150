using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entities : MonoBehaviour
{
    public static List<Rewindable> rewindables = new List<Rewindable>();
    public static List<CompositeCollider2D> walls = new List<CompositeCollider2D>();
    public static List<CompositeCollider2D> water = new List<CompositeCollider2D>();
    public static List<Box> boxes = new List<Box>();
    public static List<Candle> candles = new List<Candle>();
    public static int step = 0;
    

    private void Start()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("wall"))
        {
            walls.Add(obj.GetComponent<CompositeCollider2D>());
        }

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("water"))
        {
            water.Add(obj.GetComponent<CompositeCollider2D>());
        }

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("candle"))
        {
            Candle candle = obj.GetComponent<Candle>();
            candles.Add(candle);
            rewindables.Add(candle);

        }

        Darkness.checkLight();

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("box"))
        {
            Box box = obj.GetComponent<Box>();
            rewindables.Add(box);
            box.CandleCheck();
            boxes.Add(box);
        }

        
    }

    public static void record()
    {
        step++;
        foreach (Rewindable rewindable in rewindables)
        {
            rewindable.add();
        }
    }

    public static void undo()
    {
        if(step == 0)
        {
            return;
        }

        foreach (Rewindable rewindable in rewindables)
        {
            rewindable.pop();
        }
        step--;
        Darkness.checkLight();
    }
}
