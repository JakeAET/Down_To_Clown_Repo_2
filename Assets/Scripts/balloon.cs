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

    [SerializeField] GameObject balloonSprite;
    [SerializeField] TrailRenderer trail;

    [SerializeField] GameObject[] popOutput;
    private bool outputDone = false;


    private bool facingRight = true;

    Vector3 pos, localScale;

    private void Awake()
    {
        //Color color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f, 0.8f, 0.8f);
        //balloonSprite.GetComponent<SpriteRenderer>().material.color = color;
        //trail.startColor = color;
    }

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
            localScale.x *= 1;
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
        StartCoroutine(popRoutine());
    }

    IEnumerator popRoutine()
    {
        balloonSprite.SetActive(false);

        if (tutorialMang.step == tutorialManager.tutorial.Clicker2 && !tutorialMang.balloonPopped && !outputDone)
        {
            outputDone = true;
            popOutput[3].SetActive(true);
            popOutput[3].GetComponent<textEffect>().effectActive = true;
            currMang.whimsyIncrease(100);
            tutorialMang.balloonPop();

            yield return new WaitForSeconds(3);
            popOutput[3].GetComponent<textEffect>().reset();
            popOutput[3].SetActive(false);
            yield return null;
        }
        else
        {

            //Debug.Log("Balloon Popped");

            // Drop Outcome

            int randNum = Random.Range(1, 101);

            //Debug.Log(randNum);

            if (randNum >= 81 && randNum <= 100 && !outputDone)
            {
                outputDone = true;
                popOutput[2].SetActive(true);
                popOutput[2].GetComponent<textEffect>().effectActive = true;
                currMang.whimsyIncrease(50);
                Debug.Log("+ 50 Whimsy");

                yield return new WaitForSeconds(3);
                popOutput[2].GetComponent<textEffect>().reset();
                popOutput[2].SetActive(false);
                yield return null;

            }
            else if (randNum >= 31 && randNum <= 80 && !outputDone)
            {
                outputDone = true;
                popOutput[1].SetActive(true);
                popOutput[1].GetComponent<textEffect>().effectActive = true;
                currMang.whimsyIncrease(20);
                Debug.Log("+ 20 Whimsy");

                yield return new WaitForSeconds(3);
                popOutput[1].GetComponent<textEffect>().reset();
                popOutput[1].SetActive(false);
                yield return null;
            }
            else if (randNum >= 1 && randNum <= 30 && !outputDone)
            {
                outputDone = true;
                popOutput[0].SetActive(true);
                popOutput[0].GetComponent<textEffect>().effectActive = true;
                currMang.whimsyIncrease(10);
                Debug.Log("+ 10 Whimsy");

                yield return new WaitForSeconds(3);
                popOutput[0].GetComponent<textEffect>().reset();
                popOutput[0].SetActive(false);
                yield return null;
            }
        }
        // Spawn pop effect
        Destroy(gameObject, 3f);
    }

}
