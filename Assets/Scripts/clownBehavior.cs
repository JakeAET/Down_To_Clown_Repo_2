using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class clownBehavior : MonoBehaviour
{
    [SerializeField] string clownName;
    public Clown thisClown;
    private infoManager infoMang;
    private attractionManager attractionMang;

    //Movement States
    public enum moveState
    {
        Init, // default state
        Idle, // clown pauses when walking in field
        Walk, // clown walks in the field
        Hide, // clown hides in the field
        Tapped, // clown pauses until untapped, caused by quick tap to clown's sprite
        Grabbed, // clown moves with finger, caused by a held tap to clown's sprite
        Dropped, // transition state
        Working, // clown is at attraction
        Summoned, // clown is being summoned
    }
    public moveState state;

    //Movement
    private float moveSpeed;
    public Vector2 direction;

    //Animation
    private SpriteRenderer spriteRendererBack;
    private SpriteRenderer spriteRendererFront;
    //private SpriteRenderer spriteRendererWorking;
    [SerializeField] GameObject highlight;
    public bool isHighlighted = false;
    [SerializeField] GameObject backView;
    public GameObject frontView;
    //[SerializeField] GameObject workingView;
    public SpriteRenderer activeSprite;
    private bool backAnim = true;
    private bool frontAnim = false;
    //private bool workingAnim = false;
    [SerializeField] bool check = false;

    //Timers
    private bool setWalkTime = false;
    private float walkTimer;
    private float walkTime;
    private float idleTimer;
    [SerializeField] float idleTime; // DONT PRIVATE

    //Dragging
    private Collider2D col;
    private Dragging dragControl;
    public bool dragActive;
    public float dragTimer;
    private bool dropped = false;
    public bool canDrop = true;

    public bool isWorking = false;
    private Vector3 attractionPos;
    public bool overOpenAttraction;
    public GameObject lastHoveredAttraction;
    public GameObject currentAttraction;

    //Walk System
    private waypointManager waypointMang;
    public Vector3 currentTargetPos;
    public GameObject currentTarget;
    public bool moveX = false;
    public bool moveY = false;

    //Walk bounds
    private float maxWalkX = 60;
    private float minWalkX = -40;
    private float maxWalkY = 20f;
    private float minWalkY = -20f;

    //Summoning
    public bool summoning = false;
    private bool summonInit = true;
    public bool walkToPath = false;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        dragControl = FindObjectOfType<Dragging>();

        infoMang = GameObject.FindGameObjectWithTag("infoManager").GetComponent<infoManager>();
        waypointMang = GameObject.FindGameObjectWithTag("waypointManager").GetComponent<waypointManager>();
        attractionMang = GameObject.FindGameObjectWithTag("attractionManager").GetComponent<attractionManager>();

        foreach (Clown c in infoMang.clowns)
        {
            if (c.name == clownName)
            {
                thisClown = c;
            }
        }
        attractionPos = thisClown.targetAttraction;
    }

    // Start is called before the first frame update
    void Start()
    {
        spriteRendererBack = backView.GetComponent<SpriteRenderer>();
        spriteRendererFront = frontView.GetComponent<SpriteRenderer>();
        //spriteRendererWorking = workingView.GetComponent<SpriteRenderer>();

        state = moveState.Init;

        //Setup sprite 
        if (backAnim)
        {
            activeSprite = spriteRendererBack;
        }
        if (frontAnim)
        {
            activeSprite = spriteRendererFront;
        }
        //if (workingAnim)
        {
            //activeSprite = spriteRendererWorking;
        }

        moveSpeed = Random.Range(4, 6);
    }

    // Update is called once per frame
    void Update()
    {
        // Animation Control
        if (backAnim)
        {
            activeSprite = spriteRendererBack;
        }
        if (frontAnim)
        {
            activeSprite = spriteRendererFront;
        }
        //if (workingAnim)
        {
            //activeSprite = spriteRendererWorking;
        }

        highlight.SetActive(isHighlighted);
        backView.SetActive(backAnim);
        frontView.SetActive(frontAnim);
        //workingView.SetActive(workingAnim);

        if (dragActive)
        {
            dragTimer += Time.deltaTime;
            //isWorking = false;
        }

        if(state != moveState.Working && isWorking || thisClown.working)
        {
            //Debug.Log("stopped working from 'state != moveState.Working && isWorking || thisClown.working'");
            //stopWorking();
        }

        #region State Actions
        switch (state)
        {
            case moveState.Init:
                if (thisClown.working)
                {
                    foreach (GameObject a in attractionMang.attractionSprites)
                    {
                        if (thisClown.workingAttraction == a.name)
                        {
                            startWorking(a);
                        }
                    }
                }
                else
                {
                    isWorking = false;
                    float randX = Random.Range(minWalkX, maxWalkX);
                    float randY = Random.Range(minWalkY, maxWalkY);
                    gameObject.transform.position = new Vector2(randX, randY);

                    findTargetWaypoint();
                    snapToWalkPath();

                    state = moveState.Walk;
                }
                break;

            case moveState.Idle:

                if (setWalkTime) // if on, reset state
                {
                    setWalkTime = false;
                }

                directionAnim();

                if (frontView.GetComponent<SpriteRenderer>().sortingOrder == 4)
                {
                    frontView.GetComponent<SpriteRenderer>().sortingOrder = 10;
                }

                idleTimer += (Time.deltaTime); //start idleTimer
                walkTimer = 0; //reset walkTime
                break;

            case moveState.Walk:

                if (setWalkTime == false)
                {
                    walkTime = randTime(3, 8); // Walk Time Parameters
                    setWalkTime = true;
                }

                walkTimer += (Time.deltaTime); //start walkTimer

                if(moveX)
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector2(currentTargetPos.x, transform.position.y), moveSpeed * Time.deltaTime);

                    if (transform.position.x == currentTargetPos.x)
                    {
                        moveX = false;

                        if(transform.position.y != currentTargetPos.y)
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
                    findTargetWaypoint();
                }

                directionAnim();

                if (frontView.GetComponent<SpriteRenderer>().sortingOrder == 4)
                {
                    frontView.GetComponent<SpriteRenderer>().sortingOrder = 10;
                }

                idleTimer = 0; //reset idleTime

                break;

            case moveState.Hide:
                
                break;

            case moveState.Tapped:
                frontView.GetComponent<Animator>().SetBool("dragged", false);
                backAnim = false;
                frontAnim = true;
                break;

            case moveState.Grabbed:
                frontView.GetComponent<Animator>().SetBool("dragged", true);
                backAnim = false;
                frontAnim = true;
                if(isWorking)
                {
                    Debug.Log("stopped working from the Grab state");
                    stopWorking();
                    overOpenAttraction = true;
                }
                //workingAnim = false;
                break;

            case moveState.Dropped:
                currentAttraction = null;
                if (verifyOpenBooth())
                {
                    startWorking(lastHoveredAttraction);
                }
                else
                {
                    snapToWalkPath();
                    state = moveState.Idle;
                }
                break;

            case moveState.Working:
                frontView.GetComponent<Animator>().SetBool("dragged", false);
                if (gameObject.transform.position != attractionPos)
                {
                    gameObject.transform.position = attractionPos;
                }

                if(frontView.GetComponent<SpriteRenderer>().sortingOrder == 10)
                {
                    frontView.GetComponent<SpriteRenderer>().sortingOrder = 4;
                }
                isHighlighted = false;
                backAnim = false;
                frontAnim = true;
                //workingAnim = true;
                break;
            case moveState.Summoned:
                backAnim = false;
                frontAnim = true;

                if (summonInit)
                {
                    transform.position = new Vector3 (-2,53,0);
                    summonInit = false;
                }

                if (walkToPath)
                {
                    Vector3 walkTarget = new Vector2(10, 20);

                    if(transform.position != walkTarget)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, new Vector2(10, 20), moveSpeed * Time.deltaTime);
                    }
                    else
                    {
                        walkToPath = false;
                        summoning = false;
                    }
                }

                break;
        }
        #endregion

        determineState();
    }

    // Entering triggers
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "attraction")
        {
            if (!collision.gameObject.GetComponent<attractionBehavior>().inUse)
            {
                overOpenAttraction = true;
                lastHoveredAttraction = collision.gameObject;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "attraction" && state == moveState.Grabbed)
        {
            overOpenAttraction = false;
            //lastHoveredAttraction = null;
        }
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

        if(currentTarget != null)
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
        currentTargetPos = target.transform.position;
    }

    private void snapToWalkPath()
    {
        if (!overOpenAttraction)
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

            //Debug.Log("distance from x is: " + Mathf.Abs(Mathf.Abs(transform.position.x) - Mathf.Abs(closest.transform.position.x)) + " & distance from y is: " + Mathf.Abs(Mathf.Abs(transform.position.y) - Mathf.Abs(closest.transform.position.y)));

            if (Mathf.Abs(Mathf.Abs(transform.position.x) - Mathf.Abs(closest.transform.position.x)) < Mathf.Abs(Mathf.Abs(transform.position.y) - Mathf.Abs(closest.transform.position.y)))
            {
                transform.position = new Vector2(closest.transform.position.x, transform.position.y);
                //Debug.Log("snapped to x axis");
            }
            else if (Mathf.Abs(Mathf.Abs(transform.position.y) - Mathf.Abs(closest.transform.position.y)) < Mathf.Abs(Mathf.Abs(transform.position.x) - Mathf.Abs(closest.transform.position.x)))
            {
                transform.position = new Vector2(transform.position.x, closest.transform.position.y);
                //Debug.Log("snapped to y axis");
            }

            currentTarget = closest;
            currentTargetPos = closest.transform.position;

            //Debug.Log("snap to path completed");
        }
    }

    public void tappedAway()
    {
        isHighlighted = false;
    }

    public void startWorking(GameObject targetAttraction)
    {
        Debug.Log("Start Working");
        attractionBehavior a = targetAttraction.GetComponent<attractionBehavior>();
        a.clownWorking(this.gameObject, true);
        attractionPos = a.clownPos.transform.position;
        isWorking = true;
        dragActive = false;
        thisClown.working = true;
        thisClown.targetAttraction = a.clownPos.transform.position;
        thisClown.workingAttraction = a.name;
        currentAttraction = targetAttraction;
        state = moveState.Working;
        updateClownInfo();
    }

    public void stopWorking()
    {
        if (isWorking)
        {
            Debug.Log("Stop Working");
            attractionBehavior a = currentAttraction.GetComponent<attractionBehavior>();
            a.clownWorking(this.gameObject, false);
            attractionPos = Vector3.zero;
            thisClown.targetAttraction = Vector3.zero;
            thisClown.workingAttraction = "";
            thisClown.working = false;
            isWorking = false;
            updateClownInfo();
        }
    }

    public void dragStart()
    {
        dragActive = true;
    }

    public void dragEnd()
    {
        if(dragTimer <= 0.5f && isHighlighted == false && isWorking == false)
        {
            isHighlighted = true;
            //FindObjectOfType<audioManager>().Play("honk");
        }

        if(transform.position.y > maxWalkY)
        {
            transform.position = new Vector2(transform.position.x, maxWalkY);
        }
        else if(transform.position.y < minWalkY)
        {
            transform.position = new Vector2(transform.position.x, minWalkY);
        }

        if (transform.position.x > maxWalkX)
        {
            transform.position = new Vector2(maxWalkX, transform.position.y);
        }
        else if(transform.position.x < minWalkX)
        {
            transform.position = new Vector2(minWalkX, transform.position.y);
        }

        dragTimer = 0f;
        dropped = true;
        updateClownInfo();
        dragActive = false;
    }

    public bool verifyOpenBooth()
    {
        if(lastHoveredAttraction != null)
        {
            if (overOpenAttraction && lastHoveredAttraction.GetComponent<attractionBehavior>().inUse == false)
            {
                Debug.Log(lastHoveredAttraction.name + " not in use, returning true");
                return true;
            }
            else
            {
                Debug.Log(lastHoveredAttraction.name + " in use, returning false");
                return false;
            }
        }
        Debug.Log("no lastHovered");
        return false;
    }

    float randTime(float min, float max) // determine a random time within parameters
    {
        float rand = Random.Range(min, max);
        return rand;
    }

    void directionAnim()
    {
        frontView.GetComponent<Animator>().SetBool("dragged", false);
        if(currentTarget == null)
        {
            findTargetWaypoint();
        }
        if (transform.position.y < currentTarget.transform.position.y)
        {
            backAnim = true;
            frontAnim = false;
        }
        else
        {
            frontAnim = true;
            backAnim = false;
        }
    }

    void updateClownInfo()
    {
        int targetClown = 0;

        foreach (Clown c in infoMang.clowns)
        {
            if(c.name == thisClown.name)
            {
                infoMang.clowns[targetClown] = thisClown;
                return;
            }
            targetClown++;
        }
    }

    #region State Switch Conditions
    private void determineState()
    {
        switch (state)
        {
            case moveState.Init:
                if (isWorking)
                    state = moveState.Working;
                if (summoning)
                    state = moveState.Summoned;
                break;

            case moveState.Idle:
                if (idleTimer >= idleTime)
                    state = moveState.Walk;
                if (isHighlighted)
                    state = moveState.Tapped;
                if (dragActive)
                    state = moveState.Grabbed;
                if (isWorking)
                    state = moveState.Working;
                if (summoning)
                    state = moveState.Summoned;
                break;

            case moveState.Walk:
                if (walkTimer >= walkTime)
                    state = moveState.Idle;
                if (isHighlighted)
                    state = moveState.Tapped;
                if (dragActive)
                    state = moveState.Grabbed;
                if (isWorking)
                    state = moveState.Working;
                if (summoning)
                    state = moveState.Summoned;
                break;

            case moveState.Hide:
                
                break;

            case moveState.Tapped:
                if (dragActive)
                    state = moveState.Grabbed;
                if (isHighlighted == false)
                    state = moveState.Walk;
                break;

            case moveState.Grabbed:
                if (dragActive == false)
                    state = moveState.Dropped;
                break;
            case moveState.Dropped:

                break;

            case moveState.Working:
                if (dragActive)
                    state = moveState.Grabbed;
                break;
            case moveState.Summoned:
                if (!summoning)
                    state = moveState.Walk;
                break;
        }
    }
    #endregion
}
