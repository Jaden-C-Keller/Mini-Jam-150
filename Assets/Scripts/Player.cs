using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, Rewindable
{
    public GameObject lossText;
    public new AudioSource audio;
    public AudioClip step, drag, undo;

    Stack<Vector3> positions = new Stack<Vector3>();
    float timer = 0f;
    float undoTimer = 0f;

    private void Awake()
    {
        Entities.rewindables.Add(this);
    }
    

    void Update()
    {
        Vector3 direction;
        float input = Input.GetAxisRaw("Horizontal");
        bool undoDown = Input.GetKey(KeyCode.Z);


        if (input == 0f)
        {
            direction = new Vector3(0f, Input.GetAxisRaw("Vertical"));
            
        }
        else
        {
            direction = new Vector3(input, 0f);
        }

        if(!lossText.activeSelf && !undoDown && timer == 0f && direction != Vector3.zero)
        {
            move(direction);
        }

        if(direction != Vector3.zero)
        {
            timer += Time.deltaTime;
        }

        if(direction == Vector3.zero || timer >= 0.25f)
        {
            timer = 0f;
        }

        
        if (undoDown)
        {
            if(Entities.step > 0 && undoTimer == 0f)
            {
                audio.clip = undo;
                audio.Play();
                Entities.undo();
                lossText.SetActive(false);
            }
            
            undoTimer += Time.deltaTime;
        }

        if (!undoDown || undoTimer >= 0.25f)
        {
            undoTimer = 0f;
        }
    }

    private void move(Vector3 direction)
    {
        Vector3 newPos = transform.position + direction;

        foreach (CompositeCollider2D wall in Entities.walls)
        {
            if (wall.bounds.Contains(newPos))
            {
                return;
            }
        }

        Entities.record();

        WoodenBox.Float();

        bool doWater = true;

        foreach (Box box in Entities.boxes)
        {
            if (newPos == box.transform.position)
            {
                if(!box.walkable && !box.Move(direction))
                {
                    Entities.undo();
                    return;
                }
                else
                {
                    doWater = false;
                }
            }
        }

        transform.position = newPos;
        Darkness.checkLight();

        if (doWater)
        {
            foreach (CompositeCollider2D water in Entities.water)
            {
                if (water.bounds.Contains(newPos))
                {
                    lossText.SetActive(true);
                    return;
                }
            }
        }
            

        if (Darkness.inShadow(newPos))
        {
            lossText.SetActive(true);
            return;
        }

        audio.clip = step;
        audio.Play();
    }

    public void add()
    {
        positions.Push(transform.position);
    }

    public void pop()
    {
        if (positions.Count == 0)
        {
            return;
        }

        transform.position = positions.Pop();
    }
}
