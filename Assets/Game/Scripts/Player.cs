using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float speed = 5;
    [SerializeField] private GameObject openChess; 

    private const int left = 0;
    private const int right = 1;
    private const int forward = 2;
    private const int back = 3;
    private const int stay = 4;

    private Stack <Collider> brickStack = new Stack<Collider>();

    private Vector3 previousMousePosition = new Vector3(0, 0, 0);
    private Vector3 rayShootPosition;
    private Vector3 groundPosition;

    private bool isMoving = false;
    private bool isFinish = false;

    void Start()
    {
        
    }
    
    void Update()
    {
        //Debug.Log(isMoving);

        if(isFinish) return;

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
            if (Vector3.Distance(transform.position, targetPosition) > 0.001f)
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
        rayShootPosition.y = 10;
        groundPosition = transform.position;
        groundPosition.y = -0.1f;
        groundPosition += VectorDirection(Direction);
        int countAbleBrick = 0;

        while (!Physics.Raycast(rayShootPosition, groundPosition - rayShootPosition, Mathf.Infinity, wallLayer) && countAbleBrick<100)
        {
            countAbleBrick++;
            groundPosition += VectorDirection(Direction);
            rayShootPosition += VectorDirection(Direction);
            Debug.DrawLine(rayShootPosition, groundPosition, Color.red,3f);
            
        }


        targetPosition = transform.position + VectorDirection(Direction) * countAbleBrick;
        //if(countAbleBrick!=0)
        //{
        //    Debug.Log(countAbleBrick);
        //}

        //Debug.Log(brickStack.Count);
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
    IEnumerator WaitForNextSence(float durationTime)
    {
        yield return new WaitForSeconds(durationTime);
        FindObjectOfType<GameManager>().NextLevel();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Brick")
        {
            other.transform.SetParent(transform);
            transform.position += Vector3.up * 0.3f;
            targetPosition.y = transform.position.y;
            brickStack.Push(other);
            other.transform.position = transform.position - Vector3.up * 0.3f * brickStack.Count;
            other.gameObject.tag = "Player";
            other.gameObject.GetComponent<Collider>().isTrigger = false;
            other.gameObject.layer = LayerMask.NameToLayer("Default");
            
        }

        if(other.gameObject.tag=="UnBrick")
        {
            if(brickStack.Count == 0)
            {
                isFinish = true;
                targetPosition = transform.position;
                FindObjectOfType<GameManager>().GameOver();
                return;
            }
            transform.position -= Vector3.up * 0.3f;
            targetPosition.y = transform.position.y;
            Destroy(brickStack.Peek().gameObject);
            brickStack.Pop();
            other.gameObject.tag = "Untagged";
        }

        if (other.gameObject.tag == "Finish")
        {
            while(brickStack.Count > 1)
            {
                transform.position -= Vector3.up * 0.3f;
                targetPosition.y = transform.position.y;
                Destroy(brickStack.Peek().gameObject);
                brickStack.Pop();
            }
            isFinish = true;
            targetPosition = transform.position;

            StartCoroutine(WaitForNextSence(3f));
        }
    }
}
