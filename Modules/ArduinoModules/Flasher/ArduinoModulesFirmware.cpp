/*
 * Mayhem firmware for the Arduino platform
 * using the Arduino Uno board.
 *
 * This firmware is intended for use with Mayhem (http://www.makemayhem.com)
 * and supports DigitalOutput and DigitalInput pin controls. Follow the
 * instructions in Mayhem to flash your device with this firmware.
 *
 * Gabriel Reyes <greyes@gatech.edu>
 * http://www.makemayhem.com
 * June 17, 2012
 *
 * DISCLAIMER: Release for demonstration and education purposes only. You are fully
 * responsible for modifying and using this software.
 *
 * Copyright (c) 2012, The Outercurve Foundation
 * For more details on the license: 
 * http://www.outercurve.org/
 *
 * Version 1.0.0
 */

// Pin definition
#include "Arduino.h"
void setup();
void loop();
void setDigitalOutputs(int byteReceived);
void serialEvent();
int pin0 = 0;
int pin1 = 1;
int pin2 = 2;
int pin3 = 3;
int pin4 = 4;
int pin5 = 5;
int pin6 = 6;
int pin7 = 7;
int pin8 = 8;
int pin9 = 9;
int pin10 = 10;
int pin11 = 11;
int pin12 = 12;
int pin13 = 13;

// Variable definition
unsigned char pin2State;
unsigned char pin2StateOld;
unsigned char pin3State;
unsigned char pin3StateOld;
unsigned char pin4State;
unsigned char pin4StateOld;
unsigned char pin5State;
unsigned char pin5StateOld;
unsigned char pin6State;
unsigned char pin6StateOld;
unsigned char pin7State;
unsigned char pin7StateOld;

/*
 setup() only runs once and is used to configure the
 input and output pins used in this firmware. The code below 
 also initializes the serial port at 9600 baud.
*/
void setup()
{
  // Unused pins
  // Pin0/Pin1 are reserved for the serial port and currently unused
  // Analog Pins are currently unused
  
  // Define input pins
  pinMode(pin2, INPUT_PULLUP);
  pinMode(pin3, INPUT_PULLUP);
  pinMode(pin4, INPUT_PULLUP);
  pinMode(pin5, INPUT_PULLUP);  
  pinMode(pin6, INPUT_PULLUP);  
  pinMode(pin7, INPUT_PULLUP);  

  // Define output pins
  pinMode(pin8, OUTPUT);  
  pinMode(pin9, OUTPUT);  
  pinMode(pin10, OUTPUT);  
  pinMode(pin11, OUTPUT);  
  pinMode(pin12, OUTPUT);  
  pinMode(pin13, OUTPUT);  
  
  // Setup the serial port
  Serial.begin(9600);
  
  // Initialize and clear pin state variables
  pin2State = pin3State = pin4State = pin5State = pin6State = pin7State = 0;
  pin2StateOld = pin3StateOld = pin4StateOld = pin5StateOld = pin6StateOld = pin7StateOld = 0;
}

/*
 Loop runs continuously similar to a while(1) loop. The
 code inside the loop() polls the input pins and sends
 Mayhem a byte indicating when the digital input pin has
 been toggled, turned high, turned low.
*/
void loop()
{   
  // Initialize and save the input pin states--------------------------------------------------------
  pin2StateOld = pin2State;
  pin3StateOld = pin3State;  
  pin4StateOld = pin4State;
  pin5StateOld = pin5State;
  pin6StateOld = pin6State;
  pin7StateOld = pin7State;

  // Poll the input pins; Goes High = 0x01; Goes Low = 0x00------------------------------------------
  int pin2Read = digitalRead(pin2);
  if (pin2Read == HIGH) pin2State = 0x01; else pin2State = 0x00;
  
  int pin3Read = digitalRead(pin3);
  if (pin3Read == HIGH) pin3State = 0x01; else pin3State = 0x00;

  int pin4Read = digitalRead(pin4);
  if (pin4Read == HIGH) pin4State = 0x01; else pin4State = 0x00;

  int pin5Read = digitalRead(pin5); 
  if (pin5Read == HIGH) pin5State = 0x01; else pin5State = 0x00;

  int pin6Read = digitalRead(pin6); 
  if (pin6Read == HIGH) pin6State = 0x01; else pin6State = 0x00;

  int pin7Read = digitalRead(pin7); 
  if (pin7Read == HIGH) pin7State = 0x01; else pin7State = 0x00;
  
  // Change in input pin? Is High = 0x01; Is Low = 0x00; Serial.write Notification to Mayhem----------
  if((pin2StateOld != pin2State))
  {
    if(pin2State == 0x01) Serial.write(0x50);
    if(pin2State == 0x00) Serial.write(0x60);
  }
  
  // Change in input pin? Is High = 0x01; Is Low = 0x00; Serial.write Notification to Mayhem----------
  if((pin3StateOld != pin3State))
  {
    if(pin3State == 0x01) Serial.write(0x51);
    if(pin3State == 0x00) Serial.write(0x61);
  }

  // Change in input pin? Is High = 0x01; Is Low = 0x00; Serial.write Notification to Mayhem----------
  if((pin4StateOld != pin4State))
  {
    if(pin4State == 0x01) Serial.write(0x52);
    if(pin4State == 0x00) Serial.write(0x62);
  }

  // Change in input pin? Is High = 0x01; Is Low = 0x00; Serial.write Notification to Mayhem----------
  if((pin5StateOld != pin5State))
  {
    if(pin5State == 0x01) Serial.write(0x53);
    if(pin5State == 0x00) Serial.write(0x63);
  }
  
  // Change in input pin? Is High = 0x01; Is Low = 0x00; Serial.write Notification to Mayhem----------
  if((pin6StateOld != pin6State))
  {
    if(pin6State == 0x01) Serial.write(0x54);
    if(pin6State == 0x00) Serial.write(0x64);
  }

  // Change in input pin? Is High = 0x01; Is Low = 0x00; Serial.write Notification to Mayhem----------
  if((pin7StateOld != pin7State))
  {
    if(pin7State == 0x01) Serial.write(0x55);
    if(pin7State == 0x00) Serial.write(0x65);
  }
}

/*
 setDigitalOutputs is used to set the digital outputs
 based on the UART RX received data from Mayhem. The 
 available functionality includes toggle, turn high,
 and turn low for the defined output pins.
*/
void setDigitalOutputs(int byteReceived)
{  
  //Toggle pins (0+index in hex)--------------------------------
  if (byteReceived == 0x00)      // pin8
  {
    int pin8Read = digitalRead(pin8);
    if(pin8Read == LOW) digitalWrite(pin8, HIGH);
    if(pin8Read == HIGH) digitalWrite(pin8, LOW);
  }

  if (byteReceived == 0x01)     // pin9
  {
    int pin9Read = digitalRead(pin9);
    if(pin9Read == LOW) digitalWrite(pin9, HIGH);
    if(pin9Read == HIGH) digitalWrite(pin9, LOW);
  }

  if (byteReceived == 0x02)     // pin10 
  {
    int pin10Read = digitalRead(pin10);
    if(pin10Read == LOW) digitalWrite(pin10, HIGH);
    if(pin10Read == HIGH) digitalWrite(pin10, LOW);
  }

  if (byteReceived == 0x03)     // pin11
  {
    int pin11Read = digitalRead(pin11);
    if(pin11Read == LOW) digitalWrite(pin11, HIGH);
    if(pin11Read == HIGH) digitalWrite(pin11, LOW);
  }

  if (byteReceived == 0x04)     // pin12
  {
    int pin12Read = digitalRead(pin12);
    if(pin12Read == LOW) digitalWrite(pin12, HIGH);
    if(pin12Read == HIGH) digitalWrite(pin12, LOW);
  }

  if (byteReceived == 0x05)     // pin13
  {    
    int pin13Read = digitalRead(pin13);
    if(pin13Read == LOW) digitalWrite(pin13, HIGH);
    if(pin13Read == HIGH) digitalWrite(pin13, LOW);
  } 

  //Turn pins on (1+index in hex)-------------------------------
  if (byteReceived == 0x10) digitalWrite(pin8, HIGH);   // pin8
  if (byteReceived == 0x11) digitalWrite(pin9, HIGH);   // pin9
  if (byteReceived == 0x12) digitalWrite(pin10, HIGH);  // pin10
  if (byteReceived == 0x13) digitalWrite(pin11, HIGH);  // pin11
  if (byteReceived == 0x14) digitalWrite(pin12, HIGH);  // pin12
  if (byteReceived == 0x15) digitalWrite(pin13, HIGH);  // pin13

  //Turn pins off (2+index in hex)------------------------------
  if (byteReceived == 0x20) digitalWrite(pin8, LOW);    // pin8
  if (byteReceived == 0x21) digitalWrite(pin9, LOW);    // pin9
  if (byteReceived == 0x22) digitalWrite(pin10, LOW);   // pin10
  if (byteReceived == 0x23) digitalWrite(pin11, LOW);   // pin11
  if (byteReceived == 0x24) digitalWrite(pin12, LOW);   // pin12
  if (byteReceived == 0x25) digitalWrite(pin13, LOW);   // pin13
}

/*
 SerialEvent occurs whenever a new data comes in the
 hardware serial RX.  This routine is run between each
 time loop() runs, so using delay inside loop can delay
 response.  Multiple bytes of data may be available.
*/
void serialEvent()
{ 
  int rxBuffer = 0;
  
  // Check if data available from serial port
  if (Serial.available() > 0) 
  {
    // Read the incoming byte
    rxBuffer = Serial.read();
  } 
  
  setDigitalOutputs(rxBuffer);
}

