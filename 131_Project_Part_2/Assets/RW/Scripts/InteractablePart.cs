/*
 * Copyright (c) 2019 Razeware LLC
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish,
 * distribute, sublicense, create a derivative work, and/or sell copies of the
 * Software in any work that is designed, intended, or marketed for pedagogical or
 * instructional purposes related to programming, coding, application development,
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works,
 * or sale is expressly withheld.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is for multipart puzzles of which there are two types.
// 1) Where more than one requirement needs to be met (for examples, the gem puzzle).
// 2) Where more than one requiremt needs to be met, and in the correct order (for example, the musical note puzzle).

public class InteractablePart : MonoBehaviour
{
    public bool isSequenceImportant;
    public bool persistentTriggerRequired;
    public int partNumber;
    public InteractableScript myMasterInteractable;
    public string requirement;
    private AudioSource interactionAudio;

    private GameObject triggeredObject = null;

    private void Start()
    {
        interactionAudio = gameObject.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //avoid double calls from double collider controller
        if (triggeredObject == other.gameObject)
        {
            return;
        }

        if (other.gameObject.tag == requirement)
        {
            triggeredObject = other.gameObject;
            if (!isSequenceImportant)
            {
                myMasterInteractable.playerGuess[partNumber] = 1;
                myMasterInteractable.Interaction();
                if (requirement.Contains("Gem"))
                {
                    Material gemMaterial = other.gameObject.GetComponentInChildren<Renderer>().material;
                    StartCoroutine(LerpEmission(0.0f, 1.0f, gemMaterial));
                }

            }
            else
            {
                interactionAudio.Play();
                myMasterInteractable.playerGuess.Insert(0, partNumber);
                myMasterInteractable.playerGuess.RemoveAt(5);
                myMasterInteractable.Interaction();
            }
        }
    }
    private IEnumerator LerpEmission(float startValue, float targetValue, Material gemMaterial)
    {
        float progress = 0;
        while (progress <= 1)
        {
            float emission = Mathf.Lerp(startValue, targetValue, progress);
            gemMaterial.SetColor("_EmissionColor", gemMaterial.color * emission);
            gemMaterial.EnableKeyword("_EMISSION");
            progress += Time.deltaTime;
            yield return null;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (persistentTriggerRequired)
        {
            if (other.gameObject.name == requirement)
            {
                if (!isSequenceImportant)
                {
                    myMasterInteractable.playerGuess[partNumber] = 0;
                    myMasterInteractable.Interaction();
                }
            }
        }
        triggeredObject = null;
    }
}