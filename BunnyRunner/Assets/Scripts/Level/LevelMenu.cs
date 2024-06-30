using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{
    [System.Serializable]
    public class LevelButtonPair
    {
        public Button interactableButton;
        public Button nonInteractableButton;
    }

    public LevelButtonPair[] buttonPairs;
    public Sprite interactableSprite;
    public Sprite nonInteractableSprite;
    public GameObject heartImagePrefab;  // Prefab of the heart image to be instantiated

    public GameObject lockedLevelMessage;  // UI element to display the locked level message
    public Text lockedLevelText;           // Text to display for locked levels
    public GameObject popUpMessage;        // UI element for pop-up messages

    private void Awake()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        Debug.Log($"UnlockedLevel from PlayerPrefs: {unlockedLevel}");

        for (int i = 0; i < buttonPairs.Length; i++)
        {
            if (i < unlockedLevel)
            {
                buttonPairs[i].interactableButton.gameObject.SetActive(true);
                buttonPairs[i].nonInteractableButton.gameObject.SetActive(false);

                if (interactableSprite != null)
                {
                    buttonPairs[i].interactableButton.GetComponent<Image>().sprite = interactableSprite;
                    Debug.Log($"Interactable Button {i} set with interactableSprite.");
                }
                else
                {
                    Debug.LogWarning($"interactableSprite is not set for button {i}.");
                }

                // Manage the heart image visibility
                Transform heartTransform = buttonPairs[i].interactableButton.transform.Find("Heart");
                if (heartTransform != null)
                {
                    if (i == unlockedLevel - 1)
                    {
                        heartTransform.gameObject.SetActive(true);
                        Debug.Log($"Heart image activated on interactable button {i}.");
                    }
                    else
                    {
                        heartTransform.gameObject.SetActive(false);
                        Debug.Log($"Heart image deactivated on interactable button {i}.");
                    }
                }
                else
                {
                    Debug.LogWarning($"Heart child object not found on interactable button {i}.");
                }
            }
            else
            {
                buttonPairs[i].interactableButton.gameObject.SetActive(false);
                buttonPairs[i].nonInteractableButton.gameObject.SetActive(true);

                if (nonInteractableSprite != null)
                {
                    buttonPairs[i].nonInteractableButton.GetComponent<Image>().sprite = nonInteractableSprite;
                    Debug.Log($"Non-Interactable Button {i} set with nonInteractableSprite.");
                }
                else
                {
                    Debug.LogWarning($"nonInteractableSprite is not set for button {i}.");
                }
            }
        }
    }

    public void OpenLevel(int levelId)
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        if (levelId <= unlockedLevel)
        {
            string levelName = "Level_" + levelId;
            try
            {
                SceneManager.LoadScene(levelName);
            }
            catch
            {
                ShowErrorMessage($"Level {levelId} not found.");
            }
        }
        else
        {
            ShowLockedLevelMessage(levelId);
        }
    }

    private void ShowLockedLevelMessage(int levelId)
    {
        if (lockedLevelMessage != null && lockedLevelText != null)
        {
            lockedLevelText.text = $"Level {levelId} is locked!";
            lockedLevelMessage.SetActive(true);
            StartCoroutine(HideLockedLevelMessageAfterDelay(2f));
        }
        else
        {
            ShowErrorMessage("Locked level message or text is not set.");
        }
    }

    private void ShowErrorMessage(string message)
    {
        if (lockedLevelMessage != null && lockedLevelText != null)
        {
            lockedLevelText.text = message;
            lockedLevelMessage.SetActive(true);
            StartCoroutine(HideLockedLevelMessageAfterDelay(2f));
        }
        else
        {
            Debug.LogWarning(message);
        }

        ActivatePopupMessage();
    }

    private IEnumerator HideLockedLevelMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        lockedLevelMessage.SetActive(false);
        popUpMessage.SetActive(false); // Hide popup message after delay
    }

    private void ActivatePopupMessage()
    {
        if (popUpMessage != null)
        {
            popUpMessage.SetActive(true);
        }
        else
        {
            Debug.LogWarning("popUpMessage is not set.");
        }
    }
}
