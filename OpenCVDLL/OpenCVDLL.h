// OpenCVDLL.h
/*
*  OpenCV C++/CLI Wrapper for Mayhem Application
*
* (c) 2010 Microsoft Applied Sciences Group
*
* Author: Sven Kratz
*
*/


#pragma once
#include "stdafx.h"
#include <iostream>
#include <vector>
#include <limits>

#include "videoInput.h"
#include <opencv/cv.h>
#include <opencv/cvaux.h>
#include <opencv/highgui.h>

using namespace std;
using namespace cv;
using namespace System;


#undef DEBUG

namespace OpenCVDLL {

	public ref class MotionDetector
	{
		private:
			IplImage * currentImage;
			IplImage * diffGray;
			IplImage * difference;
			IplImage * movingAverage;
			IplImage * temp;
			CvPoint2D32f* cornersA;
			CvPoint2D32f* cornersB;
			bool first;

			int * cBoundPoints;

			

		public:

			MotionDetector(int width, int height);
			void ProcessFrame(unsigned char * imageData, int * contourPoints, int & numContours);


	};


	public class OpenCVImageProvider
	{
		private:
			videoInput  VI;
			bool threadRunning;

		public:
			 IplImage * global_image;
			 unsigned char * imageDataBuffer;
			 static OpenCVImageProvider instance;
			 bool initialized;
			 int width;
			 int height;
			 int imageSize;


			 // Deprecated: only gests number of devices presnt
			 int GetNumDevices();

			 // this method is better than the above one -- it gets the number of devices
			 // and their names as a string
			 void EnumerateDevices(char * devices, int * numDevices);

			 void InitCam(int device, int width, int height);

			 void StopCam(int device);

			 void GetNextFrame(int deviceID, unsigned char * buffer); 

			
			 int GetImageSize();			
	};



	public ref class OpenCVBindings
	{

		private:
			static  OpenCVImageProvider * i;

			static bool provider_initialized = false;
		

		// TODO: Add your methods for this class here.
		public:

			static void Initialize()
			{
				printf("InitOpenCVBindings\n");
				i = new OpenCVImageProvider();
				printf("Cam Initialized\n");
				provider_initialized = true;

			}

			static void EnumerateDevices(char * device_list, int * numDevices)
			{
				if (provider_initialized)
				{
					printf("Enumerating Devices \n");
					i->EnumerateDevices(device_list, numDevices);
				}
				else
				{
					printf("Provider is not initialized\n");
				}
			}

			static void StopCamera(int deviceId)
			{
				if (provider_initialized)
				{
					//provider_initialized = false;
					printf("Stopping Cam %d\n", deviceId);
					i->StopCam(deviceId);
				}
				else
				{
					printf("Provider is not initialized\n");
				}
			}


			static bool InitCapture(int deviceId, int cWidth, int cHeight)
			{
				printf("InitOpenCVBindings\n");
				
				if (provider_initialized)
				{
					i->InitCam(deviceId,cWidth,cHeight);
					printf("Cam Initialized\n");
					return true;
				}
				else
				{
					printf("Provider not initialized\n");
					return false;
				}
			
			}

			/*
			*  Request next frame from imaging device with deviceID
			*/
			static void GetNextFrame(int deviceID, unsigned char * imageDataBuffer)
			{
				try
				{
					i->GetNextFrame(deviceID,imageDataBuffer);
				}
				catch (void * e)
				{

				}
			}

			/*
			* Requests next frame from default device (deviceID 0) 
			*/
			static void GetNextFrame(unsigned char * imageDataBuffer)
			{
				try
				{
					i->GetNextFrame(0,imageDataBuffer);
				}
				catch (void * e)
				{

				}
			}
			
			static int GetImageSize()
			{
				/*
				* Return image size nr of bytes
				*/
				int size = i->GetImageSize();
				printf("GetImageSize : size is %d\n",size);
				return size;
			}

			static int foo()
			{
				return 42;
			}

	};

}
