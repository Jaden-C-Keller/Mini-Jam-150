using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, Rewindable
{
    public Transform tp1, tp2;
    public Transform cam;
    public Transform startingCam;
    public GameObject lossText;
    public GameObject winText;
    public Transform win;
    public new AudioSource audio;
    public AudioClip step, drag, undo, fall;

    Stack<Vector3> positions = new Stack<Vector3>();
    float timer = 0f;
    float undoTimer = 0f;
    float camTimer = 0f;
    Vector3 camTarget;
    Vector3 camPos;
    Vector3 offset;

    private void Awake()
    {
        offset = Vector3.back * 10f;
        Entities.rewindables.Add(this);
        camTarget = startingCam.position + offset;
        cam.position = camTarget;
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

        if(!lossText.activeSelf && !winText.activeSelf && !undoDown && timer == 0f && direction != Vector3.zero)
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
            if(Entities.step > 0 && undoTimer == 0f && !winText.activeSelf)
            {
                audio.clip = undo;
                audio.Play();
                Entities.undo();
                checkCam();
                lossText.SetActive(false);
            }
            
            undoTimer += Time.deltaTime;
        }

        if (!undoDown || undoTimer >= 0.25f)
        {
            undoTimer = 0f;
        }

        if(camTarget != cam.position)
        {
            cam.position = Vector3.Lerp(camPos, camTarget, camTimer);
            camTimer += Time.deltaTime * 0.8f;
        }
    }

    private void move(Vector3 direction)
    {
        Vector3 newPos = transform.position + direction;

        foreach (BoxCollider2D wall in Entities.walls)
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
            foreach (BoxCollider2D water in Entities.water)
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

        if (transform.position == win.position)
        {
            winText.SetActive(true);
        }

        audio.clip = step;
        

        if(transform.position == tp1.position)
        {
            transform.position = tp2.position;
            checkCam();
            cam.position = camTarget;
            Entities.step = 0;
            audio.clip = fall;
        }

        audio.Play();

        checkCam();
    }

    void checkCam()
    {
        foreach (BoxCollider2D room in Entities.rooms)
        {
            if (room.bounds.Contains(transform.position))
            {
                if (room.transform.position + offset != camTarget)
                {
                    camTarget = room.transform.position + offset;
                    camPos = cam.position;
                    camTimer = 0f;
                }
            }
        }
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
