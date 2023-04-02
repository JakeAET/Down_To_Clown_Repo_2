using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waypointManager : MonoBehaviour
{
    public static waypointManager Instance;

    private GameObject[] waypoints;
    public List<GameObject> walkPoints;
    public List<GameObject> attractionPoints;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {

            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        waypoints = GameObject.FindGameObjectsWithTag("waypoint");

        foreach (GameObject w in waypoints)
        {
            if (w.GetComponent<waypoint>().isAttraction)
            {
                attractionPoints.Add(w);
            }
            else
            {
                walkPoints.Add(w);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
