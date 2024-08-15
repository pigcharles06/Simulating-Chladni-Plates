using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;

public class SaveData : MonoBehaviour
{
    [SerializeField] PlayerData data;
    public InputField waveNumber;
    public InputField MaxWaveNumber;
    public InputField X_Axis;
    public InputField Y_Axis;
    public InputField SandSpeed;
    public InputField Amp;
    public InputField C;
    public Canvas LoadingCanvas;
    private string savePath;
    private static bool hasLoadedData = false; // 靜態變數追蹤是否已載入過資料


    private void Awake() {
        savePath = Path.Combine(Application.dataPath, "sandData.json");
    }

    private IEnumerator Start()
    {
        // 等待其他腳本也進場後執行LoadAllData
        yield return new WaitForSeconds(0.1f); // 延遲一點時間，以確保其他腳本進場

        LoadAllData();
    }

    private void Update() {
        if((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.S))
        {
            string json = SaveAllData();
            File.WriteAllText(savePath, json);
        }
    }

    [System.Serializable]
    public class PlayerData
    {
        public string waveNumber;
        public string MaxWaveNumber;
        public string X_Axis;
        public string Y_Axis;
        public string SandSpeed;
        public string Amp;
        public string C;
    }

    private string SaveAllData()
    {
        data.waveNumber = waveNumber.text;
        data.MaxWaveNumber = MaxWaveNumber.text;
        data.X_Axis = X_Axis.text;
        data.Y_Axis = Y_Axis.text;
        data.SandSpeed = SandSpeed.text;
        data.Amp = Amp.text;
        data.C = C.text;
        Debug.Log("save成功");
        return JsonUtility.ToJson(data);
    }

    private void LoadAllData()
    {
        LoadingCanvas.gameObject.SetActive(true);

        if (!hasLoadedData && File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            waveNumber.text = data.waveNumber;
            MaxWaveNumber.text = data.MaxWaveNumber;
            X_Axis.text = data.X_Axis;
            Y_Axis.text = data.Y_Axis;
            SandSpeed.text = data.SandSpeed;
            Amp.text = data.Amp;
            C.text = data.C;
            new_sand.Instance.Set();
        }
        else
        {
            Debug.Log("沒有東西");
        }
        hasLoadedData = true;
        StartCoroutine(HideLoadingCanvasAfterDelay(1.0f)); // 啟動延遲隱藏 Canvas 的協程
    }
    private IEnumerator HideLoadingCanvasAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadingCanvas.gameObject.SetActive(false); // 隱藏 Canvas
    }
}
