using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Emgu.CV; // the big one
using Emgu.CV.Util; // Vectors
using Emgu.CV.CvEnum; //Utility for constants
using Emgu.CV.Structure;

using System;
using System.Drawing; //Point
using System.IO; //MemoryStream, use to convert cv::Mat to Texture2D
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    VideoCapture video;

    [Range(0, 179)]
    public int cMin;
    [Range(0, 179)]
    public int cMax = 179;
    [Range(0, 255)]
    public int sMin;
    [Range(0, 255)]
    public int sMax = 255;
    [Range(0, 255)]
    public int vMin;
    [Range(0, 255)]
    public int vMax = 255;
    
    //VideoCapture webcam;

    // Start is called before the first frame update
    void Start()
    {
        //string path = "D:\\M2\\Interface\\emgucv-in-unity\\Assets\\Video\\2019-10-29 17-34-21.mp4";
        video = new VideoCapture(0);   

        //video = new VideoCapture(0); //First webcam start at 0
    }

    // Update is called once per frame
    void Update()
    {
        //CvInvoke.CvtColor(image, imgGray, ColorConversion.Bgr2Gray);

        Mat image;

        //Query the frame
        image = video.QueryFrame();

        //HSV img
        Mat imgHSV = image.Clone();
        CvInvoke.CvtColor(image, imgHSV, ColorConversion.Bgr2Hsv);

        //Blur
        Mat imgHSVBlur = imgHSV.Clone();
        CvInvoke.MedianBlur(imgHSVBlur, imgHSVBlur, 15);

        //New Img
        Image<Hsv, Byte> newImg = imgHSVBlur.ToImage<Hsv, Byte>();

        Hsv lowerBound = new Hsv(cMin,sMin,vMin);
        Hsv higherBound = new Hsv(cMax,sMax,vMax);

        Image<Gray,Byte> thresholdImg = newImg.InRange(lowerBound, higherBound);

        //Invoke C++ interface fonction "Imshow"
        CvInvoke.Imshow("Video view", image);

        //Invoke C++ interface fonction "Imshow"
        CvInvoke.Imshow("Video view hsv", imgHSV);

        //Invoke C++ interface fonction "Imshow"
        CvInvoke.Imshow("Video view hsv with blur", imgHSVBlur);

        // Invoke C++ interface fonction "Imshow"
        CvInvoke.Imshow("Video view image seuillage", thresholdImg);

        //Block thread for 24 milisecond
        CvInvoke.WaitKey(24);
    }

    private void OnDestroy()
    {
        video.Dispose();
        CvInvoke.DestroyAllWindows();
    }
}
