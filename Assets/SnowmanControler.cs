using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SnowmanControler : MonoBehaviour
{
    [SerializeField] GameObject wordContainer;
    [SerializeField] GameObject keyboardContainer;
    [SerializeField] GameObject letterContainer;
    [SerializeField] GameObject[] snowmanStages;
    [SerializeField] GameObject letterButton;
    [SerializeField] TextAsset possibleWord;

    private string word;
    private int incorrectGuesses, correctGuesses;

    void Start()
    {
        InitialiseButtons();
        InitialiseGame();
    }

    private void InitialiseButtons() {
        for (int i = 65; i <= 90; i++) {
            createButton(i);
        }
    }

    private void InitialiseGame() {
        // reset data back to original state
        incorrectGuesses = 0;
        correctGuesses = 0;
        foreach (Button child in keyboardContainer.GetComponentsInChildren<Button>()) {
            child.interactable = true;
        }
        foreach (Transform child in wordContainer.GetComponentsInChildren<Transform>()) {
            if (child != wordContainer.transform) { // avoid destroying the parent container itself
                Destroy(child.gameObject);
            }
        }
        foreach (GameObject stage in snowmanStages) {
            stage.SetActive(false);
        }

        snowmanStages[0].SetActive(true);

        // generate new word
        word = generateWord().ToUpper();
        Debug.Log("Generated word: " + word);

        // Calculate 30% of the word length
        int lettersToReveal = Mathf.CeilToInt(word.Length * 0.3f);

        // Select random indices to reveal
        HashSet<int> revealedIndices = new HashSet<int>();
        while (revealedIndices.Count < lettersToReveal) {
            int index = Random.Range(0, word.Length);
            revealedIndices.Add(index);
        }

        // Create letter containers and reveal selected letters
        for (int i = 0; i < word.Length; i++) {
            var temp = Instantiate(letterContainer, wordContainer.transform);
            var textComponent = temp.GetComponentInChildren<TextMeshProUGUI>();
            Debug.Log("Instantiated letter container for: " + word[i]);
            if (revealedIndices.Contains(i)) {
                textComponent.text = word[i].ToString();
                correctGuesses++; // Increment correct guesses for revealed letters
            } else {
                textComponent.text = "#"; // initialize with underscore or blank space
            }
        }
    }

    private void createButton(int i) {
        GameObject temp = Instantiate(letterButton, keyboardContainer.transform);
        temp.GetComponentInChildren<TextMeshProUGUI>().text = ((char)i).ToString();
        temp.GetComponent<Button>().onClick.AddListener(delegate { checkLetter(((char)i).ToString()); });
    }

    private string generateWord() {
        string[] wordList = possibleWord.text.Split("\n");
        string line = wordList[Random.Range(0, wordList.Length - 1)];
        return line.Substring(0, line.Length - 1);
    }

    private void checkLetter(string inputLetter) {
        bool letterInWord = false;
        for (int i = 0; i < word.Length; i++) {
            if (inputLetter == word[i].ToString()) {
                letterInWord = true;
                correctGuesses++;
                wordContainer.GetComponentsInChildren<TextMeshProUGUI>()[i].text = inputLetter;
            }
        }
        if (!letterInWord) {
            if (incorrectGuesses > 0) {
                // Deactivate the previous snowman stage
                snowmanStages[incorrectGuesses - 1].SetActive(false);
            }
            incorrectGuesses++;
            // Activate the current snowman stage
            snowmanStages[incorrectGuesses - 1].SetActive(true);
        }
        CheckOutcome();
    }

    private void CheckOutcome() {
        if (correctGuesses == word.Length) { // WIN
            for (int i = 0; i < word.Length; i++) {
                wordContainer.GetComponentsInChildren<TextMeshProUGUI>()[i].color = Color.green;
            }
            Invoke("InitialiseGame", 3f); // call a function, after specific time delay: Invoke(function, time-delay)
        }

        if (incorrectGuesses == snowmanStages.Length) { // LOSE
            for (int i = 0; i < word.Length; i++) {
                wordContainer.GetComponentsInChildren<TextMeshProUGUI>()[i].color = Color.red;
                wordContainer.GetComponentsInChildren<TextMeshProUGUI>()[i].text = word[i].ToString();
            }
            Invoke("InitialiseGame", 3f);
        }
    }
}
