using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodenBox : Box
{
    static List<WoodenBox> woodenBoxes = new List<WoodenBox>();
    Stack<bool> floatings = new Stack<bool>();
    Stack<Vector3> directions = new Stack<Vector3>();
    Stack<int> steps = new Stack<int>();

    bool floating = false;
    Vector3 direction = Vector3.zero;

    private void Start()
    {
        steps.Push(walkableStep);
        directions.Push(direction);
        woodenBoxes.Add(this);
    }

    public override bool Move(Vector3 direction)
    {

        Vector3 prev = transform.position;

        bool moved = base.Move(direction);

        if(!moved)
        {
            if(floating && transform.childCount == 1)
            {
                this.direction = Vector3.zero;
                transform.GetChild(0).SendMessage("drop");
            }
            return false;
        }

        if (!floating)
        {
            audio.clip = slide;
        }
        
        bool startedFloating = floating;
        floating = false;
        

        foreach (BoxCollider2D water in Entities.water)
        {
            if (water.bounds.Contains(transform.position))
            {
                foreach (Box box in Entities.boxes)
                {
                    if (box != this && transform.position == box.transform.position)
                    {
                        goto skip;
                    }
                }
                floating = true;
                this.direction = direction;
                break;
            }
        }
        skip:

        

        if (!startedFloating && floating)
        {
            audio.clip = water;
            audio.Play();
            walkableStep = Entities.step;
            sink(false);
            this.direction = direction;
            directions.Push(direction);
            steps.Push(walkableStep);
        }

        if(startedFloating && !floating)
        {
            walkableStep = Entities.step;
            rise();
            steps.Push(walkableStep);
            this.direction = Vector3.zero;
            directions.Push(this.direction);
        }

        if (render.sprite != sunk && prev != transform.position && !floating && !startedFloating)
        {
            audio.Play();
        }

        return moved;
    }

    void rise()
    {
        render.sprite = start;
        render.sortingOrder = 2;
        walkable = false;
        if(transform.childCount == 1)
        {
            transform.GetChild(0).SendMessage("pickup", transform);
        }
    }

    public static void Float(){
        foreach(WoodenBox box in woodenBoxes)
        {
            if (box.direction != Vector3.zero)
            {
                box.Move(box.direction);
            }
        }
    }

    public override void add()
    {
        base.add();
        floatings.Push(floating);
    }

    public override void pop()
    {
        if (positions.Count == 0)
        {
            return;
        }

        floating = floatings.Pop();


        if (walkableStep == Entities.step)
        {
            steps.Pop();
            directions.Pop();

            walkableStep = steps.Peek();
            direction = directions.Peek();

            if(direction == Vector3.zero)
            {
                rise();
            }
            else
            {
                sink(false);
            }
        }

        transform.position = positions.Pop();
    }
}
