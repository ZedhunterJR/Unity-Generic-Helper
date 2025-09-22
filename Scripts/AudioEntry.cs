using UnityEngine;
using System.Collections.Generic;

//move these 2 somewhere else later
public enum AudioName
{
    //placeholder
    ChaoMung,
    NoiDai,

    //may anh 
    ChaoMungMayAnh,
    GioiThieuMayAnh1,
    GioiThieuMayAnh2,
    GioiThieuMayAnh3,
    PhimTruongSo,
    BoothPhucTrang,
    Long360,
    Mocap,
    BoiCanhVR,
    KetThuc
}
public enum CharacterState
{
    Idle,
    Chao,
    Walking,
    Talking,
    Mayanh1,
    Mayanh2,
    Mayanh3,
    PhimTruongSo,
    BoothPhucTrang,
    Metaverse,
    Mocap,
    BoiCanhVR,
    KetThuc,
}
[System.Serializable]
public class NPCSoundEntry
{
    public AudioName audioName;
    public AudioClip clip;
    public float clipLength;
}

[CreateAssetMenu(fileName = "NewAudioEntry", menuName = "NPC/Audio Entry")]
public class AudioEntry : ScriptableObject
{
    public List<NPCSoundEntry> soundEntries;
    public string audioPath;
    public AudioName firstAudioName;

    public void ClearEntries()
    {
        soundEntries.Clear();
    }

    public void AddEntry(AudioClip clip)
    {
        soundEntries.Add(new NPCSoundEntry
        {
            audioName = firstAudioName,
            clip = clip,
            clipLength = clip != null ? clip.length : 0f
        });

        firstAudioName = GetNextAudioName(firstAudioName);
    }

    private AudioName GetNextAudioName(AudioName current)
    {
        var values = (AudioName[])System.Enum.GetValues(typeof(AudioName));
        int index = System.Array.IndexOf(values, current);

        // Wrap around to 0 if at the end
        int nextIndex = (index + 1) % values.Length;

        return values[nextIndex];
    }
}
