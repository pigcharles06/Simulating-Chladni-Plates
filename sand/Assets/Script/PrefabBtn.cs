using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrefabBtn : MonoBehaviour
{
    private InputField waveNumber;
    public bool KisActive = false;
    private void Start() {
        waveNumber = GameObject.Find("input_wavenumber").GetComponent<InputField>();
    }
    public void changeWaveNumber()
    {
        Text newWaveText = GetComponentInChildren<Button>().GetComponentInChildren<Text>();
        waveNumber.text = newWaveText.text;

        List<GameObject> inactiveObjects = new List<GameObject>();
        inactiveObjects = FindInactiveObjects();
        string K_name = newWaveText.text + "K_prefab";
        if(!KisActive)
        {
            foreach(GameObject K_prefab in inactiveObjects)
            {
                if(K_prefab.name == K_name)
                {
                    if(K_prefab.activeSelf == false)
                    {
                        K_prefab.SetActive(true);
                        inactiveObjects.Add(K_prefab);
                        break;
                    }
                }
            }
            KisActive = true;
        }
        else
        {
            GameObject.Find(K_name).SetActive(false);
            KisActive = false;
        }
    }

    public List<GameObject> FindInactiveObjects()
    {
        List<GameObject> inactiveObjects = new List<GameObject>();

        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (!obj.activeSelf)
            {
                inactiveObjects.Add(obj);
            }
        }

        return inactiveObjects;
    }
}
