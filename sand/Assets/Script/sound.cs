using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class sound : MonoBehaviour
{
    // Start is called before the first frame update
    public Text f;  //頻率文字
    public InputField c_InputField;
    private double c = 2.19770484;
    public InputField amp;    //震幅文字
    public InputField k;    //波數文字
    private double sampleRate;  //採樣率

    private float phase;     // 相位值
    private float phaseStep; // 相位步長

    [Header("頻率 / 振幅")]
    public double frequency;  // 正弦波的頻率
    public double amplitude = 0.5d; // 正弦波的震幅

    private AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();  //取得聲音物件
        audioSource.clip = AudioClip.Create("SineWave", 44100, 1, 44100, true); //標準音頻採樣率44100 、單聲道、44100個樣本數、連續循環為真
        audioSource.pitch = 1f; //設置音調為默認值
        audioSource.Play(); //開始播放

        sampleRate = AudioSettings.outputSampleRate; //獲取採樣率
        phase = 0f; //初始化相位值為0
        UpdatePhaseStep();  //更新相位步長
    }

    // Update is called once per frame
    public void click()
    {
        bool result;
        double j;
        result = double.TryParse(k.text, out j);
        if(result)
        {
            //計算頻率 並平滑的變換
            frequency = Mathf.Lerp((float)frequency, (float)j, Mathf.PingPong(Time.time, 1f));
            Debug.Log(0.2229f*3.14f*3.14f);
            frequency = j*j*c;
            f.text=frequency.ToString();
        }
        UpdatePhaseStep();  //更新相位步長
        UpdatePitch();  //更新音調
    }
    private void UpdatePhaseStep()
    {
        float period =(float)(1d / frequency);  //計算周期
        float samplesPerPeriod = (float)(sampleRate * period);  //每個周期的樣本數
        phaseStep = (float)(2d * Math.PI / samplesPerPeriod);   //計算相位步長
    }
    private void UpdatePitch()
    {
        audioSource.pitch =(float)(frequency / 440d);   //根據頻率更新音調
    }

    //生成正弦波樣本 
    private void OnAudioFilterRead(float[] data, int channels)  //data用來儲存全部音頻樣本數據 長度為每個音頻的樣本數乘聲道數 channels為聲道數
    {  
        for (int i = 0; i < data.Length; i += channels)
        {
            float sample = (float)(Math.Sin(phase) * amplitude);    //計算樣本值

            for (int c = 0; c < channels; c++)
            {
                data[i + c] = sample;
            }
            phase += phaseStep;

            if (phase > 2f * Mathf.PI)  //避免相位過大 才能聽出每個頻率增加的變化
            {
                phase -= 2f * Mathf.PI;
            }
        }
    }

    public void changeAmp()
    {
        if(amp.text=="")
        {
            amp.text = "0.5";
        }
        bool result;
        double j;
        result = double.TryParse(amp.text, out j);
        if(result)
        {
            amplitude = j;
        }
    }

    public void changeC()
    {
        if(c_InputField.text=="")
        {
            c_InputField.text = "2.19770484";
        }
        bool result;
        double j;
        result = double.TryParse(c_InputField.text, out j);
        if(result)
        {
            c = j;
        }
        Debug.Log(c);
    }
}
