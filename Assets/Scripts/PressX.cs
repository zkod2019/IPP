using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;


public class PressX : MonoBehaviour
{
    public GameObject pressX;
    public GameObject inputText;
    GameObject question;
    public QuestionStatus questionStatus;
    public String userAnswer;
    public QuestionType type;
    public int wrongAnswer = 0;
    public Text answerText;
    public GameObject weapon;
    public LineRenderer lineRenderer;
    public GameObject[] choices;

    void Awake() {
        choices =  new GameObject[7];
    }

    void Start()
    {
        this.question = GameObject.Find("Canvas").transform.GetChild(0).gameObject;

        pressX.SetActive(false);
        question.SetActive(false);

        Canvas canvas = (Canvas)FindObjectOfType(typeof(Canvas));

        for (int i = 0; i < 7; i++) {
            choices[i] = canvas.transform.GetChild(i+8).gameObject;
            choices[i].SetActive(false);
        }
    }

    public void OnTriggerEnter(Collider player)
    {
        bool inInventory = false;
        foreach (var collectable in player.gameObject.GetComponent<PlayerInventory>().inventory){
            if (collectable.gameObject.name == this.weapon.name) {
                inInventory = true;
                break;
            }
        }
        if (!this.questionStatus.answered && player.gameObject.tag == "Player" && !inInventory)
        {
            question.GetComponent<Image>().sprite = questionStatus.question;
            pressX.SetActive(true);
            Debug.Log(choices.Length);
            for (int i = 0; i < 7; i++) {
                Button btn = choices[i].GetComponent<Button>();
                btn.onClick.AddListener(delegate { OnButtonPress(btn); });
            }
        }
    }

    public void OnButtonPress(Button pressed) {
        Debug.Log("Button is pressed");
        Debug.Log(pressed);
        userAnswer = pressed.GetComponentInChildren<TMP_Text>().text;
        Debug.Log(userAnswer);
        Debug.Log(this.questionStatus.answer);
        if (userAnswer == this.questionStatus.answer) {
            Debug.Log("Correct Answer");
            answerText.text = "CORRECT!";
            ShowAnswerText();
            Invoke("HideAnswerText", 2);
             
            if (type == QuestionType.Logic){
                for (int i = 0; i < HintController.iqQuestions.Length; i++){
                    if (HintController.iqQuestions[i].question == this.questionStatus.question){
                        HintController.answerQuestion(type, i);
                    }
                }
            } else if (type == QuestionType.Math){
                for (int i = 0; i < HintController.mathQuestions.Length; i++){
                    if (HintController.mathQuestions[i].question == this.questionStatus.question){
                        HintController.answerQuestion(type, i);
                    }
                }
            }else if (type == QuestionType.Reading){
                for (int i = 0; i < HintController.readingQuestions.Length; i++){
                    if (HintController.readingQuestions[i].question == this.questionStatus.question){
                        HintController.answerQuestion(type, i);
                    }
                }
            }

            HintController.InitializeBarrels();
            
            pressX.SetActive(false);
            question.SetActive(false);
            for (int i = 0; i < 7; i++) {
                    choices[i].SetActive(false);
                    choices[i].GetComponent<Button>().onClick.RemoveAllListeners();
                }

            if (weapon != null) {
                var path = new UnityEngine.AI.NavMeshPath();

            // this is the little hack i added at the end
            // it wasn't working between barrel->weapon before cuz their Y positions were different
            // so we just set both Y's to the same thing
                var selfPosition = gameObject.transform.position;
                selfPosition.y = -9;
                var weaponPosition = weapon.transform.position;
                weaponPosition.y = -9;

                if (UnityEngine.AI.NavMesh.CalculatePath(selfPosition, weaponPosition, UnityEngine.AI.NavMesh.AllAreas, path))
                {
                    lineRenderer.positionCount = path.corners.Length;
                    for (int i = 0; i < path.corners.Length; i++)
                        lineRenderer.SetPosition(i, path.corners[i] + Vector3.up * 5);
                    Invoke("DisableLineRender", 15);
                }
                else{
                    Debug.Log("could not find path");
                }
            }
        }
        else{
            wrongAnswer += 1;
            if (wrongAnswer == 1){
                answerText.text = "WRONG!";
                ShowAnswerText();
                Invoke("HideAnswerText", 2);
            }
            Debug.Log("WRONG! LAST TRY!");
            if (wrongAnswer == 2){
                answerText.text = "WRONG! LAST TRY!";
                ShowAnswerText();
                Invoke("HideAnswerText", 2);
            }
            if (wrongAnswer == 3){
                answerText.text = "WRONG AGAIN!";
                ShowAnswerText();
                Invoke("HideAnswerText", 2);
                this.gameObject.SetActive(false);
                pressX.SetActive(false);
                question.SetActive(false);
        
                 for (int i = 0; i < 7; i++) {
                    choices[i].SetActive(false);
                    choices[i].GetComponent<Button>().onClick.RemoveAllListeners();
                }
            }
        }
    }

    void OnTriggerExit(Collider player)
    {
        if (player.gameObject.tag == "Player")
        {
            question.GetComponent<Image>().sprite = null;
            inputText.GetComponent<InputField>().text = "";

            pressX.SetActive(false);
            question.SetActive(false);

                    for (int i = 0; i < 7; i++) {
            choices[i].SetActive(false);
            choices[i].GetComponent<Button>().onClick.RemoveAllListeners();
        }
        }
    }

    void DisableLineRender() {
        lineRenderer.positionCount = 0;
    }

    void HideAnswerText() {
        answerText.enabled = false;
    }
    void ShowAnswerText() {
        answerText.enabled = true;
    }

    public void ActivateQuestion(){
        question.SetActive(true);
        for (int i = 0; i < 7; i++) {
            choices[i].SetActive(true);
        }
        pressX.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && pressX.activeSelf)
        {
           ActivateQuestion();
        }
    }
}