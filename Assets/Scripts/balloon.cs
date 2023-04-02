using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class balloon : MonoBehaviour
{
    private currencyManager currMang;
    private tutorialManager tutorialMang;

    [SerializeField] float moveSpeed = 5f;

    [SerializeField] float frequency = 20f;

    [SerializeField] float magnitude = 0.5f;

    private bool facingRight = true;

    Vector3 pos, localScale;

    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;

        localScale = transform.localScale;

        currMang = GameObject.FindGameObjectWithTag("currManager").GetComponent<currencyManager>();
        tutorialMang = GameObject.FindGameObjectWithTag("tutorialManager").GetComponent<tutorialManager>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckWhereToFace();

        if (facingRight)
        {
            MoveRight();
        }
        else
        {
            MoveLeft();
        }
    }

    void CheckWhereToFace()
    {
        if (pos.x < -70f)
        {
            facingRight = true;
        }
        else if (pos.x > 70f)
        {
            facingRight = false;
        }

        if (((facingRight) && (localScale.x < 0)) || ((!facingRight) && (localScale.x > 0)))
        {
            localScale.x *= -1;
        }

        transform.localScale = localScale;
    }

    void MoveRight()
    {
        pos += transform.right * Time.deltaTime * moveSpeed;
        transform.position = pos + transform.up * Mathf.Sin(Time.time * frequency) * magnitude;
    }

    void MoveLeft()
    {
        pos -= transform.right * Time.deltaTime * moveSpeed;
        transform.position = pos + transform.up * Mathf.Sin(Time.time * frequency) * magnitude;
    }

    public void popped()
    {
        if (tutorialMang.step == tutorialManager.tutorial.Clicker2 && !tutorialMang.balloonPopped)
        {
            currMang.whimsyIncrease(100);
            tutorialMang.balloonPop();
        }
        else
        {

            //Debug.Log("Balloon Popped");

            // Drop Outcome

            int randNum = Random.Range(1, 101);

            //Debug.Log(randNum);

            if (randNum >= 96 && randNum <= 100)
            {
                currMang.whimsyIncrease(50);
                Debug.Log("+ 30 Whimsy");
            }
            else if (randNum >= 81 && randNum <= 95)
            {
                currMang.multiplyerActive(3);
                Debug.Log("3x Ticket Multiplyer");
            }
            else if (randNum >= 61 && randNum <= 80)
            {
                currMang.whimsyIncrease(20);
                Debug.Log("+ 20 Whimsy");
            }
            else if (randNum >= 31 && randNum <= 60)
            {
                currMang.multiplyerActive(2);
                Debug.Log("2x Ticket Multiplyer");
            }
            else if (randNum >= 1 && randNum <= 30)
            {
                currMang.whimsyIncrease(10);
                Debug.Log("+ 10 Whimsy");
            }
        }

        // Spawn pop effect

        Destroy(gameObject);
    }

}
