using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npcBehavior : MonoBehaviour
{
    private waypointManager waypointMang;
    private GameObject currentTarget;
    public Vector3 currentTargetPos;

    public bool atAttraction = false;

    public bool moveX = false;
    public bool moveY = false;

    //Timers
    private bool setWalkTime = false;
    private float walkTimer;
    private float walkTime;
    private float idleTimer;
    [SerializeField] float idleTime; // DONT PRIVATE

    private float moveSpeed;

    //Movement States
    public enum moveState
    {
        Idle, // clown pauses when walking in field
        Walk, // clown walks in the field
        Attraction, // clown is at attraction
    }
    public moveState state;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("starting pos is " + transform.position);
        moveSpeed = Random.Range(3, 5);
        snapToWalkPath();
        state = moveState.Walk;
    }

    // Update is called once per frame
    void Update()
    {
        #region State Actions
        switch (state)
        {
            case moveState.Idle:

                if (setWalkTime) // if on, reset state
                {
                    setWalkTime = false;
                }

                idleTimer += (Time.deltaTime); //start idleTimer
                walkTimer = 0; //reset walkTime
                break;

            case moveState.Walk:

                if (setWalkTime == false)
                {
                    walkTime = randTime(1, 20); // Walk Time Parameters
                    setWalkTime = true;
                }

                walkTimer += (Time.deltaTime); //start walkTimer
                //transform.Translate(direction * moveSpeed * Time.deltaTime); //movement
                if (moveX)
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector2(currentTargetPos.x, transform.position.y), moveSpeed * Time.deltaTime);

                    if (transform.position.x == currentTargetPos.x)
                    {
                        moveX = false;

                        if (transform.position.y != currentTargetPos.y)
                        {
                            moveY = true;
                        }
                    }
                }
                else if (moveY)
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector2(transform.position.x, currentTargetPos.y), moveSpeed * Time.deltaTime);

                    if (transform.position.y == currentTargetPos.y)
                    {
                        moveY = false;

                        if (transform.position.x != currentTargetPos.x)
                        {
                            moveX = true;
                        }
                    }
                }
                else if (!moveX && !moveY)
                {
                    //Debug.Log("waypoint reached");
                    findTargetWaypoint();
                }

                //transform.position = Vector2.MoveTowards(transform.position, currentTarget, moveSpeed * Time.deltaTime); //movement
                idleTimer = 0; //reset idleTime

                break;

            case moveState.Attraction:

                //transform.Translate(direction * moveSpeed * Time.deltaTime); //movement
                if (moveX)
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector2(currentTargetPos.x, transform.position.y), moveSpeed * Time.deltaTime);

                    if (transform.position.x == currentTargetPos.x)
                    {
                        moveX = false;

                        if (transform.position.y != currentTargetPos.y)
                        {
                            moveY = true;
                        }
                    }
                }
                else if (moveY)
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector2(transform.position.x, currentTargetPos.y), moveSpeed * Time.deltaTime);

                    if (transform.position.y == currentTargetPos.y)
                    {
                        moveY = false;

                        if (transform.position.x != currentTargetPos.x)
                        {
                            moveX = true;
                        }
                    }
                }

                //transform.position = Vector2.MoveTowards(transform.position, currentTarget, moveSpeed * Time.deltaTime); //movement
                idleTimer = 0; //reset idleTime
                walkTimer = 0; //reset walkTime

                break;
        }
        #endregion

        determineState();
    }

    private void findTargetWaypoint()
    {
        List<GameObject> gos = new List<GameObject>();
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("waypoint"))
        {
            if (!g.GetComponent<waypoint>().isAttraction)
            {
                gos.Add(g);
            }
        }

        if (currentTarget != null)
        {
            gos.Remove(currentTarget);
        }

        GameObject target = gos[Random.Range(0, gos.Count)];

        if (Mathf.Abs(Mathf.Abs(transform.position.x) - Mathf.Abs(target.transform.position.x)) > Mathf.Abs(Mathf.Abs(transform.position.y) - Mathf.Abs(target.transform.position.y)))
        {
            moveX = true;
        }
        else if (Mathf.Abs(Mathf.Abs(transform.position.y) - Mathf.Abs(target.transform.position.y)) > Mathf.Abs(Mathf.Abs(transform.position.x) - Mathf.Abs(target.transform.position.x)))
        {
            moveY = true;
        }

        currentTarget = target;
        currentTargetPos = offsetPos(target.transform.position);
    }

    private void snapToWalkPath()
    {
        List<GameObject> gos = new List<GameObject>();
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("waypoint"))
        {
            if (!g.GetComponent<waypoint>().isAttraction)
            {
                gos.Add(g);
            }
        }
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }

        //Debug.Log("distance from x is: " + Mathf.Abs(transform.position.x - closest.transform.position.x) + " & distance from y is: " + Mathf.Abs(transform.position.y - closest.transform.position.y));

        if (Mathf.Abs(Mathf.Abs(transform.position.x) - Mathf.Abs(closest.transform.position.x)) < Mathf.Abs(Mathf.Abs(transform.position.y) - Mathf.Abs(closest.transform.position.y)))
        {
            //Debug.Log(gameObject.name + " spawned at: " + transform.position + " with a target of: " + closest.transform.position + " and snapped to x = " + closest.transform.position.x);
            transform.position = offsetPos(new Vector3(closest.transform.position.x, transform.position.y, transform.position.z));
            moveY = true;
        }
        else if (Mathf.Abs(Mathf.Abs(transform.position.y) - Mathf.Abs(closest.transform.position.y)) < Mathf.Abs(Mathf.Abs(transform.position.x) - Mathf.Abs(closest.transform.position.x)))
        {
            //Debug.Log(gameObject.name + " spawned at: " + transform.position + " with a target of: " + closest.transform.position + " and snapped to y = " + closest.transform.position.y);
            transform.position = offsetPos(new Vector3(transform.position.x, closest.transform.position.y, transform.position.z));
            moveX = true;
        }

        currentTarget = closest;
        currentTargetPos = offsetPos(closest.transform.position);

        //Debug.Log("snap to path completed");
    }

    public void sendToAttraction(GameObject target)
    {
        currentTarget = target;
        currentTargetPos = (target.transform.position);
        atAttraction = true;
    }

    public void releaseFromAttraction()
    {
        atAttraction = false;
        snapToWalkPath();
        findTargetWaypoint();
    }

    float randTime(float min, float max) // determine a random time within parameters
    {
        float rand = Random.Range(min, max);
        return rand;
    }

    Vector3 offsetPos(Vector3 initialPos)
    {
        if (moveX)
        {
            float offset = Random.Range(0, 2f) + initialPos.y;
            return new Vector3(initialPos.x, offset, initialPos.z);
        }
        else if (moveY)
        {
            float offset = Random.Range(0, 2f) + initialPos.x;
            return new Vector3(offset, initialPos.y, initialPos.z);
        }
        else
        {
            return initialPos;
        }
    }

    #region State Switch Conditions
    private void determineState()
    {
        switch (state)
        {
            case moveState.Idle:
                if (idleTimer >= idleTime)
                    state = moveState.Walk;
                if (atAttraction)
                    state = moveState.Attraction;
                break;

            case moveState.Walk:
                if (walkTimer >= walkTime)
                    state = moveState.Idle;
                if (atAttraction)
                    state = moveState.Attraction;
                break;

            case moveState.Attraction:
                if (!atAttraction)
                    state = moveState.Walk;
                break;
        }
    }
    #endregion
}
