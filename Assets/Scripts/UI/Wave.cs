using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wave : MonoBehaviour {

    Declarations.WaveData data;
    List<WavePart> parts;

    [SerializeField]
    Text indexText;
    [SerializeField]
    Text partCountText;

    internal void DeletePart(int index)
    {
        parts.RemoveAt(index);
        data.WaveParts.RemoveAt(index);
        for (int i = index; i < parts.Count; i++)
        {
            parts[i].UpdateIndex(i);
        }
        partCountText.text = "PartCount: " + data.WaveParts.Count.ToString();
    }

    public void AddPart(Declarations.WavePart wavePart, WavePart part)
    {
        Debug.Log("Added part");
        parts.Add(part);
        data.WaveParts.Add(wavePart);
        partCountText.text = "PartCount: " + data.WaveParts.Count.ToString();
    }

    public void EditClicked()
    {
        GameManager.instance.LevelCreator.InitWaveEdit(this, data.WaveParts);
    }

    public void DeleteClicked()
    {
        GameManager.instance.LevelCreator.DeleteWave(int.Parse(indexText.text) - 1);
        Destroy(gameObject);
    }

    internal void SetData(int index, Declarations.WaveData wave)
    {
        data = wave;
        indexText.text = (index + 1).ToString();
        partCountText.text = "PartCount: " + data.WaveParts.Count.ToString();
    }

    public void SetParts(List<WavePart> wavePartObjects)
    {
        parts = wavePartObjects;
    }

    public void UpdateIndex(int index)
    {
        indexText.text = (index + 1).ToString();
    }
}
