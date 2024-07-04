using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    public int currentCharacterIndex;
    public GameObject[] characters;

    void Start()
    {
        currentCharacterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);

        foreach (GameObject character in characters)
        {
            character.SetActive(false);
        }
        characters[currentCharacterIndex].SetActive(true);

        // Notify the PlayerController to update the animator reference
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.UpdateAnimatorReference();
        }

        // Notify the PlayerHealth to update the animator reference
        PlayerHealth playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.UpdateAnimatorReference();
        }
    }
}

