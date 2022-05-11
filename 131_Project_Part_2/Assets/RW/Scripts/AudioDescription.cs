using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDescription : MonoBehaviour
{
    private SettingsManager settingsManager;
    private AudioSource myAudioSource;
    public string myDescription;
    public bool descriptionSent = false;

    // Start is called before the first frame update
    void Start()
    {
        settingsManager = GameObject.FindGameObjectWithTag("SettingsManager").GetComponent<SettingsManager>();
        myAudioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (myAudioSource.isPlaying && descriptionSent == false)
        {
            settingsManager.HandleAudioDescription(myDescription, gameObject);
            descriptionSent = true;
            StartCoroutine(ResetReportingDescription(myAudioSource.clip.length));
        }
    }

    private IEnumerator ResetReportingDescription(float clipLength)
    {
        yield return new WaitForSeconds(clipLength + 0.1f);
        descriptionSent = false;
    }

}
