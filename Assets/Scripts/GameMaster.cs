using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System;

public class GameMaster : MonoBehaviour
{
    public static GameMaster Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    [Header("Main objects")]
    public GameObject languageUI;
    public GameObject topicSelectorUI;
    public GameObject topicUI;

    private void Start()
    {
        //za start potrebno je samo ukljuciti selektiranje jezika
        ActivateUI("");
        switchImageTimerConstant = switchImageTimer;
        audioProgressBar.onValueChanged.AddListener(OnSliderValueChanged);
        oldSliderValue = audioProgressBar.value;
    }

    private void Update()
    {
        if (loadedImages.Count == 0)
            return;

        switchImageTimer -= Time.deltaTime;
        if (switchImageTimer <= 0)
            SwitchImages();

        ChangeAudioStuff();
    }

    [Header("Language objects")]
    public GameObject languagePrefab;
    public GameObject content;

    public void CreateLanguageButtons()
    {
        foreach (var data in DataHandler.data.TranslatedContents)
        {
            Instantiate(languagePrefab, content.transform).GetComponent<LanguageButton>().SetLanguageData(data);
        }
    }

    [Header("Topic objects")]
    public GameObject topicButtonPrefab;
    public GameObject contentTopics;

    public void CreateLanguageTopics(List<Topic> _translatedContent)
    {
        //prazni vec postojeće od prijašnjeg jezika
        foreach (Transform child in contentTopics.transform)
        {
            Destroy(child.gameObject);
        }

        //kreira nove 
        int number = 1;
        foreach(Topic topic in _translatedContent)
        {
            Instantiate(topicButtonPrefab, contentTopics.transform).GetComponent<TopicsButton>().SetTopicData(number,topic);
            number++;
        }
        ActivateUI("Topic");
    }

    [Header("Selected topic")]
    public TextMeshProUGUI topicName;
    public TextMeshProUGUI topicNumber;
    public Image topicGallery;

    private Topic selectedTopic;

    public void OpenThisTopic(int _topicNumber ,Topic _newTopic)
    {
        Debug.Log("Original \n" + _newTopic.Media[0].Photos[0].Path);
        selectedTopic = _newTopic;
        topicName.text = selectedTopic.Name;
        topicNumber.text = _topicNumber.ToString();
        LoadAudio();
        LoadImages();
        ActivateUI("Topic Selected");
    }

   [Header("Audio")]
    public Image playImage;
    bool playingSound;
    public Sprite playSprite;
    public Sprite pauseSprite;
    public void PlayAudio()
    {
        //prvi put pokreće audio
        if(audioSource.isPlaying == false)
        {
            audioSource.Play();
        }

        //pauzira ga i pokrece opet
        if (audioSource.clip != null)
        {
            if (playingSound)
            {
                audioSource.Pause();
                playImage.sprite = playSprite;
                playingSound = false;
                return;
            }

            audioSource.UnPause();
            playImage.sprite = pauseSprite;
            playingSound = true;
        }
    }

    private void OnSliderValueChanged( float newValue)
    {
        if (Math.Abs(oldSliderValue - newValue) > 1f)
        {
            audioSource.time = newValue;
        }
    }

    public TextMeshProUGUI audioTime;
    public Slider audioProgressBar;
    private float oldSliderValue;
    public void ChangeAudioStuff()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            // max vrijeme clipa
            float clipLength = audioSource.clip.length;

            // trenutno vrijeme clipa
            float currentPosition = audioSource.time;

            audioTime.text = TimeSpan.FromSeconds(currentPosition).ToString(@"mm\:ss")
                            + " / " +
                            TimeSpan.FromSeconds(clipLength).ToString(@"mm\:ss");

            audioProgressBar.value = currentPosition;
        }
    }

    #region Image and audio loading
    [Header("Image and audio loading")]
    public List<Sprite> loadedImages;
    public float switchImageTimer;
    private float switchImageTimerConstant;
    int spriteIndex = 0;
    public AudioSource audioSource;
    private void LoadImages()
    {
        loadedImages.Clear();

        foreach(Photo image in selectedTopic.Media[0].Photos)
        {
            string imagesPath = Application.persistentDataPath + image.Path;
            if (File.Exists(imagesPath))
            {
                // Read the image file
                byte[] imageData = File.ReadAllBytes(imagesPath);

                // Create a Texture2D from the loaded image data
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(imageData);

                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                loadedImages.Add(sprite);
            }
            else
            {
                Debug.LogError("Image file not found at path: " + imagesPath);
            }
        }
        SwitchImages();
    }

    private void SwitchImages()
    {
        Debug.Log("Switching sprite");
        if (spriteIndex >= loadedImages.Count)
            spriteIndex = 0;
        topicGallery.sprite = loadedImages[spriteIndex];
        switchImageTimer = switchImageTimerConstant;
        spriteIndex++;
    }

    
    private void LoadAudio()
    {
        string audioPath = Application.persistentDataPath + selectedTopic.Media[1].FilePath;
        if (File.Exists(audioPath))
        {
            WWW www = new WWW("file://" + audioPath);
            while (!www.isDone) { }

            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError("Error loading audio file: " + www.error);
            }

            audioSource.clip = www.GetAudioClip();
            audioProgressBar.minValue = 0;
            audioProgressBar.maxValue = audioSource.clip.length;
        }
        else
        {
            Debug.LogError("Audio file not found at path: "+ audioPath);
        }
    }
    #endregion


    #region For Back buttons
    public void ActivateUI(string _whatToActivate)
    {
        StopAllCoroutines();

        //reseta audio button
        audioSource.Stop();
        playImage.sprite = playSprite;

        //deaktivira sve
        languageUI.SetActive(false);
        topicSelectorUI.SetActive(false);
        topicUI.SetActive(false);

        //aktivira potrebno
        switch (_whatToActivate)
        {
            case "Language":
                languageUI.SetActive(true);
                break;
            case "Topic":
                topicSelectorUI.SetActive(true);
                break;
            case "Topic Selected":
                topicUI.SetActive(true);
                break;
            default:
                languageUI.SetActive(true);
                break;
        }
    }
    #endregion

}
