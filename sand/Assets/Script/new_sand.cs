using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class new_sand : MonoBehaviour
{
    public GameObject sand;     //沙子

    public GameObject sandbox; //用來裝沙子的父物件
    public InputField howmuch;    //設定沙子數量
    public AudioSource audioSource; //頻率聲音
    private Button stopBtn;
    private Button continueBtn;
    private bool isMove;
    
    [Header("沙子移動速度")]
    public InputField sandSpeed_text;
    
    [Header("沙數量/隨機生成區域")]
    //x邊界-4、4
    //y邊界-3、5
    public int amount=1000; //沙子數量
    [SerializeField] public float x1;
    [SerializeField] public float x2;
    [SerializeField] public float y1;
    [SerializeField] public float y2;
    private static new_sand instance;

    // 確保只有一個實例
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static new_sand Instance
    {
        get { return instance; }
    }
    
    private void Start() {
        stopBtn = GameObject.Find("Stop_btn").GetComponent<Button>();
        continueBtn = GameObject.Find("Continue_btn").GetComponent<Button>();
        audioSource = GameObject.Find("SoundCreater").GetComponent<AudioSource>();
        TurnOffButton(continueBtn);
    }
    //生成沙子
    public void generateSand()
    {   
        bool result;
        int i;
        result = int.TryParse(howmuch.text, out i);
        if(result)
        {
            amount = i;
            Debug.Log(i);
        }
        gosand();
    }
    //更改沙子移動速度
    public void changeSandSeep()
    {
        if(sandSpeed_text.text=="")
        {
            sandSpeed_text.text = "0.35";
        }
        bool result;
        float i;
        result = float.TryParse(sandSpeed_text.text, out i);
        if(result)
        {
            move.sandSpeed = i;
        }
    }
    //移除沙子
    public void dty()
    {
        GameObject[] all = GameObject.FindGameObjectsWithTag("Player");
        for(int i=0;i<all.Length;i++)
            Destroy(all[i]);
    }
    //生成的func
    private void gosand()
    {    
        for(int i=0;i<amount;i++)
        {
            string s1 = Random.Range(x1,x2).ToString("#.##");
            string s2 = Random.Range(y1,y2).ToString("#.##");
            bool result;
            float j;
            float xf=0f;
            float yf=0f;
            result = float.TryParse(s1, out j);
            if(result)
            {
                xf=j;
            }
            result = float.TryParse(s2, out j);
            if(result)
            {
                yf=j;
            }
            Instantiate(sand,new Vector3(xf,yf,0),Quaternion.identity,sandbox.transform);
        }
    }
    //暫停
    public void stop()
    {
        if(audioSource.enabled == true)
        {
            TurnOffButton(stopBtn);
            TurnOnButton(continueBtn);
            audioSource.enabled = false;
            math.isPause = true;
            isMove = math._isMove;
            math._isMove = false;
            Time.timeScale = 0f;
        }
    }

    public void ContinueRun()
    {
        if(audioSource.enabled == false)
        {
            TurnOffButton(continueBtn);
            TurnOnButton(stopBtn);
            audioSource.enabled = true;
            math.isPause = false;
            math._isMove = isMove;
            Time.timeScale = 1f;
        }
    }

    public void Set()
    {
        changeSandSeep();
        math mathGameManger = GameObject.Find("gameManager").GetComponent<math>();
        mathGameManger.change_xy();
        sound SoundCreater = GameObject.Find("SoundCreater").GetComponent<sound>();
        SoundCreater.changeAmp();
        SoundCreater.changeC();
    }

    private void TurnOffButton(Button btn)
    {
        Color colorA =  btn.GetComponent<Image>().color;
        colorA.a = 0.5f;
        btn.GetComponent<Image>().color = colorA;
    }

    private void TurnOnButton(Button btn)
    {
        Color colorA =  btn.GetComponent<Image>().color;
        colorA.a = 1f;
        btn.GetComponent<Image>().color = colorA;
    }
}

