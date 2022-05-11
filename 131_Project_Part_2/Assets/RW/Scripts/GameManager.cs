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
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

[System.Serializable]
public class Item
{
    public string itemLabel;
    public GameObject itemPrefab;
    public Sprite inventoryRepresentation;
    public string itemSubtitle;
}

public class GameManager : MonoBehaviour
{
    public Button startGameButton;
    public Button restartGameButton;
    public Button firstInventorySlot;

    public bool isRebinding = false;
    public GameObject character;
    public bool gameStarted = false;

    [Header("Collectables")]
    public List<Item> itemList;

    [Header("Inventory Variables")]
    public Inventory playerInventory;

    public GameObject inventorySettingsButton;
    public RectTransform inventoryRectTransform;
    private Vector2 inventoryOpenPosition = new Vector2(-70.0f, 0.0f);
    private Vector2 inventoryClosedPosition = new Vector2(70.0f, 0.0f);
    public Item heldItem;
    public GameObject heldGameObject;
    public bool inventoryScreenIsOpen = false;
    public float progress = 0;

    [Header("UI Stuff")]
    public GameObject menuBackground;

    public GameObject startMenu;
    public Image cursorImage;
    public GameObject subtitlePrefab;
    public GameObject subtitlePanel;
    public GameObject endMenu;

    [Header("Miscellaneous")]
    public KeyCode inventoryKeyCode = KeyCode.I;

    public bool isSettingsMenuOpen = false;
    public Animator startDoorAnimator;

    private void Start()
    {
        startGameButton.Select();
        menuBackground.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
    }

    private void Update()
    {
        if (gameStarted && !isRebinding && !isSettingsMenuOpen && Input.GetKeyDown(inventoryKeyCode))
        {
            ToggleInventoryScreen();
        }
    }

    public void CollectItem(string collectedItem)
    {
        foreach (Item item in itemList)
        {
            if (item.itemLabel == collectedItem)
            {
                playerInventory.AddItem(item);
                PublishSubtitle(item.itemSubtitle);
                //Item found and passed to Inventory
                if (progress == 0)
                {
                    StartCoroutine(LerpInventory());
                }
                return;
            }
            else
            {
                //Item not found
            }
        }
    }

    public void ToggleInventoryScreen()
    {
        StopAllCoroutines();
        inventoryRectTransform.anchoredPosition = inventoryClosedPosition;
        progress = 0;
        if (inventoryScreenIsOpen)
        {
            inventorySettingsButton.SetActive(false);
            menuBackground.SetActive(false);
            inventoryRectTransform.anchoredPosition = inventoryClosedPosition;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;

            inventoryScreenIsOpen = !inventoryScreenIsOpen;
        }
        else
        {
            firstInventorySlot.Select();
            inventorySettingsButton.SetActive(true);
            menuBackground.SetActive(true);
            inventoryRectTransform.anchoredPosition = inventoryOpenPosition;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            inventoryScreenIsOpen = !inventoryScreenIsOpen;
        }
    }

    private IEnumerator LerpInventory()
    {
        while (progress <= 1)
        {
            inventoryRectTransform.anchoredPosition = Vector2.Lerp(inventoryClosedPosition, inventoryOpenPosition, progress);
            progress += Time.deltaTime;
            yield return null;
        }
        inventoryRectTransform.anchoredPosition = inventoryOpenPosition;
        yield return new WaitForSeconds(2.0f);
        progress = 0;
        while (progress <= 1)
        {
            inventoryRectTransform.anchoredPosition = Vector2.Lerp(inventoryOpenPosition, inventoryClosedPosition, progress);
            progress += Time.deltaTime;
            yield return null;
        }
        inventoryRectTransform.anchoredPosition = inventoryClosedPosition;
        progress = 0;
    }

    public void SelectItem(Item item)
    {
        heldItem = item;
        Vector3 playerPos = character.transform.position;
        Vector3 playerDirection = character.transform.forward;
        Quaternion playerRotation = character.transform.rotation;
        float spawnDistance = 1;
        Vector3 spawnPos = playerPos + playerDirection * spawnDistance;
        heldGameObject = GameObject.Instantiate(heldItem.itemPrefab, spawnPos, playerRotation, character.transform);

        heldGameObject.name = heldItem.itemPrefab.name;
        HeldScript script = heldGameObject.GetComponent<HeldScript>();
        script.enabled = true;
        ToggleInventoryScreen();
    }

    public void GameStartPressed()
    {
        startMenu.SetActive(false);
        menuBackground.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        startDoorAnimator.SetBool("Open", true);
        gameStarted = true;
    }
    public void PublishSubtitle(string textToPublish)
    {
        PublishSubtitle(textToPublish, Color.white);
    }


    public void PublishSubtitle(string textToPublish, Color subtitleTint)
    {
        if (!gameStarted)
        {
            return;
        }
        else
        {
            GameObject currentSubtitle = GameObject.Instantiate(subtitlePrefab, subtitlePanel.transform);
            TextMeshProUGUI currentText = currentSubtitle.GetComponent<TextMeshProUGUI>();
            currentText.text = textToPublish;
            currentText.color = subtitleTint;
        }
    }

    public void CompleteLevel()
    {
        restartGameButton.Select();
        endMenu.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene("DungeonScene");
    }
}