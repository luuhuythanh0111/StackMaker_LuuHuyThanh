using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float speed = 5;

    private const int left = 0;
    private const int right = 1;
    private const int forward = 2;
    private const int back = 3;
    private const int stay = 4;

    private Stack <int> brickStack = new Stack<int>();

    private Vector3 previousMousePosition = new Vector3(0, 0, 0);
    private Vector3 rayShootPosition;
    private Vector3 groundPosition;

    private bool isMoving = false;


    void Start()
    {
        
    }
    
    void Update()
    {
        //Debug.Log(isMoving);

        if (isMoving == false)
        {
            int Direction = GetMouseDirection();
            if (Direction != 4)
            {
                isMoving = Move(Direction);
            }
        }
        else
        {
            //Debug.Log(targetPosition);
            //Debug.DrawLine(rayShootPosition, targetPosition, Color.red);
            //Debug.Log(Vector3.Distance(transform.position, targetPosition));
            //Debug.Log(targetPosition + "     " + transform.position);
            if (Vector3.Distance(transform.position, targetPosition) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            }
            else
            {
                isMoving = false;
            }
        }
    }

    Vector3 VectorDirection(int Direction)
    {
        if(Direction==left)
        {
            return Vector3.left;
        }
        else if(Direction==right)
        {
            return Vector3.right;
        }
        else if(Direction== forward)
        {
            return Vector3.forward;
        }
        else if(Direction== back)
        {
            return Vector3.back;
        }
        return Vector3.zero;
    }

    bool Move(int Direction)
    {
        rayShootPosition = transform.position;
        rayShootPosition.y = 2;
        groundPosition = transform.position;
        groundPosition.y = -0.1f;
        groundPosition += VectorDirection(Direction);
        int countAbleBrick = 0;

        while (!Physics.Raycast(rayShootPosition, groundPosition - rayShootPosition, Mathf.Infinity, wallLayer))
        {
            countAbleBrick++;
            groundPosition += VectorDirection(Direction);
            //Debug.DrawRay(rayShootPosition, groundPosition - rayShootPosition, Color.red);
            //Debug.Log(Physics.Raycast(rayShootPosition, groundPosition - rayShootPosition, Mathf.Infinity, wallLayer));
        }


        targetPosition = transform.position + VectorDirection(Direction) * countAbleBrick;
        if(countAbleBrick!=0)
        {
            Debug.Log(countAbleBrick);
        }
        
        return countAbleBrick != 0;
    }

    private int GetMouseDirection()
    {
        Vector3 currentMousePosition = Input.mousePosition;
        if (previousMousePosition.x == 0)
        {
            previousMousePosition = currentMousePosition;
            return 4;
        }

        float denta_X = currentMousePosition.x - previousMousePosition.x;
        float denta_Y = currentMousePosition.y - previousMousePosition.y;
        previousMousePosition = currentMousePosition;
        if (Mathf.Abs(denta_X) <= 15f && Mathf.Abs(denta_Y) <= 15f)
            return stay;
        if (Mathf.Abs(denta_X) > Mathf.Abs(denta_Y) + 0.1f)
        {// right or left
            if (denta_X < 0.00)
                return left;
            else
                return right;
        }
        else
        {// forward or back
            if (denta_Y < 0.00)
                return back;
            else
                return forward;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Brick")
        {
            other.transform.SetParent(transform);
            transform.position += Vector3.up * 0.25f;
            targetPosition.y = transform.position.y;
            other.transform.position = transform.position - Vector3.up * 0.25f * brickStack.Count;
            other.gameObject.tag = "Player";
            other.gameObject.GetComponent<Collider>().isTrigger = false;
            other.gameObject.layer = LayerMask.NameToLayer("Default");
            brickStack.Push(1);
        }
    }
}
