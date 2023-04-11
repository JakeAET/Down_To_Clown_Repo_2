using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class textEffect : MonoBehaviour
{
    [SerializeField] Text text;
    [SerializeField] Text text2;
    [SerializeField] Image image;
    private Color startColorT;
    private Color startColorT2;
    private Color startColorI;
    private Color colorT;
    private Color colorT2;
    private Color colorI;
    [SerializeField] float fadeSpeed;
    private float timer;
    public bool effectActive = false;


    // Start is called before the first frame update
    void Start()
    {
        startColorT = text.color;
        if(text2 != null)
        {
            startColorT2 = text2.color;
        }
        if(image != null)
        {
            startColorI = image.color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (effectActive)
        {
            timer += Time.deltaTime;

            colorT = text.color;
            colorT.a -= Time.deltaTime * fadeSpeed;
            text.color = colorT;

            if(text2 != null)
            {
                colorT2 = text2.color;
                colorT2.a -= Time.deltaTime * fadeSpeed;
                text2.color = colorT2;
            }

            if(image != null)
            {
                colorI = image.color;
                colorI.a -= Time.deltaTime * fadeSpeed;
                image.color = colorI;
            }

            if(colorT.a >= 0)
            {
                effectActive = false;
            }
        }
    }

    public void reset()
    {
        effectActive = false;
        text.color = startColorT;
        if(text2 != null)
        {
            text2.color = startColorT2;
        }
        if(image != null)
        {
            image.color = startColorI;
        }
    }
}
