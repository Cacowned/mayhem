//  OpenCVDLL.cpp
//
//  OpenCV Wrapper functions for Mayhem
//
// (c) 2010 Microsoft Applied Sciences Group
// Author: Sven Kratz







#include "stdafx.h"

#include "OpenCVDLL.h"

using namespace OpenCVDLL;


 int OpenCVImageProvider::GetNumDevices()
			{
				return VI.listDevices();
				//return 3;
			}

void  OpenCVImageProvider::EnumerateDevices(char * devices, int * numDevices)
{
	int nDevices = VI.listDevices();

	// create buffers
	//devices = new char*[nDevices];

	int offset = 0; 
	for (int i = 0 ; i < nDevices; i++)
	{
		int length = 0 ;
		char * dName = VI.getDeviceName(i);
		

		// determine length of string
		char c = dName[length++];
		while (c != '\0')
		{
			c = dName[length++];
		}

		//create a buffer with the right size
		//char * deviceName = devices[i];
		// copy string over (note buffer offset)
		//memcpy(&devices[offset], &dName[0], length);

		strncat(devices, dName, length);
		if ( i < nDevices -1)
		{
			strncat(devices,";",1);
		}

		//offset += length;
		// add a separator
		//devices[offset++] = ';';
	}

	// hook up the pointer and the reference

	*numDevices = nDevices;
}

// stops and releases a given image device
void OpenCVImageProvider::StopCam(int device)
{
	VI.stopDevice(device);
}



 /*
 *   The OpenCVImageProvider provides image frames to all Mayhem triggers or actions
 *
 */
 void OpenCVImageProvider::InitCam(int device, int width, int height)
			{
				deviceID = device;
				printf("\tOpenCVImageProvider::InitCam -- starting \n");
				if (true)
				{

					printf("\tOpenCVImageProvider::InitCam w%d h%d\n",width, height);

					//videoInput VI;
					

					 int numDevices = VI.listDevices();

					 if (device > numDevices)
						 return;

					// set the correct deviceID
					deviceID = device;
					VI.setupDevice(device,width,height);

					// auto reconnect on freeze
					VI.setAutoReconnectOnFreeze(device, true, 4);
					width = VI.getWidth(device);
					height = VI.getHeight(device);

					// create buffer for image
					imageSize = VI.getSize(device);
					imageDataBuffer = new unsigned char[imageSize];

					// create current (global) camera image
					global_image = cvCreateImage(cvSize(width, height), 8, 3);
				
				}

			}

	  void OpenCVImageProvider::GetNextFrame(unsigned char * buffer)
	  {
		  // this should be called to prevent the cam from freezing
		  if(VI.isFrameNew(deviceID)){}
		  VI.getPixels(deviceID, buffer, false, false);

		  global_image->imageData = (char*) buffer;
		  try
		  {
			  cvConvertImage(global_image, global_image, CV_CVTIMG_FLIP);
		  }
		  catch (void * e) // TODO Get a reference to the right exception type
		  {

		  }
	  }

	  int OpenCVImageProvider::GetImageSize()
	  {
		  return imageSize;
	  }

	

 
///// MotionDetector Module

	MotionDetector::MotionDetector(int width, int height)
	{

#ifdef DEBUG
		printf("[MotionDetector::MotionDetector]\n");
#endif

		// initialize image buffers
		CvSize imgSize = cvSize(width,height);
		currentImage = cvCreateImage(imgSize, 8,3);
		movingAverage = cvCreateImage( imgSize, IPL_DEPTH_32F, 3);
		diffGray = cvCreateImage( cvSize(width,height), IPL_DEPTH_8U, 1);
		difference = cvCreateImage(cvSize(width, height), 8, 3);
		first = true;
		cBoundPoints = 0;


#ifdef DEBUG
		printf("[MotionDetector::MotionDetector] --- DONE!\n");
#endif
	}


	void MotionDetector::ProcessFrame(unsigned char * imageData, int * contourBoundPoints, int & numContours)
	{
		currentImage->imageData = (char*) imageData;

		if (first)
		{
			
			temp = cvCloneImage(currentImage);
			cvConvertScale(currentImage, movingAverage, 1.0, 0.0);
			first = false;
			return;
		}

		cvRunningAvg(currentImage, movingAverage, 0.02, 0);
		cvConvertScale(movingAverage, temp, 1.0, 0.0);
		cvAbsDiff(currentImage, temp, difference);
		cvCvtColor(difference,diffGray, CV_RGB2GRAY);

		cvSmooth(diffGray, diffGray, CV_GAUSSIAN,3,3);
		cvThreshold(diffGray, diffGray, 70, 255, CV_THRESH_BINARY);
		cvDilate(diffGray, diffGray, 0, 18);
		cvErode(diffGray, diffGray, 0, 10);

		////////////////////////////////
		IplImage * grayCtr = cvCloneImage(diffGray);
		////////////////////////////////

		// contours
		////////////////////////////////
		CvMemStorage * storage = cvCreateMemStorage(0);
		///////////////////////////////
		CvSeq * contour = 0;

		cvFindContours( grayCtr, storage, &contour, sizeof(CvContour), CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
	
		if (contour == 0)
		{
			numContours = 0;
			return;
		}
		int numPoints = contour->total * 4;
#ifdef DEBUG
		printf("Number of boundingRectPoints: %d \n", numPoints);
#endif

		//numContours = numPoints;
		
		CvRect bndRect = cvRect(0,0,0,0);
		int prevX;

		// contour array index
		int cIdx = 0;
		
		for( ; contour != 0; contour = contour->h_next )
		{

			CvPoint pt1;
			CvPoint pt2;

			//Get a bounding rectangle around the moving object.
			bndRect = cvBoundingRect(contour, 0);

			pt1.x = bndRect.x;
			pt1.y = bndRect.y;
			pt2.x = bndRect.x + bndRect.width;
			pt2.y = bndRect.y + bndRect.height;

			contourBoundPoints[cIdx++] = pt1.x;
			contourBoundPoints[cIdx++] = pt1.y;
			contourBoundPoints[cIdx++] = pt2.x;
			contourBoundPoints[cIdx++] = pt2.y;
#ifdef DEBUG
			printf("x1 %d y1 %d x2 %d y2 %d\n", pt1.x, pt1.y, pt2.x, pt2.y);
#endif
			
		}

		numContours = cIdx;

		/////////////////////////////////////
		// release memory

		cvReleaseImage(&grayCtr);
		cvFree(&storage);
		

	}