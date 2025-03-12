using UnityEngine;

public class RadioModel
{
    public string Name { get; private set; }
    public AudioClip Clip { get; private set; }

    public RadioModel(string name, AudioClip clip)
    {
        Name = name;
        Clip = clip;
    }
}
