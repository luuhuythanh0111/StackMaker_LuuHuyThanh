using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private List<GameObject> teleportGameObject = new List<GameObject>();
    [SerializeField] private GameObject MainCamera;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask unBrickLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float speed = 5;
    
    private const int left = 0;
    private const int right = 1;
    private const int forward = 2;
    private const int back = 3;
    private const int stay = 4;


    private Stack <Collider> brickStack = new Stack<Collider>();

    private Vector3 previousMousePosition = new Vector3(0, 0, 0);
    private Vector3 rayShootPosition;
    private Vector3 groundPosition;

    private List<Vector3> UsedBridgePosition = new List<Vector3>();
    private bool isMoving = false;
    private bool isFinish = false;
    private bool isMovingOnBridge = false;
    private bool isMoveingAfterBridge = true;
    private bool isTeleport = false;
    private bool isPressedReturn = false;

    private int indexTeleport = 0;
    private int Direction = 4;
    private int score = 0;

    private void Start()
    {
        UIManager.instance.SetScore(score);
    }

    IEnumerator WaitForStop()
    {
        transform.position = targetPosition;
        yield return new WaitForSeconds(0.1f);
        isMoving = false;
    }
    void Update()
    {
        if(isFinish) return;

        if(isPressedReturn == false)
        { 
            if (isTeleport)
            {
                if (TeleportMove())
                {
                    return;
                }
            }
        }

        if(isMovingOnBridge)
        {
            MoveOnBridge();
            return;
        }

        if(isMoveingAfterBridge==false)
        {
            Move(Direction);
            isMoveingAfterBridge = true;
        }


        if (isMoving == false)
        {
            int getDirection = GetMouseDirection();

            if (getDirection != 4)
            {
                Direction = getDirection;
                isMoving = Move(Direction);
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                isPressedReturn = false;
            }
            else
            {
                //StartCoroutine(WaitForStop());
                transform.position = targetPosition;
                isMoving = false;
            }
        }
    }

    Vector3 VectorDirection(int Direct)
    {
        if(Direct==left)
        {
            return Vector3.left;
        }
        else if(Direct==right)
        {
            return Vector3.right;
        }
        else if(Direct== forward)
        {
            return Vector3.forward;
        }
        else if(Direct== back)
        {
            return Vector3.back;
        }
        return Vector3.zero;
    }

    bool Move(int Direction)
    {
        rayShootPosition = transform.position;
        rayShootPosition.y = 7;
        groundPosition = transform.position;
        groundPosition.y = -0f;
        groundPosition += VectorDirection(Direction);
        int countAbleBrick = 0;

        while (!Physics.Raycast(rayShootPosition, groundPosition - rayShootPosition, Mathf.Infinity, wallLayer) && countAbleBrick<100)
        {
            countAbleBrick++;
            groundPosition += VectorDirection(Direction);
            rayShootPosition += VectorDirection(Direction);
            Debug.DrawLine(rayShootPosition, groundPosition, Color.red,3f);
            if (Physics.Raycast(rayShootPosition, groundPosition - rayShootPosition, Mathf.Infinity, unBrickLayer))
            {
                isMovingOnBridge = true;
                //UsedBridgePosition.Add(groundPosition);
                Debug.DrawLine(rayShootPosition, groundPosition, Color.white, 5f);
                break;
            }
        }

        targetPosition = transform.position + VectorDirection(Direction) * countAbleBrick;
        return countAbleBrick != 0;
    }

    bool CheckInList(Vector3 position)
    {
        for(int i=0; i<UsedBridgePosition.Count; i++)
        {
            if (Vector3.Distance(UsedBridgePosition[i], position) < 0.01f) 
                return true;
        }

        return false;
    }

    void MoveOnBridge()
    {
        if (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition,2f * speed * Time.deltaTime);
            return;
        }

        transform.position = targetPosition;

        for (int i = 0; i<4; i++)
        {
            rayShootPosition = transform.position;
            rayShootPosition.y = 10;
            groundPosition = transform.position;
            groundPosition.y = -0f;

            groundPosition += VectorDirection(i);

            if (CheckInList(groundPosition))
            {
                groundPosition -= VectorDirection(i);
                continue;
            }

            if(Physics.Raycast(rayShootPosition, groundPosition - rayShootPosition, Mathf.Infinity, unBrickLayer))
            {
                Debug.DrawLine(rayShootPosition, groundPosition, Color.black, 5f);
                targetPosition = transform.position + VectorDirection(i);
                UsedBridgePosition.Add(groundPosition);
                return;
            }

        }

        for (int i = 0; i < 4; i++)
        {
            rayShootPosition = transform.position;
            rayShootPosition.y = 10;
            groundPosition = transform.position;
            groundPosition.y = -0f;

            groundPosition += VectorDirection(i);
            Debug.DrawLine(rayShootPosition, groundPosition, Color.green, 5f);
            if (Physics.Raycast(rayShootPosition, groundPosition - rayShootPosition, Mathf.Infinity, groundLayer))
            {
                
                targetPosition = transform.position + VectorDirection(i);
                isMovingOnBridge = false;
                UsedBridgePosition.Clear();
                Direction = i;
                isMoveingAfterBridge = false;
                //Debug.Log("Di vao ground");
                return;
            }

            groundPosition -= VectorDirection(i);
        }

    }

    private int GetMouseDirection()
    {
       
        if (Input.GetMouseButtonDown(0))
        {
            previousMousePosition = Input.mousePosition;
        }

        Vector3 currentMousePosition = new Vector3();
        if (Input.GetMouseButtonUp(0))
        {
            currentMousePosition = Input.mousePosition;
        }

        if (previousMousePosition.x == 0.0f)
        {
            return 4;
        }

        if (currentMousePosition.x == 0.0f)
        { 
            return 4;
        }

        float denta_X = currentMousePosition.x - previousMousePosition.x;
        float denta_Y = currentMousePosition.y - previousMousePosition.y;
        
        if (Mathf.Abs(denta_X) <= 20f && Mathf.Abs(denta_Y) <= 20f)
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

    IEnumerator EndLevel(float durationTime)
    {
        yield return new WaitForSeconds(durationTime);
        UIManager.instance.SetLastScore(score);
        FindObjectOfType<GameManager>().LoadingNextLevel();
    }

    private bool TeleportMove()
    {
        if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            return false;
        if (Input.GetMouseButtonDown(0))
        {
            indexTeleport++;
            if (indexTeleport > teleportGameObject.Count-1)
            {
                indexTeleport = 0;
            }
            MainCamera.GetComponent<CameraFollow>().target = teleportGameObject[indexTeleport].transform;
            return true;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            transform.position = new Vector3(teleportGameObject[indexTeleport].transform.position.x,
                                             transform.position.y,
                                             teleportGameObject[indexTeleport].transform.position.z);
            targetPosition = transform.position;
            isMoving = false;
            isTeleport = false;
            isPressedReturn = true;
            return true;
        }

        return true;
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
            score++;
            UIManager.instance.SetScore(score);
            
        }

        if(other.gameObject.tag == "UnBrick")
        {
            other.gameObject.tag = "Untagged";
            if (brickStack.Count == 0)
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

            StartCoroutine(EndLevel(1f));
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Push")
        {
            isMoving = true;
            if (Vector3.Distance(transform.position, targetPosition) < 0.1)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (i == left && Direction == right) continue;
                    if (i == right && Direction == left) continue;
                    if (i == forward && Direction == back) continue;
                    if (i == back && Direction == forward) continue;

                    if (Move(i))
                    {
                        Direction = i;
                        return;
                    }
                }
            }
        }

        if (other.gameObject.tag == "Teleport")
        {
            if (Vector3.Distance(transform.position, targetPosition) < 0.1)
            {
                Debug.Log(other.gameObject.tag);
                isTeleport = true;
            }

        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Teleport")
        {
            isTeleport = false;
        }
    }
}
