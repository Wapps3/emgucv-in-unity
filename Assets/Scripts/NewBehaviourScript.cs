﻿using System.Collections;
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

    public int sizeX;
    public int sizeY;

    public int coordX;
    public int coordY;

    public ElementShape eltShape;

    public int nbrIteration;
    public BorderType border;

    public ChainApproxMethod chainApprox;
    public RetrType retraitType;

    
    //VideoCapture webcam;

    // Start is called before the first frame update
    void Start()
    {
        // string path = "D:\\M2\\Interface\\emgucv-in-unity\\Assets\\Video\\2019-10-29 17-34-21.mp4";
        video = new VideoCapture(0);   

        //video = new VideoCapture(0); //First webcam start at 0
    }

    // Update is called once per frame
    void Update()
    {
        //CvInvoke.CvtColor(image, imgGray, ColorConversion.Bgr2Gray);

        Mat image;
        
        Mat structElt = CvInvoke.GetStructuringElement(eltShape,new Size(sizeX,sizeY) , new Point(coordX,coordY) );

        //Query the frame
        if (video.IsOpened)
        {
            image = video.QueryFrame();

            //HSV img
            Mat imgHSV = image.Clone();
            CvInvoke.CvtColor(image, imgHSV, ColorConversion.Bgr2Hsv);

            //Blur
            Mat imgHSVBlur = imgHSV.Clone();
            CvInvoke.MedianBlur(imgHSVBlur, imgHSVBlur, 21);

            //New Img
            Image<Hsv, Byte> newImg = imgHSVBlur.ToImage<Hsv, Byte>();

            Hsv lowerBound = new Hsv(cMin, sMin, vMin);
            Hsv higherBound = new Hsv(cMax, sMax, vMax);

            Image<Gray, Byte> thresholdImg = newImg.InRange(lowerBound, higherBound);

            Image<Gray, Byte> thresholdImgErode = thresholdImg.Clone();

            CvInvoke.Dilate(thresholdImgErode, thresholdImgErode, structElt, new Point(-1, -1), nbrIteration, border, new MCvScalar(0));
            CvInvoke.Erode(thresholdImgErode, thresholdImgErode, structElt, new Point(-1, -1), nbrIteration, border, new MCvScalar(0));


            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            VectorOfPoint biggestContour = new VectorOfPoint();
            int biggestContourIndex = 0;
            double biggestContourArea = 0;

            Mat hierarchy = new Mat();
            CvInvoke.FindContours(thresholdImgErode, contours, hierarchy, retraitType, chainApprox);


            for (int i = 0; i < contours.Size; i++)
            {
                if (biggestContourArea < CvInvoke.ContourArea(contours[i]))
                {
                    biggestContourArea = CvInvoke.ContourArea(contours[i]);
                    biggestContour = contours[i];
                    biggestContourIndex = i;
                }
            }

            /*
            if (biggestContour.Size > 0)
            {
                int x = 0;
                int y = 0;

                for (int i = 0; i < biggestContour.Size; i++)
                {
                    x += biggestContour[i].X;
                    y += biggestContour[i].Y;
                }

                x /= biggestContour.Size;
                y /= biggestContour.Size;

                Point centroid = new Point(x, y);

                CvInvoke.Circle(image, centroid, 10, new MCvScalar(0, 0, 255));
            }*/

            //Centroid
            var moments = CvInvoke.Moments(biggestContour);
            int cx = (int)(moments.M10 / moments.M00);
            int cy = (int)(moments.M01 / moments.M00);
            Point centroid = new Point(cx, cy);
            CvInvoke.Circle(image, centroid, 10, new MCvScalar(0, 0, 255));

            CvInvoke.DrawContours(image, contours, biggestContourIndex, new MCvScalar(0, 0, 0));


            //Invoke C++ interface fonction "Imshow"
            CvInvoke.Imshow("Video view", image);

            //Invoke C++ interface fonction "Imshow"
            CvInvoke.Imshow("Video view hsv", imgHSVBlur);

            // Invoke C++ interface fonction "Imshow"
            CvInvoke.Imshow("Video view threshhold", thresholdImg);

            //Invoke C++ interface fonction "Imshow"
            CvInvoke.Imshow("Video view threshold erode + dilate", thresholdImgErode);

            //Block thread for 24 milisecond
            CvInvoke.WaitKey(24);
        }
    }

    private void OnDestroy()
    {
        video.Dispose();
        CvInvoke.DestroyAllWindows();
    }
}
