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

// We attach this script to anything that we want to interact with.
// Our raycaster looks for this script so it knows what to do if we click on it.
// The interactiveType helps helps determine the fate of an interacted on object.
// for example collectable is set for items we can pick up while defaultInteractable is for items such as locks.
// Non collectables have a requirement. If that requirement is met or not, the game can respond accordingly.

public class InteractableScript : MonoBehaviour
{
    public enum InteractableType
    {
        defaultInteractable,
        collectable,
        multiPartPuzzle,
        strengthInteractable
    };

    public InteractableType interactableType;
    public ReactionScript myReactionScript;
    public GameManager gameManager;
    public string requirement;
    public Inventory inventory;

    [Header("For Multi Part Puzzles")]
    public List<int> requirements;

    public List<int> playerGuess = new List<int>();

    [Header("For Strength Interactables")]
    public float effortMeter = 0.0f;

    public bool continueCoroutine = true;

    public float EffortMeter
    {
        get
        {
            return effortMeter;
        }
        set
        {
            if (value > 30)
            {
                effortMeter = 31;
            }
            else
            {
                effortMeter = value;
                immovableObject.transform.eulerAngles = new Vector3(0.0f + (value * 4), -90.0f, 0.0f);
            }
        }
    }

    public GameObject immovableObject;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();

        if (interactableType == InteractableType.strengthInteractable)
        {
            StartCoroutine(AddResistance());
        }
    }

    public void Interaction()
    {
        switch (interactableType)
        {
            case InteractableType.collectable:
                gameManager.CollectItem(gameObject.name);
                Destroy(gameObject, 0.1f);
                break;

            case InteractableType.multiPartPuzzle:
                if (ListsEqual(requirements, playerGuess))
                {
                    myReactionScript.React();
                }
                break;

            case InteractableType.strengthInteractable:
                EffortMeter++;
                if (effortMeter == 1)
                {
                    gameManager.PublishSubtitle("The lever is stiff! You will need to put some effort into this!");
                }
                else if (effortMeter == 10)
                {
                    gameManager.PublishSubtitle("Nice work! Keep those fingers pumping!");
                }
                else if (effortMeter == 20)
                {
                    gameManager.PublishSubtitle("Almost there!");
                }
                else if (effortMeter == 30)
                {
                    continueCoroutine = false;
                    myReactionScript.React();
                    gameManager.PublishSubtitle("Done, and I didn't even break a sweat!");
                    EffortMeter = 31.0f;
                }

                break;

            case InteractableType.defaultInteractable:
                if (gameManager.heldItem != null && gameManager.heldItem.itemLabel == requirement)
                {
                    myReactionScript.React();
                    inventory.RemoveItem(gameManager.heldItem);
                    gameManager.heldItem = null;
                    Destroy(gameManager.heldGameObject);
                }
                else
                {
                    gameManager.PublishSubtitle("Hmmm, looks like we need a " + requirement + "! (Press '" + gameManager.inventoryKeyCode.ToString() + "' to open your inventory.)");
                }
                break;

            default:
                print("Enum not set for: " + gameObject.name);
                break;
        }
    }

    private bool ListsEqual(List<int> requirements, List<int> playerGuess)
    {
        if (requirements.Count != playerGuess.Count)
        {
            return false;
        }
        for (var i = 0; i < requirements.Count; i++)
        {
            if (requirements[i] != playerGuess[i])
            {
                return false;
            }
        }
        return true;
    }

    private IEnumerator AddResistance()
    {
        while (continueCoroutine)
        {
            yield return new WaitForSeconds(1);
            if (effortMeter > 0)
            {
                EffortMeter--;
            }
        }
    }
}