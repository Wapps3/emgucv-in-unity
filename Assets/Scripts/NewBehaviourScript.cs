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

public class NewBehaviourScript : MonoBehaviour
{
    VideoCapture video;
    
    //VideoCapture webcam;

    // Start is called before the first frame update
    void Start()
    {
        string path = "D:\\M2\\Interface\\emgucv-in-unity\\Assets\\Video\\2019-10-29 17-34-21.mp4";
        video = new VideoCapture(0);   

        //video = new VideoCapture(0); //First webcam start at 0
    }

    // Update is called once per frame
    void Update()
    {
        Mat image;

        //Query the frame
        image = video.QueryFrame();

        //Gray img
        Mat imgGray = image.Clone();
        CvInvoke.CvtColor(image,imgGray,ColorConversion.Bgr2Gray);

        //HSV img
        Mat imgHSV = image.Clone();
        CvInvoke.CvtColor(image, imgHSV, ColorConversion.Bgr2Hsv);

        //Blur
        Mat imgHSVBlur = imgHSV.Clone();
        CvInvoke.GaussianBlur(imgHSVBlur, imgHSVBlur, new Size(21,21) ,0,0, BorderType.Reflect101);

        //New Img
        Image<Bgr, Byte> newImg = image.ToImage<Bgr, Byte>();

        Hsv lowerBound = new Hsv(60,0,0);
        Hsv upperBound = new Hsv(180,255,255);

        newImg.inRange(lowerBound, upperBound);

       

        //Invoke C++ interface fonction "Imshow"
        CvInvoke.Imshow("Video view", image);

        //Invoke C++ interface fonction "Imshow"
        CvInvoke.Imshow("Video view gray", imgGray);

        //Invoke C++ interface fonction "Imshow"
        CvInvoke.Imshow("Video view hsv", imgHSV);

        //Invoke C++ interface fonction "Imshow"
        CvInvoke.Imshow("Video view hsv with blur", imgHSVBlur);

        // Invoke C++ interface fonction "Imshow"
        CvInvoke.Imshow("Video view image format", newImg);

        //Block thread for 24 milisecond
        CvInvoke.WaitKey(24);
    }

    private void OnDestroy()
    {
        video.Dispose();
        CvInvoke.DestroyAllWindows();
    }
}
