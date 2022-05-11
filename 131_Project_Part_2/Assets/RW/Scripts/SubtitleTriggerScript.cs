using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubtitleTriggerScript : MonoBehaviour
{
    private GameManager gameManager;
    private bool clueWasTriggered = false;
    public string clueText;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !clueWasTriggered)
        {
            gameManager.PublishSubtitle(clueText, Color.cyan);
            clueWasTriggered = true;
        }
    }
}
