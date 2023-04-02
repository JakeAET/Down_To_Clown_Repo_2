using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npcManager : MonoBehaviour
{
    public static npcManager Instance;

    [SerializeField] int npcSpawnCount;
    [SerializeField] GameObject npc;

    private float maxWalkX = 60;
    private float minWalkX = -40;
    private float maxWalkY = 20f;
    private float minWalkY = -20f;

    public List<GameObject> npcList;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        spawnNPCs(npcSpawnCount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void spawnNPCs(int num)
    {
        int i = 0;
        while(i < num)
        {
            float randSize = Random.Range(0.15f, 0.25f);
            float randX = Random.Range(minWalkX, maxWalkX);
            float randY = Random.Range(minWalkY, maxWalkY);
            Vector2 spawnPos = new Vector2(randX, randY);

            var newNPC = Instantiate(npc, spawnPos, Quaternion.identity);
            newNPC.name = "npc_" + i;
            newNPC.transform.parent = gameObject.transform;
            newNPC.transform.localScale = new Vector3 (randSize, randSize, randSize);
            npcList.Add(newNPC);
            i++;
        }
    }

    public void selectForAttraction(GameObject attraction)
    {
        bool i = true;

        while (i)
        {
            GameObject targetNPC = npcList[Random.Range(0, npcList.Count)];

            if (!targetNPC.GetComponent<npcBehavior>().atAttraction)
            {
                targetNPC.GetComponent<npcBehavior>().sendToAttraction(attraction.GetComponent<attractionBehavior>().attractionWalkPoint);
                attraction.GetComponent<attractionBehavior>().activeNPC = targetNPC;
                i = false;
            }

        }
    }
}
