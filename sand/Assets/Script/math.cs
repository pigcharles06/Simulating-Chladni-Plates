using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Numerics;
using System.Linq;
using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;


public class math : MonoBehaviour
{
    [Header("擷取畫面")]
    public static bool _isMove = false;

    [Header("連續變換波數")]
    [SerializeField]public int photo = 5;   //波數連續變化的總張數
    public bool isWaiting = false;  //上一張波數是否移動完
    public static bool isPause = false; //暫停
    public InputField MaxWaveText;        //輸入最大波數(用來連續變換)
    public sound Sound;     //用來呼叫sound class的函示 物件放soundcreater
    public InputField WaveText;        //輸入波數

    [Header("高峰/低峰參數")]
    public Image mes;        //高峰低峰值的預制物
    public GameObject highcontent;        //放高峰的scroll box的content
    public GameObject lowcontent;          //放低峰的scroll box的content
    public float WaveNumber;        //波數值
    public float MaxWaveNumber;        //最大波數值
    public GameObject K_numberText_Panel;
    public Canvas canvas;

    [Header("數學參數")]
    //數學式的各種參數
    float L = 1f;
    static public float x0 = 0.5f;
    static public float y0 = 0.5f;
    float epsilon = Mathf.Pow(10,-7);
    float s = 15f;

    Complex psi0 = 0;       //用來計算k的
    Complex k0 = 0;         //用來計算k的
    float s0 = 100f;        //用來計算k的

    float pi = 3.141592653589793f;
    float gamma = 0.01f;
    public float[] x = new float[200];
    public float[] y = new float[200];
    public Complex[,] I = new Complex[200,200];
    public static Complex[,] I2 = new Complex[200,200];
    public static Complex[,] I4 = new Complex[200,200];
    public Complex nor = 0;
    public Complex[,] inv = new  Complex[200,200];
    public Complex[,] psi  = new Complex[200,200];
    Complex com = new Complex(0,1);

    //用來畫k值高峰低峰圖的參數
    [Header("畫圖")]
    public GameObject graph;    //放 讓k圖對齊位置 的物件
    public GameObject graphX;
    public GameObject graphY;
    
    [SerializeField]public float scaleX = 0.025f;    // 縮放x軸
    [SerializeField]public float scaleY = 0.5f;    // 縮小y軸
    
    private LineRenderer lineRenderer;  //畫圖組件
    private LineRenderer lineRendererX;  //畫圖組件
    private LineRenderer lineRendererY;  //畫圖組件

    [Header("中心偏移量")]
    //x0,y0偏移量的參數
    public InputField xinput;
    public InputField yinput;
    public Image WarningPanel;
    
    public double func(int m, int n, double x, double y){
        //(2/L)*Mathf.Cos(m*pi*x/L)*
        double r =(2/L)*Math.Cos((double)m*Math.PI*(double)x/L)*Math.Cos((double)n*Math.PI*(double)y/L);
        return r;
    }
    public float kmn(int m, int n){
        float k = pi/L*(Mathf.Sqrt(Mathf.Pow(n,2)+Mathf.Pow(m,2)));
        return k;
    }
    
    public Complex countk(Complex k)
    {
        psi0 = 0;
        k0 = 0;
        s0 = 100;
        for(int n=0; n<s0; n++)
        {
            for(int m=0; m<s0; m++)
            {
                psi0+=Math.Pow(func(m,n,x0,y0),2)/(Complex.Pow(k,2)-Math.Pow(kmn(m,n),2)+2*com*gamma*k);
            }
        }
        k0 = Complex.Abs(psi0/(1+3.3*psi0));
        return k0;
    }
    public void plotk()
    {
        //1-50 每個間隔為0.1的陣列
        Complex[] complexArray = new Complex[(int)((50f - 1f) / 0.1f) + 1];
        
        //裝高峰與低峰得波數
        List<Complex> highwave = new List<Complex>();
        List<Complex> lowave = new List<Complex>();
        
        //將陣列填值
        for (int i = 0; i < complexArray.Length; i++)
        {
            double realPart = 1 + i * 0.1;
            complexArray[i] = new Complex(realPart, 0);
        }
        //再生成一個裝計算k值陣列
        Complex[] f = new Complex[complexArray.Length];

        for(int i=0; i<complexArray.Length; i++)
        {
            f[i]=countk(complexArray[i]);
        }
        plotK_Y(f);

        lineRenderer = graph.GetComponent<LineRenderer>();

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0; // 清除之前的線段位置
            bool firstOne = true;
            foreach (Transform child in highcontent.transform)
            {
                if(firstOne==true)
                {
                    firstOne=false;
                    continue;
                }
                Destroy(child.gameObject); // 刪除子物件
            }
            firstOne = true;
            foreach (Transform child in lowcontent.transform)
            {
                if(firstOne==true)
                {
                    firstOne=false;
                    continue;
                }
                Destroy(child.gameObject); // 刪除子物件
            }
        }
        else
        {
            //把LineRenderer組件 加入graph物件
            lineRenderer = graph.AddComponent<LineRenderer>();

            // 設置 LineRenderer 的材質和顏色
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;

            // 設置 LineRenderer 的寬度
            lineRenderer.startWidth = 0.06f;
            lineRenderer.endWidth = 0.06f;
        }
        
        //將畫圖的起始位置設置在graph的位置
        UnityEngine.Vector3 backgroundOffset = graph.transform.position;

        
        // 設置 LineRenderer 的頂點數量
        lineRenderer.positionCount = f.Length;
        for (int i = 0; i < f.Length; i++)
        {
            float x = i * scaleX+backgroundOffset.x;
            //complex沒辦法直接放在vector裡 需要先轉乘float
            float realPart = (float)f[i].Real;
            float imaginaryPart = (float)f[i].Imaginary;
            //將圖形轉換成log形式
            float y =  Mathf.Log10(realPart+imaginaryPart)* scaleY+backgroundOffset.y;
            //設置點位
            lineRenderer.SetPosition(i, new UnityEngine.Vector3(x, y, 0f));
        }
        //計算各個峰值的頂點
        for(int i=0; i<f.Length; i++)
        {
            if(i==0 || i==f.Length-1)
                continue;
            //complex需要使用Magnitude才能比大小
            if(f[i].Magnitude>f[i+1].Magnitude&&f[i].Magnitude>f[i-1].Magnitude)
            {
                float x = i * scaleX+backgroundOffset.x;
                //complex沒辦法直接放在vector裡 需要先轉乘float
                float realPart = (float)f[i].Real;
                float imaginaryPart = (float)f[i].Imaginary;
                //將圖形轉換成log形式
                float y =  Mathf.Log10(realPart+imaginaryPart)* scaleY+backgroundOffset.y;

                //生成頂點預覽文字Panel
                Camera mainCamera = Camera.main;
                UnityEngine.Vector3 K_screenPoint = mainCamera.WorldToScreenPoint(new UnityEngine.Vector2(x,y));
                UnityEngine.Vector2 K_screenPosition = new UnityEngine.Vector2(K_screenPoint.x, K_screenPoint.y);
                GameObject K_prefab = Instantiate(K_numberText_Panel, K_screenPoint, UnityEngine.Quaternion.Euler(0f, 0f, -45f), canvas.transform);
                Text K_text = K_prefab.GetComponentInChildren<Image>().GetComponentInChildren<Text>();
                K_text.text = complexArray[i].Real.ToString() + " , " +  Mathf.Log10(realPart+imaginaryPart).ToString("F2");
                K_prefab.name = complexArray[i].Real.ToString() + "K_prefab";
                K_prefab.SetActive(false);


                highwave.Add(complexArray[i]);
            }
            else if(f[i].Magnitude<f[i+1].Magnitude&&f[i].Magnitude<f[i-1].Magnitude)
            {
                float x = i * scaleX+backgroundOffset.x;
                //complex沒辦法直接放在vector裡 需要先轉乘float
                float realPart = (float)f[i].Real;
                float imaginaryPart = (float)f[i].Imaginary;
                //將圖形轉換成log形式
                float y =  Mathf.Log10(realPart+imaginaryPart)* scaleY+backgroundOffset.y;

                //生成頂點預覽文字Panel
                Camera mainCamera = Camera.main;
                UnityEngine.Vector3 K_screenPoint = mainCamera.WorldToScreenPoint(new UnityEngine.Vector2(x,y));
                UnityEngine.Vector2 K_screenPosition = new UnityEngine.Vector2(K_screenPoint.x, K_screenPoint.y);
                GameObject K_prefab = Instantiate(K_numberText_Panel, K_screenPoint, UnityEngine.Quaternion.Euler(0f, 0f, -45f), canvas.transform);
                Text K_text = K_prefab.GetComponentInChildren<Image>().GetComponentInChildren<Text>();
                K_text.text = complexArray[i].Real.ToString() + " , " +  Mathf.Log10(realPart+imaginaryPart).ToString("F2")+"e";
                K_prefab.name = complexArray[i].Real.ToString() + "K_prefab";
                K_prefab.SetActive(false);

                lowave.Add(complexArray[i]);
            }
        }
        //生成低峰高峰的預制物件 並賦值
        foreach (Complex complex in highwave)
        {
            Image newmes = Instantiate(mes,highcontent.transform);
            Text textComponent = newmes.GetComponentInChildren<Button>().GetComponentInChildren<Text>();

            textComponent.text = complex.Real.ToString();
        }
        foreach (Complex complex in lowave)
        {
            Image newmes = Instantiate(mes,lowcontent.transform);
            Text textComponent = newmes.GetComponentInChildren<Text>().GetComponentInChildren<Text>();

            textComponent.text = complex.Real.ToString();
        }
    }

    public void plotK_X(Complex[] f, float minY)
    {
        lineRendererX = graphX.GetComponent<LineRenderer>();

        if (lineRendererX != null)
        {
            lineRendererX.positionCount = 0; // 清除之前的線段位置
        }
        else
        {
            // 把 LineRenderer 組件加入 graph 物件
            lineRendererX = graphX.AddComponent<LineRenderer>();
            // 設置 LineRenderer 的材質和顏色
            lineRendererX.material = new Material(Shader.Find("Sprites/Default"));
            lineRendererX.startColor = Color.black;
            lineRendererX.endColor = Color.black;

            // 設置 LineRenderer 的寬度
            lineRendererX.startWidth = 0.06f;
            lineRendererX.endWidth = 0.06f;
        }
        //將畫圖的起始位置設置在graph的位置
        UnityEngine.Vector3 backgroundOffset = graph.transform.position;
        // 設置 LineRenderer 的頂點數量
        lineRendererX.positionCount = f.Length;


        for (int i = 0; i < f.Length; i++)
        {
            float x = i * scaleX+backgroundOffset.x;
            //將圖形轉換成log形式
            float y =  Mathf.Log10(minY)* scaleY+backgroundOffset.y;
            
            //設置點位
            lineRendererX.SetPosition(i, new UnityEngine.Vector3(x, y, 0f));
        }
    }

    public void plotK_Y(Complex[] f)
    {
        lineRendererY = graphY.GetComponent<LineRenderer>();

        if (lineRendererY != null)
        {
            lineRendererY.positionCount = 0; // 清除之前的線段位置
        }
        else
        {
            // 把 LineRenderer 組件加入 graph 物件
            lineRendererY = graphY.AddComponent<LineRenderer>();
            // 設置 LineRenderer 的材質和顏色
            lineRendererY.material = new Material(Shader.Find("Sprites/Default"));
            lineRendererY.startColor = Color.black;
            lineRendererY.endColor = Color.black;

            // 設置 LineRenderer 的寬度
            lineRendererY.startWidth = 0.06f;
            lineRendererY.endWidth = 0.06f;
        }
        //將畫圖的起始位置設置在graph的位置
        UnityEngine.Vector3 backgroundOffset = graphY.transform.position;
        // 設置 LineRenderer 的頂點數量
        lineRendererY.positionCount = f.Length;
        float minY = 100;
        for (int i = 0; i < f.Length; i++)
        {
            float x = backgroundOffset.x;
            //complex沒辦法直接放在vector裡 需要先轉乘float
            float realPart = (float)f[i].Real;
            float imaginaryPart = (float)f[i].Imaginary;
            if(minY>realPart+imaginaryPart)
            {
                minY = realPart+imaginaryPart;
            }
            //將圖形轉換成log形式
            float y =  Mathf.Log10(realPart+imaginaryPart)* scaleY+backgroundOffset.y;
            //設置點位
            lineRendererY.SetPosition(i, new UnityEngine.Vector3(x, y, 0f));
        }
        plotK_X(f, minY);
    }

    //更改中心點
    public void change_xy()
    {
        float i,j,a,b;
        a = 0.5f;
        b = 0.5f;
        if(xinput.text=="")
        {
            xinput.text = "0.5";
        }
        if(yinput.text=="")
        {
            yinput.text = "0.5";
        }
        bool result;
        result = float.TryParse(xinput.text, out i);
        if(result)
        {
            a = i;
        }
        result = float.TryParse(yinput.text, out j);
        if(result)
        {
            b = j;
        }

        if((a!=0||b!=0)&&(a!=1||b!=1))
        {
            x0 = a;
            y0 = b;
        }
        else
        {
            GameObject canvas = GameObject.Find("Canvas");
            Instantiate(WarningPanel, canvas.transform);
            x0 = 0.5f;
            y0 = 0.5f;
            xinput.text = x0.ToString();
            yinput.text = y0.ToString();
        }
    }

    //連續計算
    public async void run_lot() //須解決問題=>要讓他重複按時 取消上一個Task
    {
        int i=0;
        bool result;
        float j;
        photo = 5;
        float MaxMinusMin_AVG = 0;
        Button runBtn = GameObject.Find("Run_btn").GetComponent<Button>();
        
        TurnOffButton(runBtn);
        result = float.TryParse(WaveText.text, out j);
        if(result)
        {
            WaveNumber = j;
        }

        if(MaxWaveText.text == "")
        {
            MaxWaveNumber = WaveNumber;
            photo = 1;
        }
        else{
            result = float.TryParse(MaxWaveText.text, out j);
            if(result)
            {
                MaxWaveNumber = j;
            }
            MaxMinusMin_AVG = (MaxWaveNumber-WaveNumber)/(photo-1);
        }

        
        while (i < photo)
        {
            if(isPause==false){
                WaveText.text = (WaveNumber).ToString();
                mai(WaveNumber); //開始計算 並立即執行下一行
                
                await Task.Delay(3000);
                Sound.click();          //等待兩秒 再播放該頻率聲音
                
                while(isPause)          //暫停
                   await Task.Delay(500);

                isWaiting = true;       //正在等待
                await Task.Delay(7000); //每張等待7秒
                isWaiting = false;      //等待完成
                
                WaveNumber = WaveNumber+MaxMinusMin_AVG;
                i++;
                if(WaveText.text == MaxWaveText.text)
                    break;
            }
            await Task.Delay(500);  //避免暫停時的bug
        }
        TurnOnButton(runBtn);

        await Task.Delay(5000);
        _isMove = false;
    }
    
    public Task mai(float k){
        return Task.Run(async () =>     //可以在背後事先運算
        {
            nor=0;
            psi  = new Complex[200,200];
            inv  = new Complex[200,200];
            I    = new Complex[200,200];
            for(int i=0;i<200;i++){
                x[i]=L*i/200;
                for(int j=0;j<200;j++){
                    y[j]=L*j/200;
                    for(int n=0;n<s;n++){
                        for(int m=0;m<s;m++){
                            psi[i,j] += func(m, n, x[i], y[j])*func(m,n,x0,y0)/(Mathf.Pow(k,2)-Mathf.Pow(kmn(m,n),2)+2*com*gamma*k);
                        }
                    }
                    I[i,j]=Complex.Abs(psi[i,j]);
                    I[i,j]=I[i,j]*I[i,j];
                    I2[i,j]=I[i,j];
                    nor = nor + I[i,j];
                }
            }
            _isMove = true;
            Array.Copy(I2, I4, I2.Length);
            
            if (isWaiting)          //如果上一張還沒跑完就先繼續等待
            {
                await Task.Delay(7000);
            }
        });
    }

    public void resetOfAll()
    {
        photo = 5;   //波數連續變化的總張數
        isWaiting = false;  //上一張波數是否移動完
        isPause = false; //暫停
        
        L = 1f;
        x0 = 0.5f;
        y0 = 0.5f;
        epsilon = Mathf.Pow(10,-7);
        s = 15f;

        psi0 = 0;       //用來計算k的
        k0 = 0;         //用來計算k的
        s0 = 100f;        //用來計算k的

        pi = 3.141592653589793f;
        gamma = 0.01f;
        x = new float[200];
        y = new float[200];
        I = new Complex[200,200];
        I2 = new Complex[200,200];
        I4 = new Complex[200,200];
        nor = 0;
        inv = new  Complex[200,200];
        psi  = new Complex[200,200];
        com = new Complex(0,1);
        move.sandSpeed = 0.35f;
        _isMove = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("SampleScene");
    }

    private void TurnOffButton(Button btn)
    {
        btn.enabled = false;
        Color colorA =  btn.GetComponent<Image>().color;
        colorA.a = 0.5f;
        btn.GetComponent<Image>().color = colorA;
    }

    private void TurnOnButton(Button btn)
    {
        btn.enabled = true;
        Color colorA =  btn.GetComponent<Image>().color;
        colorA.a = 1f;
        btn.GetComponent<Image>().color = colorA;
    }
}
