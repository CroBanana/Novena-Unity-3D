using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TopicsButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    public Topic topic;
    public TextMeshProUGUI topicNumberText;
    public int topicNumber;

    public void SetTopicData(int _number, Topic _translatedContent)
    {
        topicNumber = _number;
        topic = _translatedContent;
        buttonText.text = topic.Name;
        topicNumberText.text = topicNumber.ToString();
    }

    public void UseThisTopic()
    {
        GameMaster.Instance.OpenThisTopic(topicNumber ,topic);
    }
}
