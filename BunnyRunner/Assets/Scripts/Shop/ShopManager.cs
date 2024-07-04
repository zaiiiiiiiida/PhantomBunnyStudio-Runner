using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public int currentCharacterIndex = 0;
    public GameObject[] characters;
    public Text totalScoreText;
    private int totalScore;

    public CharacterBlueprint[] characterBlueprints;
    public Button playButton;
    public Button buyButton;

    public AudioSource audioSource; // Reference to the AudioSource component
    public AudioClip changeSound; // Sound clip for character change
    public AudioClip backgroundMusic; // Background music clip

    public GameObject particlePrefab; // Prefab of the particle effect

    private void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // Add AudioSource if not already present
        }

        // Play background music
        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true; // Loop the background music
            audioSource.Play();
        }

        foreach (CharacterBlueprint characterBlueprint in characterBlueprints)
        {
            if (characterBlueprint.price == 0)
            {
                characterBlueprint.isUnlocked = true;
            }
            else
            {
                characterBlueprint.isUnlocked = PlayerPrefs.GetInt(characterBlueprint.name, 0) == 1 ? true : false;
            }
        }

        totalScore = PlayerPrefs.GetInt("TotalScore", 0);
        totalScoreText.text = totalScore.ToString();
        currentCharacterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        foreach (GameObject character in characters)
        {
            character.SetActive(false);
        }
        characters[currentCharacterIndex].SetActive(true);
    }

    private void Update()
    {
        UpdateUI();
    }

    public void ChangeNext()
    {
        PlayChangeSound(); // Play the change sound

        characters[currentCharacterIndex].SetActive(false);
        currentCharacterIndex++;
        if (currentCharacterIndex == characters.Length)
        {
            currentCharacterIndex = 0;
        }
        characters[currentCharacterIndex].SetActive(true);

        // Instantiate particle effect
        InstantiateParticleEffect();

        CharacterBlueprint ch = characterBlueprints[currentCharacterIndex];
        if (!ch.isUnlocked)
        {
            return;
        }

        PlayerPrefs.SetInt("SelectedCharacter", currentCharacterIndex);
    }

    public void ChangePrevious()
    {
        PlayChangeSound(); // Play the change sound

        characters[currentCharacterIndex].SetActive(false);
        currentCharacterIndex--;
        if (currentCharacterIndex == -1)
        {
            currentCharacterIndex = characters.Length - 1;
        }
        characters[currentCharacterIndex].SetActive(true);

        // Instantiate particle effect
        InstantiateParticleEffect();

        CharacterBlueprint ch = characterBlueprints[currentCharacterIndex];
        if (!ch.isUnlocked)
        {
            return;
        }

        PlayerPrefs.SetInt("SelectedCharacter", currentCharacterIndex);
    }

    private void UpdateUI()
    {
        CharacterBlueprint ch = characterBlueprints[currentCharacterIndex];
        if (ch.isUnlocked)
        {
            buyButton.gameObject.SetActive(false);
            playButton.gameObject.SetActive(true);
        }
        else
        {
            buyButton.gameObject.SetActive(true);
            playButton.gameObject.SetActive(false);

            buyButton.GetComponentInChildren<Text>().text = ch.price.ToString();

            if (ch.price > PlayerPrefs.GetInt("TotalScore", 0))
            {
                buyButton.interactable = false;
            }
            else
            {
                buyButton.interactable = true;
            }
        }
    }

    public void UnlockCharacter()
    {
        CharacterBlueprint ch = characterBlueprints[currentCharacterIndex];

        PlayerPrefs.SetInt(ch.name, 1);
        PlayerPrefs.SetInt("SelectedCharacter", currentCharacterIndex);
        ch.isUnlocked = true;
        PlayerPrefs.SetInt("TotalScore", PlayerPrefs.GetInt("TotalScore", 0) - ch.price);
    }

    private void PlayChangeSound()
    {
        if (audioSource != null && changeSound != null)
        {
            audioSource.PlayOneShot(changeSound);
        }
    }

    private void InstantiateParticleEffect()
    {
        // Instantiate the particle effect at the position of the currently active character
        if (particlePrefab != null && characters[currentCharacterIndex] != null)
        {
            GameObject particleInstance = Instantiate(particlePrefab, characters[currentCharacterIndex].transform.position, Quaternion.identity);
            Destroy(particleInstance, 2f); // Destroy the particle effect after 2 seconds
        }
    }
}
