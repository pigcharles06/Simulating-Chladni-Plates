using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Numerics;
using UnityEngine.UI;

public class move : MonoBehaviour
{
    // Start is called before the first frame update
    public Complex[,] I = new Complex[200,200];
    public static float sandSpeed = 0.35f;
    
    void Start()
    {
        I = math.I4;
        StartCoroutine(FunCoroutine()); //開始執行funCoroutine 每0.35執行一次
    }
    
    private System.Collections.IEnumerator FunCoroutine()
    {
        while (true)
        {
            go();   //移動沙子
            
            yield return new WaitForSeconds(sandSpeed);
        }
    }

    public void go()
    {
        float x_now = transform.position.x;
        float y_now = transform.position.y;
        
        //將座標轉成0-200的形式
        x_now = (x_now+4f)*24.875f;
        y_now = (y_now+3f)*24.875f;

        //利用string取整數
        string s = x_now.ToString("#");
        string s1 = y_now.ToString("#");
        //string轉int
        int x = 0;
        int y = 0;
        bool result;
        int i;
        result = int.TryParse(s, out i);
        if(result)
        {
            x = Mathf.Abs(i);
        }
        result = int.TryParse(s1, out i);
        if(result)
        {
            y = Mathf.Abs(i);
        }

        if(x<=0||x>=199||y<=0||y>=199)
        {
            Destroy(gameObject);
        }
        Complex sant0 = I[x,y];
        Complex sant1 = I[x,y];
        Complex sant2 = I[x,y];
        Complex sant3 = I[x,y];

        Complex sant4 = I[x,y];
        Complex sant5 = I[x,y];

        Complex sant6 = I[x,y];
        Complex sant7 = I[x,y];
        Complex sant8 = I[x,y];
        if(x>0 && x<199 && y>0 && y<199)
        {
            sant0 = I[x,y];
            sant1 = I[x-1,y+1];
            sant2 = I[x,y+1];
            sant3 = I[x+1,y+1];

            sant4 = I[x-1,y];
            sant5 = I[x+1,y];

            sant6 = I[x-1,y-1];
            sant7 = I[x,y-1];
            sant8 = I[x+1,y-1];
        }
        else if(x==0&&y==0)
        {
            sant2 = I[x,y+1];
            sant3 = I[x+1,y+1];
            sant5 = I[x+1,y];

            sant1 = I[x,y];
            sant4 = I[x,y];
            sant6 = I[x,y];
            sant7 = I[x,y];
            sant8 = I[x,y];
        }
        else if(x==0&&y<199&&y>0)
        {
            sant1 = I[x,y];
            sant4 = I[x,y];
            sant6 = I[x,y];

            sant2 = I[x,y+1];
            sant3 = I[x+1,y+1];
            sant5 = I[x+1,y];
            sant7 = I[x,y-1];
            sant8 = I[x+1,y-1];
        }
        else if(x==0&&y==199)
        {
            sant1=I[x,y];
            sant2=I[x,y];
            sant3=I[x,y];
            sant4=I[x,y];
            sant6=I[x,y];

            sant5 = I[x+1,y];
            sant7 = I[x,y-1];
            sant8 = I[x+1,y-1];
        }
        else if(x>0&&x<199&&y==0)
        {
            sant1 = I[x-1,y+1];
            sant2 = I[x,y+1];
            sant3 = I[x+1,y+1];
            sant4 = I[x-1,y];
            sant5 = I[x+1,y];

            sant6 = I[x,y];
            sant7 = I[x,y];
            sant8 = I[x,y];
        }
        else if(x>0&&x<199&&y==199)
        {
            sant4 = I[x-1, y];
            sant5 = I[x+1, y];
            sant6 = I[x-1, y-1];
            sant7 = I[x, y-1];
            sant8 = I[x+1, y-1];

            sant1 = I[x, y];
            sant2 = I[x, y];
            sant3 = I[x, y];
        }
        else if(x==199&&y==0)
        {
            sant1 = I[x-1, y+1];
            sant2 = I[x, y+1];
            sant4 = I[x-1, y];

            sant3 = I[x, y];
            sant5 = I[x, y];
            sant6 = I[x, y];
            sant7 = I[x, y];
            sant8 = I[x, y];
        }
        else if(x==199&&y>0&&y<199)
        {
            sant1 = I[x-1, y+1];
            sant2 = I[x, y+1];
            sant4 = I[x-1, y];
            sant6 = I[x-1, y-1];
            sant7 = I[x, y-1];

            sant3 = I[x, y];
            sant5 = I[x, y];
            sant8 = I[x, y];
        }
        else if(x==199&&y==199)
        {
            sant1 = I[x, y];
            sant2 = I[x, y];
            sant3 = I[x, y];
            sant5 = I[x, y];
            sant8 = I[x, y];

            sant4 = I[x-1, y];
            sant6 = I[x-1, y-1];
            sant7 = I[x, y-1];
        }

        Complex[] arr = {sant0,sant1,sant2,sant3,sant4,sant5,sant6,sant7,sant8};
        //找出arr中最小的值
        Complex minValue = arr.OrderBy(c => c.Magnitude).FirstOrDefault();
        
        if(minValue!=sant0)
        {
            if(minValue==sant1 && sant0.Magnitude>(sant1+0.00005).Magnitude)
            {
                float x1 = ((x-1f)/24.875f)-4f;
                float y1 = ((y+1f)/24.875f)-3f;
                string s2 = Random.Range(-0.05f,0.05f).ToString("#.##");
                
                float j = 0f;
                float r1 = 0f;
                result = float.TryParse(s2, out j);
                if(result)
                {
                    r1 = j;
                }
                
                if((x1+r1)<-4||(x1+r1)>4||(y1+r1)<-3||(y1+r1)>5)
                {
                    Destroy(gameObject);
                }
                transform.position = new UnityEngine.Vector3(x1+r1,y1+r1,0);
            }
            else if(minValue==sant2 && sant0.Magnitude>(sant2+0.00005).Magnitude)
            {
                float y1 = ((y+1f)/24.875f)-3f;
                string s2 = Random.Range(-0.05f,0.05f).ToString("#.##");
                
                float j = 0f;
                float r1 = 0f;
                result = float.TryParse(s2, out j);
                if(result)
                {
                    r1 = j;
                }
                if((x+r1)<-4||(x+r1)>4||(y1+r1)<-3||(y1+r1)>5)
                {
                    Destroy(gameObject);
                }
                transform.position = new UnityEngine.Vector3(x+r1,y1+r1,0);
            }
            else if(minValue==sant3 && sant0.Magnitude>(sant3+0.00005).Magnitude)
            {
                float x1 = ((x+1f)/24.875f)-4f;
                float y1 = ((y+1f)/24.875f)-3f;
                string s2 = Random.Range(-0.05f,0.05f).ToString("#.##");
                
                float j = 0f;
                float r1 = 0f;
                result = float.TryParse(s2, out j);
                if(result)
                {
                    r1 = j;
                }
                if((x1+r1)<-4||(x1+r1)>4||(y1+r1)<-3||(y1+r1)>5)
                {
                    Destroy(gameObject);
                }
                transform.position = new UnityEngine.Vector3(x1+r1,y1+r1,0);
            }
            else if(minValue==sant4 && sant0.Magnitude>(sant4+0.00005).Magnitude)
            {
                float x1 = ((x-1f)/24.875f)-4f;
                string s2 = Random.Range(-0.05f,0.05f).ToString("#.##");
                
                float j = 0f;
                float r1 = 0f;
                result = float.TryParse(s2, out j);
                if(result)
                {
                    r1 = j;
                }
                if((x1+r1)<-4||(x1+r1)>4||(y+r1)<-3||(y+r1)>5)
                {
                    Destroy(gameObject);
                }
                transform.position = new UnityEngine.Vector3(x1+r1,y+r1,0);
            }
            else if(minValue==sant5 && sant0.Magnitude>(sant5+0.00005).Magnitude)
            {
                float x1 = ((x+1f)/24.875f)-4f;
                string s2 = Random.Range(-0.05f,0.05f).ToString("#.##");
                
                float j = 0f;
                float r1 = 0f;
                result = float.TryParse(s2, out j);
                if(result)
                {
                    r1 = j;
                }
                if((x1+r1)<-4||(x1+r1)>4||(y+r1)<-3||(y+r1)>5)
                {
                    Destroy(gameObject);
                }
                transform.position = new UnityEngine.Vector3(x1+r1,y+r1,0);
            }
            else if(minValue==sant6 && sant0.Magnitude>(sant6+0.00005).Magnitude)
            {
                float x1 = ((x-1f)/24.875f)-4f;
                float y1 = ((y-1f)/24.875f)-3f;
                string s2 = Random.Range(-0.05f,0.05f).ToString("#.##");
                
                float j = 0f;
                float r1 = 0f;
                result = float.TryParse(s2, out j);
                if(result)
                {
                    r1 = j;
                }
                if((x1+r1)<-4||(x1+r1)>4||(y1+r1)<-3||(y1+r1)>5)
                {
                    Destroy(gameObject);
                }
                transform.position = new UnityEngine.Vector3(x1+r1,y1+r1,0);
            }
            else if(minValue==sant7 && sant0.Magnitude>(sant7+0.00005).Magnitude)
            {
                float y1 = ((y-1f)/24.875f)-3f;
                string s2 = Random.Range(-0.05f,0.05f).ToString("#.##");
                
                float j = 0f;
                float r1 = 0f;
                result = float.TryParse(s2, out j);
                if(result)
                {
                    r1 = j;
                }
                if((x+r1)<-4||(x+r1)>4||(y1+r1)<-3||(y1+r1)>5)
                {
                    Destroy(gameObject);
                }
                transform.position = new UnityEngine.Vector3(x+r1,y1+r1,0);
            }
            else if(minValue==sant8 && sant0.Magnitude>(sant8+0.00005).Magnitude)
            {
                float x1 = ((x+1f)/24.875f)-4f;
                float y1 = ((y-1f)/24.875f)-3f;
                string s2 = Random.Range(-0.05f,0.05f).ToString("#.##");
                
                float j = 0f;
                float r1 = 0f;
                result = float.TryParse(s2, out j);
                if(result)
                {
                    r1 = j;
                }
                if((x1+r1)<-4||(x1+r1)>4||(y1+r1)<-3||(y1+r1)>5)
                {
                    Destroy(gameObject);
                }
                transform.position = new UnityEngine.Vector3(x1+r1,y1+r1,0);
            }
        }
        
    }
}
