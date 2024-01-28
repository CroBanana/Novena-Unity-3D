using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Data 
{
    public List<TranslatedContent> TranslatedContents;
}

[Serializable]
public class Media
{
    public string Name;
    public List<Photo> Photos;
    public string FilePath;
}

[Serializable]
public class Photo
{
    public string Path;
    public string Name;
}

[Serializable]
public class Topic
{
    public string Name;
    public List<Media> Media;
}

[Serializable]
public class TranslatedContent
{
    public int LanguageId;
    public string LanguageName;
    public List<Topic> Topics;
}