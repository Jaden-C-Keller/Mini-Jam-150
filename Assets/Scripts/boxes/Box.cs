using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Box : MonoBehaviour, Rewindable
{
    public SpriteRenderer render;
    public Sprite start, sunk;
    public new AudioSource audio;
    public AudioClip slide, water;

    protected int walkableStep = -1;

    protected Stack<Vector3> positions = new Stack<Vector3>();
    public bool walkable = false;

    public void Awake()
    {
        gameObject.tag = "box";
    }

    public virtual bool Move(Vector3 direction)
    {
        Vector3 newPos = transform.position + direction;

        foreach (BoxCollider2D wall in Entities.walls)
        {
            if (wall.bounds.Contains(newPos))
            {
                return false;
            }
        }

        foreach (Box box in Entities.boxes)
        {
            if(box != this && !box.walkable && newPos == box.transform.position)
            {
                return false;
            }
        }
        
        transform.position = newPos;

        CandleCheck();

        return true;
    }

    //private void Update()
    //{
    //    if (Darkness.inShadow(transform.position))
    //    {
    //        render.color = new Color(.8f, .8f, .8f);
    //    }
    //    else
    //    {
    //        render.color = Color.white;
    //    }
    //}

    public void CandleCheck()
    {
        foreach (Candle candle in Entities.candles)
        {
            if (transform.childCount == 0 && transform.position == candle.transform.position)
            {
                candle.pickup(transform);
            }
        }
    }

    protected void sink(bool drop = true)
    {
        render.sprite = sunk;
        render.sortingOrder = 0;
        walkable = true;
        
        if (drop && transform.childCount == 1)
        {
            transform.GetChild(0).SendMessage("drop");
        }
        else if(transform.childCount == 1)
        {
            transform.GetChild(0).SendMessage("lower");
        }
    }

    public virtual void add()
    {
        positions.Push(transform.position);
    }

    public virtual void pop()
    {
        if (positions.Count == 0)
        {
            return;
        }
        

        
        if (walkableStep == Entities.step)
        {
            walkable = false;
            render.sprite = start;
            render.sortingOrder = 2;
            walkableStep = -1;
        }

        transform.position = positions.Pop();
    }
}
