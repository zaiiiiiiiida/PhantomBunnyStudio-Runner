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

    private void Start()
    {   foreach(CharacterBlueprint characterBlueprint in characterBlueprints)
        {
            if(characterBlueprint.price == 0)
            {
                characterBlueprint.isUnlocked = true;
            }
            else
            {
                characterBlueprint.isUnlocked = PlayerPrefs.GetInt(characterBlueprint.name, 0) == 0 ? false: true;
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
        characters[currentCharacterIndex].SetActive(false);
        currentCharacterIndex++;
        if(currentCharacterIndex == characters.Length) 
        { 
            currentCharacterIndex = 0;
        }
        characters[currentCharacterIndex].SetActive(true);

        CharacterBlueprint ch = characterBlueprints[currentCharacterIndex];
        if(!ch.isUnlocked) 
        {
            return;
        }


        PlayerPrefs.SetInt("SelectedCharacter", currentCharacterIndex);

    }
    public void ChangePrevious()
    {
        characters[currentCharacterIndex].SetActive(false);
        currentCharacterIndex--;
        if (currentCharacterIndex == -1)
        {
            currentCharacterIndex = characters.Length - 1;
        }
        characters[currentCharacterIndex].SetActive(true);

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
        if(ch.isUnlocked)
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


}
