using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ScreenShot : MonoBehaviour
{
    public Camera captureCamera;
    public int captureWidth;  // 截圖寬度
    public int captureHeight; // 截圖高度

    public int shotPhotoNumber = 1;//紀錄目前有幾張照片
    private bool isScreenShot = false;
    public Button switchScreenShot_btn;
    public InputField input_wavenumber;
    public int ManualShotPhotoNumber = 1;
    private string folderPath_ContinuousShotScreen;
    private string folderPath_ManualShotScreen;

    void Start()
    {
        folderPath_ContinuousShotScreen = Path.Combine(Application.dataPath, "ScreenShotImageDIR");
        folderPath_ManualShotScreen = Path.Combine(Application.dataPath, "ManualScreenShotImageDIR");
        TurnOffScreenShot();
        screenShot();
    }
    
    public void screenShot()
    {
        StartCoroutine(FunCoroutine()); //開始執行funCoroutine 每0.35執行一次
    }
    private System.Collections.IEnumerator FunCoroutine()
    {
        while (isScreenShot)
        {
            if(math._isMove)
            {
                CaptureScreenshot();
                Debug.Log("我有執行");
                shotPhotoNumber++;
            }
            yield return new WaitForSeconds(move.sandSpeed);
        }
    }

    private int SetCaptureSize()
    {
        return Mathf.Min(Screen.width, Screen.height);
    }

    private RenderTexture SetRenderTexture(int captureSize)
    {
        // 創建渲染紋理
        RenderTexture renderTexture = new RenderTexture(captureSize, captureSize, 0);
        renderTexture.antiAliasing = 8;
        renderTexture.filterMode = FilterMode.Bilinear;
        renderTexture.wrapMode = TextureWrapMode.Clamp;
        captureCamera.targetTexture = renderTexture;
        //將相機目前的視圖渲染結果儲存到targetTexture
        float rectSize = (float)captureSize / Screen.height;
        captureCamera.rect = new Rect((1 - rectSize) / 2, 0, rectSize, 1);
        captureCamera.Render();

        //將設置好的RenderTexture回傳
        return renderTexture;
    }

    private void releaseResources(RenderTexture renderTexture)
    {
        // 釋放資源
        RenderTexture.active = null;
        captureCamera.targetTexture = null;
        captureCamera.rect = new Rect(0, 0, 1, 1); // 還原相機顯示區域
        RenderTexture.Destroy(renderTexture);
    }

    public void CaptureScreenshot()
    {
        int captureSize = SetCaptureSize();
        // 創建渲染紋理
        RenderTexture renderTexture = SetRenderTexture(captureSize);

        // 讀取渲染紋理內的像素
        //新增紋理2D Texture2D(寬, 高, 紋理格式, linear=false) linear=false時紋理不受光照影響 =>UI元件畫面的感覺
        Texture2D screenshotTexture = new Texture2D(captureSize, captureSize, TextureFormat.RGB24, false);

        //設置當前活動渲染紋理=> ReadPixels會讀取當前活動的渲染紋理
        RenderTexture.active = renderTexture;
        
        
        //new Rect(0, 0, captureWidth, captureHeight) 新增矩形 minX, mixY, maxX, maxY
        screenshotTexture.ReadPixels(new Rect(0, 0, captureSize, captureSize), 0, 0);
        //上面將渲染紋理讀取到screenshotTexture後 Apply會將修改後的像素數據應用到screenshotTexture
        screenshotTexture.Apply();

        if (!Directory.Exists(folderPath_ContinuousShotScreen))
        {
            Directory.CreateDirectory(folderPath_ContinuousShotScreen);
        }

        // 將截圖保存為圖片檔案
        byte[] screenshotBytes = screenshotTexture.EncodeToJPG();
        string waveNumber = input_wavenumber.text;
        string screenshotName = waveNumber + "(" + shotPhotoNumber + ")" + ".jpg";
        string screenshotPath =  Application.dataPath + "/ScreenShotImageDIR/" + screenshotName;
        System.IO.File.WriteAllBytes(screenshotPath, screenshotBytes);

        Debug.Log("截圖已保存：" + screenshotPath);
        //釋放資源
        releaseResources(renderTexture);
    }

    public void SingleScreenshot()
    {
        int captureSize = SetCaptureSize();
        // 創建渲染紋理
        RenderTexture renderTexture = SetRenderTexture(captureSize);

        // 讀取渲染紋理內的像素
        //新增紋理2D Texture2D(寬, 高, 紋理格式, linear=false) linear=false時紋理不受光照影響 =>UI元件畫面的感覺
        Texture2D screenshotTexture = new Texture2D(captureSize, captureSize, TextureFormat.RGB24, false);

        //設置當前活動渲染紋理=> ReadPixels會讀取當前活動的渲染紋理
        RenderTexture.active = renderTexture;
        
        
        //new Rect(0, 0, captureWidth, captureHeight) 新增矩形 minX, mixY, maxX, maxY
        screenshotTexture.ReadPixels(new Rect(0, 0, captureSize, captureSize), 0, 0);
        //上面將渲染紋理讀取到screenshotTexture後 Apply會將修改後的像素數據應用到screenshotTexture
        screenshotTexture.Apply();

        if (!Directory.Exists(folderPath_ManualShotScreen))
        {
            Directory.CreateDirectory(folderPath_ManualShotScreen);
        }

        // 將截圖保存為圖片檔案
        byte[] screenshotBytes = screenshotTexture.EncodeToJPG();
        string waveNumber = input_wavenumber.text;
        string screenshotName = waveNumber + "_Manual(" + ManualShotPhotoNumber + ")" + ".jpg";
        string screenshotPath =  Application.dataPath + "/ManualScreenShotImageDIR/" + screenshotName;
        System.IO.File.WriteAllBytes(screenshotPath, screenshotBytes);

        Debug.Log("截圖已保存：" + screenshotPath);

        //釋放資源
        releaseResources(renderTexture);
        ManualShotPhotoNumber++;
    }
    
    public void switchScreenShot()
    {
        if(isScreenShot)
        {
            TurnOffScreenShot();
        }
        else
        {
            TurnOnScreenShot();
        }
    }

    private void TurnOffScreenShot()
    {
        isScreenShot = false;
        Color colorA =  switchScreenShot_btn.GetComponent<Image>().color;
        colorA.a = 0.5f;
        switchScreenShot_btn.GetComponent<Image>().color = colorA;
    }

    private void TurnOnScreenShot()
    {
        isScreenShot = true;
        screenShot();
        Color colorA =  switchScreenShot_btn.GetComponent<Image>().color;
        colorA.a = 1f;
        switchScreenShot_btn.GetComponent<Image>().color = colorA;
    }
}
