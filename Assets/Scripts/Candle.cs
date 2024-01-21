using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candle : MonoBehaviour, Rewindable
{
    public bool isLit = true;

    public Transform spriteTransform;
    public SpriteRenderer sprite;

    Stack<Transform> parent = new Stack<Transform>();

    private void Awake()
    {
        gameObject.tag = "candle";
    }

    public void pickup(Transform parent)
    {
        transform.parent = parent;
        if(transform.position.y == spriteTransform.position.y)
        {
            spriteTransform.position += Vector3.up * 0.25f;
        }
        sprite.sortingOrder = 3;
    }

    public void drop()
    {
        transform.parent = null;
        if(transform.position.y != spriteTransform.position.y)
        {
            spriteTransform.position -= Vector3.up * 0.25f;
        }
        sprite.sortingOrder = 0;
    }

    public void lower()
    {
        if (transform.position.y != spriteTransform.position.y)
        {
            spriteTransform.position -= Vector3.up * 0.25f;
        }
    }

    public void add()
    {
        parent.Push(transform.parent);
    }

    public void pop()
    {
        if (parent.Count == 0)
        {
            return;
        }


        Transform prev = transform.parent;
        transform.parent = parent.Pop();

        if(prev == null && transform.parent != null)
        {
            pickup(transform.parent);
        }
        else if(transform.parent == null && prev != null)
        {
            drop();
        }
    }
}
