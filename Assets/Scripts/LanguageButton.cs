using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LanguageButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;

    public TranslatedContent translatedContent;

    public void SetLanguageData(TranslatedContent _translatedContent)
    {
        translatedContent = _translatedContent;
        buttonText.text = translatedContent.LanguageName;
    }

    public void UseThisLanguage()
    {
        GameMaster.Instance.CreateLanguageTopics(translatedContent.Topics);
    }
}
