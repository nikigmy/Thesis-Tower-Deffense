using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WavePart : MonoBehaviour {

    Wave parent;
    Declarations.WavePart data;
    [SerializeField]
    Text indexText;
    [Header("Enemy")]
    [SerializeField]
    Dropdown typeDropdown;

    [Header("Time")]
    [SerializeField]
    InputField delayField;
    
    public void SetData(int index, Wave wave, Declarations.WavePart wavePart)
    {
        data = wavePart;
        parent = wave;
        indexText = transform.GetChild(0).GetComponent<Text>();
        indexText.text = (index + 1).ToString();
        if(wavePart.Type == Declarations.WavePartType.Spawn)
        {
            var enemyType = ((Declarations.SpawnWavePart)wavePart).EnemyToSpawn.Type;
            var options = new List<Dropdown.OptionData>();
            var typeIndex = -1;
            var enemyTypes = (Declarations.EnemyType[])Enum.GetValues(typeof(Declarations.EnemyType));
            for (int i = 0;  i < enemyTypes.Length;  i++)
            {
                if (enemyType == enemyTypes[i])
                {
                    typeIndex = i;
                }
                options.Add(new Dropdown.OptionData(enemyTypes[i].ToString()));
            }
            if (typeIndex != -1)
            {
                typeDropdown.ClearOptions();
                typeDropdown.AddOptions(options);
                typeDropdown.value = typeIndex;
            }
            else
            {
                wave.DeletePart(index);
                Destroy(gameObject);
            }
        }
        else
        {
            delayField.text = ((Declarations.DelayWavePart)wavePart).Delay.ToString();
        }
    }

    public void OnDelayChanged(string value)
    {
        float time;
        if (float.TryParse(value, out time))
        {
            ((Declarations.DelayWavePart)data).Delay = time;
        }
        else
        {
            Debug.Log("cant parse time: " + time);
        }
    }

    public void OnEnemyTypeChanged(int index)
    {
        ((Declarations.SpawnWavePart)data).EnemyToSpawn = Def.Instance.EnemyDictionary[(Declarations.EnemyType)index];
    }

    public void UpdateIndex(int index)
    {
        indexText.text = (index + 1).ToString();
    }

    public void Delete()
    {
        parent.DeletePart(int.Parse(indexText.text) - 1);
        Destroy(gameObject);
    }
}
