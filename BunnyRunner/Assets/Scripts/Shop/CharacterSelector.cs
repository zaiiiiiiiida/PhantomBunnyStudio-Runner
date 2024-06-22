using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
