// test-opencv.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#include <iostream>
#include <vector>
#include <limits>

#include "stdafx.h"
#include "videoInput.h"
#include "cv.h"
#include "cvaux.h"
#include "highgui.h"

using namespace cv;
using namespace std;








int high_switch_value = 0;
int highInt = 0;
int low_switch_value = 0;
int lowInt = 0;

// Kernel size
int N = 7;

void switch_callback_h( int position ){
		highInt = position;
	}
	void switch_callback_l( int position ){
		lowInt = position;
	}
 
int main()
{
	// Edge Detection Variables
	int aperature_size = N;
	double lowThresh = 20;
	double highThresh = 40;
	
    videoInput VI;

    int numDevices = VI.listDevices();
  
	// enumerate devvices
	
	for (int i = 0; i < numDevices; i++)
	{
		char * name = VI.getDeviceName(i);
		printf("Device Nr: %d -- %s\n", i,name);
	}

     int device1= 0;


    VI.setupDevice(device1,320,240);
    int width = VI.getWidth(device1);
    int height = VI.getHeight(device1);


   

	int size = VI.getSize(device1);
	printf("Size: %d \n", size);
    unsigned char* yourBuffer = new unsigned char[VI.getSize(device1)];
   // cvNamedWindow("GrayScale");

	IplImage * image2 = cvCreateImage(cvSize(width, height),8,3);
	unsigned char* image2Buffer = new unsigned char[VI.getSize(device1)];
   
	cvNamedWindow("Output", CV_WINDOW_AUTOSIZE );


	cvNamedWindow( "Diff");
	//cvNamedWindow( "ImageB", 0 );
	cvNamedWindow( "DiffGray", CV_WINDOW_AUTOSIZE );

	CvSize imgSize = cvSize(width,height);
	
	

	// Create trackbars
	// cvCreateTrackbar( "High", "GaussianFilter", &high_switch_value, 4, switch_callback_h );
	// cvCreateTrackbar( "Low", "GaussianFilter", &low_switch_value, 4, switch_callback_l );

	bool first = true;

	IplImage*  currentImage = cvCreateImage(cvSize(width, height), 8, 3);
	IplImage*  difference = cvCreateImage(cvSize(width, height), 8, 3);


	IplImage*  motionImage; 

	IplImage* lastImage; 

	IplImage * movingAverage   = cvCreateImage( imgSize, IPL_DEPTH_32F, 3);
//	IplImage * difference;
	IplImage * diffGray = cvCreateImage( cvSize(width,height), IPL_DEPTH_8U, 1);
	IplImage * grayImageLast = cvCreateImage( cvSize(width,height), IPL_DEPTH_8U, 1);

	IplImage * temp;
	IplImage * motionHistory = cvCreateImage(cvSize(width,height), IPL_DEPTH_8U,3);

	int avgX = 0;
	int prevX = 0;
	int numPeople = 0;

	int closestToLeft = 0;
	//Same as above, but for the right.
	int closestToRight = 320;

	// character buffer and font for text output
	char wow[65];
	CvFont font;

	//Rectangle to use to put around the people.
	CvRect bndRect = cvRect(0,0,0,0);

	//Points for the edges of the rectangle.
	CvPoint pt1, pt2;

	const int MAX_CORNERS = 100;
	int win_size = 15;


	motionImage = cvCreateImage(cvSize(width, height), 8, 3);

	CvPoint2D32f* cornersA;
	CvPoint2D32f* cornersB;
	//IplImage * currentImage = cvCreateImage(cvSize(width, height), 8, 3);
	while (1)
	{
		
		VI.getPixels(device1, yourBuffer, false, false);
		//printf("11\n");
		currentImage->imageData = (char*) yourBuffer;
		// flip image
		cvConvertImage(currentImage,currentImage, CV_CVTIMG_FLIP );
		
	//	cvSmooth(currentImage, currentImage, CV_GAUSSIAN, 5,5);

		if (first)
		{
			lastImage =  currentImage;
			first = false;
			temp = cvCloneImage(currentImage);

			cvConvertScale(currentImage, movingAverage, 1.0, 0.0);
			continue;
		}


		cvRunningAvg(currentImage, movingAverage, 0.020, 0);
		cvConvertScale(movingAverage, temp, 1.0, 0.0);
		cvAbsDiff(currentImage, temp, difference);

		//cvConvertScale(difference, difference, 1.0, 0.0);

		cvCvtColor(difference, diffGray,  CV_RGB2GRAY);

		

		cvSmooth(diffGray, diffGray, CV_GAUSSIAN, 3,3);

		cvThreshold(diffGray, diffGray, 70, 255, CV_THRESH_BINARY);
		
		cvDilate(diffGray, diffGray, 0, 18);

		cvErode(diffGray,diffGray, 0, 10);
		

		IplImage * grayCtr = cvCloneImage(diffGray);
	

		// countours

		CvMemStorage* storage = cvCreateMemStorage(0);
		CvSeq* contour = 0;
		cvFindContours( grayCtr, storage, &contour, sizeof(CvContour), CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
		
		for( ; contour != 0; contour = contour->h_next )
		{
			//Get a bounding rectangle around the moving object.
			bndRect = cvBoundingRect(contour, 0);

			pt1.x = bndRect.x;
			pt1.y = bndRect.y;
			pt2.x = bndRect.x + bndRect.width;
			pt2.y = bndRect.y + bndRect.height;

			//Get an average X position of the moving contour.
			avgX = (pt1.x + pt2.x) / 2;

			
			if(avgX > 90 && avgX < 250)
			{
				//If the the previous contour was within 2 of the left boundary...
				if(closestToLeft >= 88 && closestToLeft <= 90)
				{
					//If the current X position is greater than the previous...
					if(avgX > prevX)
					{
						//Increase the number of people.
						numPeople++;

						//Reset the closest object to the left indicator.
						closestToLeft = 0;
					}
				}
				//else if the previous contour was within 2 of the right boundary...
				else if(closestToRight >= 250 && closestToRight <= 252)
				{
					//If the current X position is less than the previous...
					if(avgX < prevX)
					{
						//Increase the number of people.
						numPeople++;

						//Reset the closest object to the right counter.
						closestToRight = 320;
					}
				}

				//Draw the bounding rectangle around the moving object.
				cvRectangle(difference, pt1, pt2, CV_RGB(255,0,0), 1);
			}

			//If the current object is closer to the left boundary but still not across
			//it, then change the closest to the left counter to this value.
			if(avgX > closestToLeft && avgX <= 90)
			{
				closestToLeft = avgX;
			}

			//If the current object is closer to the right boundary but still not across
			//it, then change the closest to the right counter to this value.
			if(avgX < closestToRight && avgX >= 250)
			{
				closestToRight = avgX;
			}

			//Save the current X value to use as the previous in the next iteration.
			prevX = avgX; 
		}


		 if(cvWaitKey(15)==27) break;

		 //lastImage = cvCreateImage(cvSize(width, height), 8, 3);
		 //lastImage->imageData = (char*) yourBuffer;



		 cvShowImage( "Output", currentImage );
		 cvShowImage( "Diff", difference );
		 cvShowImage( "DiffGray", diffGray );

		 lastImage = currentImage;

	} // while
	VI.stopDevice(device1);
	cvDestroyWindow("test");
	// cvReleaseImage(&image_);

	return 0;	
}



#ifdef ignore

// "advanced algorithm --> poor results!"

//if (first)
//		{
//			lastImage = currentImage;
//			first = false;
//
//			difference = cvCloneImage(currentImage);
//
//			temp = cvCloneImage(currentImage);
//
//			cvConvertScale(currentImage, movingAverage, 1.0, 0.0);
//
//			 cornersA = new CvPoint2D32f[ MAX_CORNERS ];
//
//			 cornersB = new CvPoint2D32f[ MAX_CORNERS ];
//
//		}
//		else
//		{
//
//			printf("a\n");
//
//
//
//			cvRunningAvg(currentImage, movingAverage, 0.020, 0);
//
//			cvConvertScale(movingAverage, temp, 1.0, 0.0);
//
//			cvAbsDiff(currentImage, temp, difference);
//
//			// Get the features for tracking
//			IplImage* eig_image = cvCreateImage( imgSize, IPL_DEPTH_32F, 1 );
//			IplImage* tmp_image = cvCreateImage( imgSize, IPL_DEPTH_32F, 1 );
//
//			int corner_count = MAX_CORNERS;
//
//
//			
//
//
//			printf("b\n");
//
//			cvCvtColor(currentImage, grayImage, CV_RGB2GRAY);
//			cvCvtColor(lastImage, grayImageLast, CV_RGB2GRAY);
//			
//
//			cvGoodFeaturesToTrack( grayImage, eig_image, tmp_image, cornersA, &corner_count, 0.1, 5.0, 0, 3, 0.5, 0.04 ); 
//
//			printf("bb\n");
//
//			cvFindCornerSubPix( grayImage, cornersA, corner_count, cvSize( win_size, win_size ),
//				cvSize( -1, -1 ), cvTermCriteria( CV_TERMCRIT_ITER | CV_TERMCRIT_EPS, 10, 0.01 ) );
//
//			/*
//			cvFindCornerSubPix( grayImageLast, cornersA, corner_count, cvSize( win_size, win_size ),
//				cvSize( -1, -1 ), cvTermCriteria( CV_TERMCRIT_ITER | CV_TERMCRIT_EPS, 10, 0.01 ) ); */
//
//			printf("c\n");
//
//			// Call Lucas Kanade algorithm
//			char features_found[ MAX_CORNERS ];
//			float feature_errors[ MAX_CORNERS ];
//
//			CvSize pyr_sz = cvSize( currentImage->width+8, lastImage->height/3 );
//
//			IplImage* pyrA = cvCreateImage( pyr_sz, IPL_DEPTH_32F, 1 );
//			IplImage* pyrB = cvCreateImage( pyr_sz, IPL_DEPTH_32F, 1 );
//
//			//CvPoint2D32f* cornersB = new CvPoint2D32f[ MAX_CORNERS ];
//
//
//			printf("d\n");
//
//			cvCalcOpticalFlowPyrLK( grayImage, grayImageLast, pyrA, pyrB, cornersA, cornersB, corner_count, 
//				cvSize( win_size, win_size ), 5, features_found, feature_errors,
//				cvTermCriteria( CV_TERMCRIT_ITER | CV_TERMCRIT_EPS, 20, 0.3 ), 0 );
//
//
//			printf("e\n");
//
//			// Make an image of the results
//
//			
//			for( int i=0; i <  MAX_CORNERS ; i++ )
//			{
//				if (feature_errors[i])
//				{
//				//	printf("Error is %f/n", feature_errors[i]);
//					continue;
//				}
//				// printf("Got it/n");
//				CvPoint p0 = cvPoint( cvRound( cornersA[i].x ), cvRound( cornersA[i].y ) );
//				CvPoint p1 = cvPoint( cvRound( cornersB[i].x ), cvRound( cornersB[i].y ) );
//				cvLine( motionImage, p0, p1, CV_RGB(255,0,0), 2 );
//			}
//
//
//
//			
//			//cvShowImage( "ImageB", lastImage );
//			cvShowImage( "LKpyr_OpticalFlow", motionImage );
//			cvShowImage("Average", tmp_image);
//
//			//cvShowImage("Average", pyrA);
//			lastImage = currentImage;
//
//
//
//
//		}
//







	
 //   while(1)
 //   {
	//	// rectangle to put around ppl
	//		CvRect bndRec = cvRect(0,0,0,0);

	//		CvPoint pt1;
	//		CvPoint pt2;

	//

	//	// grab frame from camera
 //       VI.getPixels(device1, yourBuffer, false, false);
 //       colorImage->imageData = (char*) yourBuffer;

	//	// flip image
	//	cvConvertImage(colorImage,colorImage, CV_CVTIMG_FLIP );
	//	


	//	if (first)
	//	{
	//		difference = cvCloneImage(colorImage);
	//		temp = cvCloneImage(colorImage);
	//		first = false;
	//	}
	//	else
	//	{
	//		cvRunningAvg(colorImage, movingAverage, 0.02,NULL);
	//	}

	//	// convert scale of moving averge

	//	cvConvertScale(movingAverage, temp, 1.0, 0.0);

	//	// subtract the current frame from moving average

	//	cvAbsDiff(colorImage, temp, difference);

	//	// convert image to grayscale

	//	cvCvtColor(difference, gray, CV_RGB2GRAY);

	//	// convert image to b&w

	//	cvThreshold(gray, gray, 70,255, CV_THRESH_BINARY);

	//	// dilate and erode to get blobs

	//	cvDilate(gray, gray, 0,18);
	//	cvErode(gray,gray,0,10);

	//	cvShowImage("GrayScale", gray);

	//	//Find the contours of the moving images in the frame.
	//	CvMemStorage* storage = cvCreateMemStorage(0);
	//	CvSeq* contour = 0;
	//	cvFindContours( gray, storage, &contour, sizeof(CvContour), CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );



	//	//Process each moving contour in the current frame...
	//	for( ; contour != 0; contour = contour->h_next )
	//	{
	//		//Get a bounding rectangle around the moving object.
	//		bndRect = cvBoundingRect(contour, 0);

	//		pt1.x = bndRect.x;
	//		pt1.y = bndRect.y;
	//		pt2.x = bndRect.x + bndRect.width;
	//		pt2.y = bndRect.y + bndRect.height;

	//		//Get an average X position of the moving contour.
	//		avgX = (pt1.x + pt2.x) / 2;

	//		//If the contour is within the edges of the building...
	//		if(avgX > 90 && avgX < 250)
	//		{
	//			//If the the previous contour was within 2 of the left boundary...
	//			if(closestToLeft >= 88 && closestToLeft <= 90)
	//			{
	//				//If the current X position is greater than the previous...
	//				if(avgX > prevX)
	//				{
	//					//Increase the number of people.
	//					numPeople++;

	//					//Reset the closest object to the left indicator.
	//					closestToLeft = 0;
	//				}
	//			}
	//			//else if the previous contour was within 2 of the right boundary...
	//			else if(closestToRight >= 250 && closestToRight <= 252)
	//			{
	//				//If the current X position is less than the previous...
	//				if(avgX < prevX)
	//				{
	//					//Increase the number of people.
	//					numPeople++;

	//					//Reset the closest object to the right counter.
	//					closestToRight = 320;
	//				}
	//			}

	//			//Draw the bounding rectangle around the moving object.
	//			cvRectangle(colorImage, pt1, pt2, CV_RGB(255,0,0), 1);
	//		}

	//		//If the current object is closer to the left boundary but still not across
	//		//it, then change the closest to the left counter to this value.
	//		if(avgX > closestToLeft && avgX <= 90)
	//		{
	//			closestToLeft = avgX;
	//		}

	//		//If the current object is closer to the right boundary but still not across
	//		//it, then change the closest to the right counter to this value.
	//		if(avgX < closestToRight && avgX >= 250)
	//		{
	//			closestToRight = avgX;
	//		}

	//		//Save the current X value to use as the previous in the next iteration.
	//		prevX = avgX; 
	//	}


	//	//Write the number of people counted at the top of the output frame.
	//	cvInitFont(&font, CV_FONT_HERSHEY_SIMPLEX, 0.8, 0.8, 0, 2);
	//	cvPutText(colorImage, _itoa(numPeople, wow, 10), cvPoint(60, 200), &font, cvScalar(0, 0, 300));

	//	//Show the frame.
	//	cvShowImage("Output", colorImage);

	//	//Wait for the user to see it.
	//	//cvWaitKey(10);

	//	 if(cvWaitKey(15)==27) break;
#endif 
   