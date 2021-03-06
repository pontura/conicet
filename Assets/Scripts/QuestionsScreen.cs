﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionsScreen : ScreenMain
{
    public List<string> content;
    int num = 0;
    public Text field;
    public Image image;
    public GameObject intro;
    public FillAmountAnim introImage;
    public Character character;
    public GameObject repeatButton;
    public SliderLoop sliderLoop;

    public override void OnEnable()
    {
        intro.SetActive(true);
        introImage.Init();
        base.OnEnable();
    }
    public override void OnReady()
    {
        introImage.Init();
        base.OnReady();
        string story_id = Data.Instance.storiesData.activeContent.id;
        GamesData.Content c = Data.Instance.gamesData.GetContent(story_id);
        content = c.GetContentFor(type, gameID);
        print(gameID);
        print("story_id: " + story_id);
        print(content.Count);
        if (content == null) return;
        field.text = "";
        TextsData.Content tipContent = Data.Instance.daysData.GetTip("tip_questions");
        if (tipContent == null)
            OnTipDone();
        else
            Events.OnCharacterSay(tipContent, OnTipDone, tipContent.character_type);
    } 
    public void Repeat()
    {
        if(audio_text != "")
            Events.PlaySoundTillReady("voices", "genericTexts/" + audio_text, OnTextDone);
    }
    void OnTipDone()
    {
        introImage.Init();
        introImage.AnimateOff();
        num = 0;
        SetCard();
        sliderLoop.Init(content[num]);
    }
    int imageID = 0;
    string audio_text = "";
    void SetCard()
    {
        if(content[0] == "empty")
        {
            repeatButton.gameObject.SetActive(false);
            character.gameObject.SetActive(false);
            Invoke("OnTextDone", 10);
            return;
        }
        string text_id = content[num];
        audio_text = content[num];
        if (Data.Instance.lang == Data.langs.QOM) audio_text = "qom_" + text_id;
        Repeat();
        field.text = Data.Instance.textsData.GetContent(text_id, false).text;
        character.Init(Data.Instance.textsData.GetContent(text_id, false).character_type);
    }
    void OnTextDone()
    {
        Events.SetReadyButton(ButtonClicked);
    }
    public override void Hide(bool toLeft)
    {
        base.Hide(toLeft);
        intro.SetActive(true);
        introImage.Init();
    }
    void ButtonClicked()
    {
        num++;
        if (num >= content.Count)
        {
            
            OnComplete();
            Events.OnGoto(true);
        } else
        {            
            SetCard();
        }
    }
}
