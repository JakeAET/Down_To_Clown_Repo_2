using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragging : MonoBehaviour
{
    public bool isDragging = false;
    private Vector2 screenPos;
    private Vector3 worldPos;
    public GameObject lastDragged;
    public GameObject currentDragged;

    public static Dragging Instance;

    private tutorialManager tutorialMang;

    private void Awake()
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

    private void Start()
    {
        tutorialMang = GameObject.FindGameObjectWithTag("tutorialManager").GetComponent<tutorialManager>();
    }

    void Update()
    {
        if(isDragging && (Input.GetMouseButtonUp(0)) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            Drop();
            return;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            screenPos = new Vector2(mousePos.x, mousePos.y);
            if (lastDragged != null)
            {
                lastDragged.GetComponent<clownBehavior>().isHighlighted = false;
                lastDragged = null;
            }
        }
        else if(Input.touchCount > 0)
        {
            screenPos = Input.GetTouch(0).position;

            if(lastDragged != null)
            {
                lastDragged.GetComponent<clownBehavior>().isHighlighted = false;
                lastDragged = null;
            }
        }
        else
        {
            return;
        }

        worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        if (isDragging)
        {
            Drag();
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
            if(hit.collider != null)
            {
                GameObject draggedObj = hit.transform.gameObject;
                if(draggedObj.tag == "clown")
                {
                    //Debug.Log(draggedObj.name + " has been selected");
                    currentDragged = draggedObj;
                    draggedObj.GetComponent<clownBehavior>().dragStart();
                    startDrag();

                    if(tutorialMang.step == tutorialManager.tutorial.Working && !tutorialMang.clownDragged)
                    {
                        tutorialMang.clownDrag();
                    }
                }
                else if(draggedObj.tag == "balloon")
                {
                    balloon thisBalloon = draggedObj.GetComponent<balloon>();
                    thisBalloon.popped();
                }
            }
        }
    }

    void startDrag()
    {
        isDragging = true;
        UpdateDragStatus(true);
        FindObjectOfType<audioManager>().Play("honk");
        FindObjectOfType<audioManager>().Play("wiggle");
    }

    void Drag()
    {
        currentDragged.transform.position = new Vector2(worldPos.x, worldPos.y);
    }

    void Drop()
    {
        UpdateDragStatus(false);
        lastDragged = currentDragged;
        currentDragged = null;
        isDragging = false;
        FindObjectOfType<audioManager>().Pause("wiggle");

    }

    void UpdateDragStatus(bool dragActive)
    {
        if(currentDragged != null)
        {
            isDragging = currentDragged.GetComponent<clownBehavior>().dragActive = dragActive;
            //currentDragged.gameObject.layer = dragActive ? Layer.Dragging : Layer.Default;

            if (!dragActive)
            {
                currentDragged.GetComponent<clownBehavior>().dragEnd();
            }
        }
    }
}
