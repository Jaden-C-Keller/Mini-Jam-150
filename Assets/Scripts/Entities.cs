using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entities : MonoBehaviour
{
    public static List<Rewindable> rewindables = new List<Rewindable>();
    public static List<BoxCollider2D> walls;
    public static List<BoxCollider2D> water;
    public static List<Box> boxes = new List<Box>();
    public static List<Candle> candles = new List<Candle>();
    public static List<BoxCollider2D> rooms = new List<BoxCollider2D>();
    public static int step = 0;
    

    private void Start()
    {
        walls = new List<BoxCollider2D>(GameObject.FindGameObjectWithTag("wall").GetComponents<BoxCollider2D>());

        water = new List<BoxCollider2D>(GameObject.FindGameObjectWithTag("water").GetComponents<BoxCollider2D>());    

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
            box.Move(Vector3.zero);
            box.audio.Stop();
            boxes.Add(box);
        }

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("room"))
        {
            rooms.Add(obj.GetComponent<BoxCollider2D>());
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
