﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Letra : ScreenMain
{
    public List<string> content;
    int ok = 0;
    public Text field;
    string originalText;
    public SimpleButton card;
    public List<SimpleButton> allButtons;
    public Transform container;
    public Image image;
    bool clicked;

    public override void Hide(bool toLeft)
    {
        base.Hide(toLeft);
        Utils.RemoveAllChildsIn(container);
    }
    private void OnEnable()
    {
        image.gameObject.SetActive(false);
        field.text = "";
        Utils.RemoveAllChildsIn(container);
    }
    TextsData.Content tipContent;
    public override void OnReady()
    {
        image.gameObject.SetActive(false);
        Utils.RemoveAllChildsIn(container);
        base.OnReady();
        string story_id = Data.Instance.storiesData.activeContent.id;

        GamesData.Content c = Data.Instance.gamesData.GetContent(story_id);
        content = c.GetContentFor(type, gameID);

        if (content == null) return;
        field.text = "";
        clicked = false;
        tipContent = Data.Instance.daysData.GetTip("tip_letra");

        Events.OnCharacterSay(tipContent, OnTipDone, tipContent.character_type);
    }
    void OnTipDone()
    {
        allButtons.Clear();
        int id = 0;
        foreach (string text in content)
        {
            if (id == 0)
            {
                originalText = text;
                SetOriginalText();
            }
            else if (id == 1)
            {
                string[] arr = text.Split(","[0]);
                int id2 = 1;
                foreach (string s in arr)
                {
                    SetLetter(id2, s);
                    id2++;
                }
            } else if(id==2)
            {
                image.gameObject.SetActive(true);
                Sprite sprite = Data.Instance.assetsData.GetContent(text).sprite;
                if (sprite == null)
                    Events.Log("No hay asset para " + text);
                else
                    image.sprite = sprite;
            }
            id++;
        }
        AddButtons();
    }
    void SetOriginalText()
    {
        field.text = originalText;
    }
    void SetTitle(string letter)
    {
        string st = originalText;
        string newWord = st.Replace("_", letter);
        field.text = newWord;
        field.GetComponent<Animation>().Play();
    }
    void SetLetter(int id, string letter)
    {
        SimpleButton sb = Instantiate(card);
        sb.Init(id, null, letter, OnClicked);
        allButtons.Add(sb);
    }
    void AddButtons()
    {
        Utils.Shuffle(allButtons);
        foreach (SimpleButton sb in allButtons)
        {
            sb.transform.SetParent(container);
            sb.transform.localScale = Vector2.one;
        }
    }
    bool IsOk(int id)
    {
        return false;
    }
    void OnClicked(SimpleButton button)
    {
        if (clicked) return;
        clicked = true;
        SetTitle(button.text);
        if (button.id == 1)
        {
            button.GetComponent<SimpleFeedback>().SetState(SimpleFeedback.states.OK, 2);
            Invoke("OnCorrect", 1);
        }
        else
        {
            button.GetComponent<SimpleFeedback>().SetState(SimpleFeedback.states.WRONG, 2);
            Invoke("ResetLetters", 2);
        }
    }
    void OnCorrect()
    {
        Events.SetReadyButton(OnReadyClicked);
    }
    void ResetLetters()
    {
        clicked = false;
        foreach (SimpleButton sb in allButtons)
            sb.GetComponent<SimpleFeedback>().SetOff();
        SetOriginalText();
    }
    void OnReadyClicked()
    {
        OnComplete();
        Events.OnGoto(true);
    }
   
}
