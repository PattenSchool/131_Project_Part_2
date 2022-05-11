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

// This script checks to see of an object is infront of the player and close enough to be interacted with

public class CrosshairRay : MonoBehaviour
{
    public GameObject cursorGameObject;
    public GameObject interactionCursorGameObject;
    public bool isHoldButtonEnabled;
    public float rayDistance = 3.0f;
    public bool isInteractableHit;
    public InteractableScript currentInteractable;
    public GameManager gameManager;
    public Inventory inventory;

    private void Update()
    {
        RaycastHit hit;
        // If the inventory screen is open, we do not want to check for and 'activate' interactibles when the mouse is clicked or another 'Fire 1' control is pressed
        if (gameManager.inventoryScreenIsOpen)
        {
            return;
        }
        // The 'Ray' is cast infront of the player. If it hits an object and that object has an Interactable Script attached we capture a reference to it.
        if (Physics.Raycast(transform.position, transform.forward, out hit, rayDistance) && hit.transform.gameObject.GetComponent<InteractableScript>() != null)
        {
            currentInteractable = hit.transform.gameObject.GetComponent<InteractableScript>();
            isInteractableHit = true;
            cursorGameObject.SetActive(false);
            interactionCursorGameObject.SetActive(true);
        }
        else
        {
            currentInteractable = null;
            isInteractableHit = false;
            cursorGameObject.SetActive(true);
            interactionCursorGameObject.SetActive(false);
        }
        // There are a few scenarios we need to react to
        // The outcome is denpendant on whether we are in range of an interactable, type of interactable, whether we are currently holding an object.
        // 1 We are currently holding an object and not near an interactable: Drop the object.
        if (Input.GetButtonDown("Fire1") && !isInteractableHit || Input.GetButtonDown("Fire1") && currentInteractable == null)
        {
            if (gameManager.heldGameObject != null)
            {
                HeldScript heldObjectScript = gameManager.heldGameObject.gameObject.GetComponent<HeldScript>();
                heldObjectScript.enabled = false;
                gameManager.heldGameObject = null;
                inventory.RemoveItem(gameManager.heldItem);
                gameManager.heldItem = null;
            }
        }
        // 2 we are in range of an interactable and "Fire1" has been pressed: Interact!
        else if (!isHoldButtonEnabled && Input.GetButtonDown("Fire1") && isInteractableHit)
        {
            currentInteractable.Interaction();
        }
        // 3 If the player has enabled the button hold and the character is in range of a "strength" interactable, 
        // rather than use GetButtonDown, which is only true for the first frame, 
        // you use GetButton, which will remain true for the entire time the player holds the button.

        else if (isHoldButtonEnabled
            && isInteractableHit
            && currentInteractable.interactableType
               == InteractableScript.InteractableType.strengthInteractable
            && Input.GetButton("Fire1"))
        {
            currentInteractable.Interaction();
        }
        else if (isHoldButtonEnabled
            && isInteractableHit
            && currentInteractable.interactableType
               != InteractableScript.InteractableType.strengthInteractable
            && Input.GetButtonDown("Fire1"))
        {
            currentInteractable.Interaction();
        }

    }
}