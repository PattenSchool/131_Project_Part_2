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
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;
using UnityStandardAssets.Characters.FirstPerson;


public class SettingsManager : MonoBehaviour
{
    public Dropdown fontDropdown;
    public float subtitleDurationSelected;
    public TextMeshProUGUI subtitleDurationLabel;
    public CrosshairRay crosshairRay;
    public Button startMenuSettingsButton;
    private Text controlToRebindText;
    private string controlToRebind;
    private bool isRebinding = false;
    public Dictionary<string, KeyCode> buttonkeys = new Dictionary<string, KeyCode>();

    public RigidbodyFirstPersonController rigidbodyFirstPersonController;
    private GameObject player;
    private bool audioDescriptionEnabled = false;

    public List<GameObject> gameUIPanels = new List<GameObject>();
    private List<TextMeshProUGUI> gameUITextComponents = new List<TextMeshProUGUI>();
    private List<Button> gameUIButtonComponents = new List<Button>();


    //1
    public TMP_FontAsset simpleFont;
    public TMP_FontAsset fancyFont;
    //2
    public TextMeshProUGUI exampleText;
    //3
    public TMP_FontAsset fontStyleSelected;
    public float fontSizeSelected;

    public AudioMixer mixer;
    private GameManager gameManager;

    [Header("Settings Panel elements")]
    public Slider volumeSlider;

    public Slider torchVolumeSlider;
    public Slider fontSizeSlider;
    public Toggle audioDescriptionToggle;
    public Slider mouseSensitivityXSlider;
    public Slider mouseSensitivityYSlider;
    public Text inventoryRebindKeyText;
    public Toggle holdButtonToggle;
    public Slider subtitleDurationSlider;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        foreach (GameObject panel in gameUIPanels)
        {
            TextMeshProUGUI[] tempArray = panel.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI item in tempArray)
            {
                gameUITextComponents.Add(item);
            }
            Button[] tempButtonArray = panel.GetComponentsInChildren<Button>();
            foreach (Button button in tempButtonArray)
            {
                gameUIButtonComponents.Add(button);
            }
        }

        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        LoadSettings(); // Keep this line of code at the end of the Start method!!!
    }

    private void Update()
    {
        if (isRebinding)
        {
            //1
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                return;
            }
            //2
            if (Input.anyKeyDown)
            {
                foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        //3
                        controlToRebindText.text = keyCode.ToString();
                        HandleKeyBindChangefor(controlToRebind, keyCode);
                        isRebinding = false;
                        gameManager.isRebinding = false;
                        break;
                    }
                }
            }
        }

    }

    #region AdjustSettings

    public void SetLevel(float sliderValue)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20);
        SaveSettings("Volume", sliderValue);
    }

    public void SetTorchLevel(float sliderValue)
    {
        mixer.SetFloat("TorchVolume", Mathf.Log10(sliderValue) * 20);
        SaveSettings("TorchVolume", sliderValue);
    }

    public void SetFontType(int dropdownValue)
    {
        //1

        switch (dropdownValue)
        {
            case 0:
                exampleText.font = fancyFont;
                fontStyleSelected = fancyFont;
                break;

            case 1:
                exampleText.font = simpleFont;
                fontStyleSelected = simpleFont;
                break;

            default:
                exampleText.font = fancyFont;
                break;
        }
        SaveSettings("FontType", dropdownValue);
    }
    private void SaveSettings(string keyString, int intToSave)
    {
        PlayerPrefs.SetInt(keyString, intToSave);
    }

    public void SetSize(float sliderValue)
    {
        //1
        exampleText.fontSize = sliderValue;
        fontSizeSelected = sliderValue;
        SaveSettings("FontSize", sliderValue); //**See Note below**
    }

    public void ToggleAudioDescription(bool isDescriptionEnabled)
    {
        audioDescriptionEnabled = isDescriptionEnabled;
        SaveSettings("DescriptionEnabled", isDescriptionEnabled.ToString());

    }

    public void SetMouseSensitivityX(float sliderValue)
    {
        rigidbodyFirstPersonController.mouseLook.XSensitivity = sliderValue;
        SaveSettings("MouseX", sliderValue);
    }

    public void SetMouseSensitivityY(float sliderValue)
    {
        rigidbodyFirstPersonController.mouseLook.YSensitivity = sliderValue;
        SaveSettings("MouseY", sliderValue);
    }

    public void HandleRebindOfControl(string controlPressed, Text buttonInstanceText)
    {
        controlToRebind = controlPressed;
        controlToRebindText = buttonInstanceText;
        isRebinding = true;
        gameManager.isRebinding = true;
    }


    public void HandleKeyBindChangefor(string controlToRebind, KeyCode keyCode)
    {
        buttonkeys[controlToRebind] = keyCode;
        PlayerPrefs.SetString(controlToRebind, keyCode.ToString());
        if (controlToRebind == "Inventory")
        {
            gameManager.inventoryKeyCode = keyCode;
        }
    }

    public void ToggleHoldButton(bool isHoldEnabled)
    {
        crosshairRay.isHoldButtonEnabled = isHoldEnabled;
        SaveSettings("HoldEnabled", isHoldEnabled.ToString());
    }

    public void SetSubtitleDuration(float sliderValue)
    {
        subtitleDurationSelected = sliderValue;
        subtitleDurationLabel.text = sliderValue.ToString() + " secs";
        SaveSettings("SubtitleDuration", sliderValue);
    }

    #endregion AdjustSettings

    public void SettingsDidExitOrLoad()
    {
        //1
        foreach (TextMeshProUGUI textComponent in gameUITextComponents)
        {
            textComponent.font = fontStyleSelected;
            textComponent.fontSize = fontSizeSelected;
        }
        //2
        foreach (Button button in gameUIButtonComponents)
        {
            button.image.rectTransform.sizeDelta = new Vector2((fontSizeSelected / 32) * 200, (fontSizeSelected / 32) * 50);
        }

        if (gameManager.gameStarted)
        {
            gameManager.ToggleInventoryScreen();
        }
        else
        {
            startMenuSettingsButton.Select();
        }

    }

    public void HandleAudioDescription(string subtitle, GameObject origin)
    {
        if (!audioDescriptionEnabled)
        {
            return;
        }
        else
        {
            //1
            var heading = origin.transform.position - player.transform.position;
            //2
            var distance = heading.magnitude;
            if (distance <= 5.0)
            {
                subtitle = subtitle + " (close ";
            }
            else if (distance >= 15)
            {
                subtitle = subtitle + " (far ";
            }
            else if (distance >= 20)
            {
                return;
            }
            else
            {
                subtitle = subtitle + " (";
            }
            //3
            Quaternion rotation = Quaternion.LookRotation(heading, Vector3.up);
            Vector3 angles = rotation.eulerAngles;
            //4
            // Adjust for the rotation of the player
            var direction = player.transform.eulerAngles.y - angles.y;
            if (direction >= -45 && direction <= 45)
            {
                subtitle = subtitle + "infront of you)";
            }
            else if (direction > 135 || direction < -135)
            {
                subtitle = subtitle + "behind you)";
            }
            else if (direction < -45 && direction >= -135)
            {
                subtitle = subtitle + "to your right)";
            }
            else if (direction > 45 && direction <= 135)
            {
                subtitle = subtitle + "to your left)";
            }
            gameManager.PublishSubtitle(subtitle, Color.yellow);
        }
    }

    public void SelectFirstUIElement()
    {
        volumeSlider.Select();
    }


    #region SavingAndLoading

    private void SaveSettings(string keyString, float floatToSave)
    {
        PlayerPrefs.SetFloat(keyString, floatToSave);
    }

    private void SaveSettings(string keyString, string stringToSave)
    {
        PlayerPrefs.SetString(keyString, stringToSave);
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("Volume"))
        {
            float volumeFloat = (PlayerPrefs.GetFloat("Volume"));
            SetLevel(volumeFloat);
            volumeSlider.value = volumeFloat;
        }
        else
        {
            SetLevel(1.0f);
            volumeSlider.value = 1.0f;
        }
        if (PlayerPrefs.HasKey("FontType"))
        {
            int fontTypeInt = PlayerPrefs.GetInt("FontType");
            SetFontType(fontTypeInt);
            fontDropdown.value = fontTypeInt;
        }
        else
        {
            SetFontType(0);
            fontDropdown.value = 0;
        }
        if (PlayerPrefs.HasKey("FontSize"))
        {
            float fontSizeFloat = PlayerPrefs.GetFloat("FontSize");
            SetSize(fontSizeFloat);
            fontSizeSlider.value = fontSizeFloat;
        }
        else
        {
            SetSize(36.0f);
            fontSizeSlider.value = 36.0f;
        }
        if (PlayerPrefs.HasKey("Inventory"))
        {
            string keycodeString = PlayerPrefs.GetString("Inventory");
            buttonkeys["Inventory"] = (KeyCode)Enum.Parse(typeof(KeyCode), keycodeString);
            inventoryRebindKeyText.text = keycodeString;
        }
        else
        {
            buttonkeys["Inventory"] = KeyCode.I;
            inventoryRebindKeyText.text = "I";
        }

        if (PlayerPrefs.HasKey("SubtitleDuration"))
        {
            float subtitleDurationFloat = PlayerPrefs.GetFloat("SubtitleDuration");
            SetSubtitleDuration(subtitleDurationFloat);
            subtitleDurationSlider.value = subtitleDurationFloat;
            subtitleDurationLabel.text = subtitleDurationFloat.ToString("F2") + " secs";
        }
        else
        {
            SetSubtitleDuration(3.0f);
            subtitleDurationSlider.value = 3.0f;
            subtitleDurationLabel.text = "3 secs";
        }

        if (PlayerPrefs.HasKey("TorchVolume"))
        {
            float volumeFloat = (PlayerPrefs.GetFloat("TorchVolume"));
            SetTorchLevel(volumeFloat);
            torchVolumeSlider.value = volumeFloat;
        }
        else
        {
            SetLevel(1.0f);
            volumeSlider.value = 1.0f;
        }

        if (PlayerPrefs.HasKey("HoldEnabled"))
        {
            bool holdEnabled = Convert.ToBoolean(PlayerPrefs.GetString("HoldEnabled"));
            ToggleHoldButton(holdEnabled);
            holdButtonToggle.isOn = holdEnabled;
        }
        else
        {
            ToggleHoldButton(false);
            holdButtonToggle.isOn = false;
        }

        if (PlayerPrefs.HasKey("DescriptionEnabled"))
        {
            audioDescriptionEnabled = Convert.ToBoolean(PlayerPrefs.GetString("DescriptionEnabled"));
            audioDescriptionToggle.isOn = audioDescriptionEnabled;
        }
        else
        {
            audioDescriptionEnabled = false;
            audioDescriptionToggle.isOn = false;
        }
        if (PlayerPrefs.HasKey("MouseX"))
        {
            float mouseSensitivityX = PlayerPrefs.GetFloat("MouseX");
            SetMouseSensitivityX(mouseSensitivityX);
            mouseSensitivityXSlider.value = mouseSensitivityX;
        }
        else
        {
            SetMouseSensitivityX(2.0f);
            mouseSensitivityXSlider.value = 2.0f;
        }
        if (PlayerPrefs.HasKey("MouseY"))
        {
            float mouseSensitivityY = PlayerPrefs.GetFloat("MouseY");
            SetMouseSensitivityY(mouseSensitivityY);
            mouseSensitivityYSlider.value = mouseSensitivityY;
        }
        else
        {
            SetMouseSensitivityY(2.0f);
            mouseSensitivityYSlider.value = 2.0f;
        }
        SettingsDidExitOrLoad();
    }

    #endregion SavingAndLoading
}