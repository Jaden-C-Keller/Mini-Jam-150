using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalBox : Box
{
    public override bool Move(Vector3 direction)
    {
        Vector3 prev = transform.position;

        bool moved = base.Move(direction);


        if (moved)
        {
            audio.clip = slide;
            foreach (BoxCollider2D water in Entities.water)
            {
                
                if (water.bounds.Contains(transform.position))
                {

                    bool willSink = true;
                    foreach (Box box in Entities.boxes)
                    {
                        if(box.walkable && transform.position == box.transform.position)
                        {
                            willSink = false;
                            break;
                        }
                    }

                    if (willSink)
                    {
                        walkableStep = Entities.step;
                        audio.clip = base.water;
                        sink();
                    }
                    break;
                }
            }

            if(prev != transform.position)
            {
                audio.Play();
            }
        }

        

        return moved;
    }
}
