
#include "MayDuino.h"
#include <Servo.h>

void PrintAck()
{
 Serial.println("OK"); 
}

void setup() {
  // put your setup code here, to run once:
  pinMode(13, OUTPUT);
  Serial.begin(57600);
  Serial.println("========== MayhemDuino ========");
//  Serial.println(sizeof(ConnectionEEPROMImage), DEC);
  //Config config_on = Config(DIGITALEVENT, HIGH, DIGITALREACTION,HIGH,0,0);
 // Config config_off = Config(DIGITALEVENT, LOW, DIGITALREACTION,LOW,0,0);
 /*
 
   Config analog_on =  Config(ANALOGEVENT, GREATER, 150, ANALOGREACTION, HIGH, 512);
   Config analog_off = Config(ANALOGEVENT, SMALLER, 150, ANALOGREACTION, LOW, 128); 
   
   Config analog_on_pwm =  Config(ANALOGEVENT, SMALLER, 150, ANALOGREACTION, HIGH, 512);
   Config analog_off_pwm = Config(ANALOGEVENT, GREATER, 150, ANALOGREACTION, LOW, 128); 
   
   Config servo_on  =  Config(ANALOGEVENT, SMALLER, 150, SERVOREACTION, LOW, 180);
   Config servo_off = Config(ANALOGEVENT, GREATER, 150, SERVOREACTION, LOW,0); 

   Connection::Connect(0,5, analog_on);
   Connection::Connect(0,5, analog_off);
  
   Connection::Connect(0,6, analog_on_pwm);
   Connection::Connect(0,6, analog_off_pwm);
   
   Connection::Connect(0,7, servo_on);
   Connection::Connect(0,7, servo_off);
   */
   //Connection::Reset();
   Connection::ReadFromEEPROM();
   
}

void loop() {
 
  while (true)
  {
    // put your main code here, to run repeatedly: 
    
   
     byte bytes = Serial.available();
     if (bytes>0)
     {
       if (Serial.read() == '$')
       {
         char cmd = Serial.read();
         switch (cmd)
         {
          case 'c':                  // new connection
              {   
                if (Serial.read() == ',')          
                parseNewConnectionCommand(bytes-3);
              }
             break;
          case 'r':                 // reset connection     
             Connection::Reset();
             PrintAck();
             break; 
          case 'a':                 // activate connections          
             PrintAck();
             break;  
          case 'w':                // save connections to EEPROM
             Connection::WriteAlltoEEPROM();
             PrintAck();
             break;     
          case 'l':
             Connection::ReadFromEEPROM();
             PrintAck();
             break;
         }
       }
     }
    
    
    for (int i = 0; i < MAX_CONNECTIONS; i++)
    {
      //Serial.println(i, DEC); 
      Connection::Test(i);  
    }
    delay(40);
  }
}

inline void parseNewConnectionCommand(byte bytes)
{
  // order of cammand is
  // evPin, EvType, evCond, evArgs, rePin, reaType, reaCond, reaArgs
  
  EPIN eventPin;
  TPIN reactionPin;
  EventType eventType;
  ReactionType reactionType;
  int eventArgs, reactionArgs;
  byte eventCond, reactionCond;
  
 
  // buffer to hold the command string
  //char * buffer = (char *) malloc(sizeof(char) * bytes) ; 
  char buffer[bytes];
  
  for (int i = 0; i < bytes; i++)
  {
    buffer[i] = Serial.read();
  }
  // clear the buffer (extra read) 

  
  //Serial.print("Parse Connection command: ");
  //Serial.println(buffer);
  
  char * pch;
  pch = strtok((char *) buffer, ",");
  byte tokens = 0; 
  
  //////////////////////// event
  // event pin
  //pch = strtok(NULL, ",");
  if (pch != NULL)
  {
     eventPin = atoi(pch); 
    // Serial.println(pch);
  }
  else
  {
   return; 
  }
  
  // event type
  pch = strtok(NULL, ",");
  if (pch != NULL)
  {
     eventType = (EventType) atoi(pch); 
    //  Serial.println(pch);
  }
  else
  {
    return;
  }
  
  // event condition
  pch = strtok(NULL, ",");
  if (pch != NULL)
  {
     eventCond = (EventType) atoi(pch); 
     // Serial.println(pch);
  }
  else
  {
    return;
  }
  
  // event args
  pch = strtok(NULL, ",");
  if (pch != NULL)
  {
     eventArgs = atoi(pch); 
     // Serial.println(pch);
  }
  else
  {
    return;
  }
  
  /////////////////////////// reaction
  // reaction pin
  pch = strtok(NULL, ",");
  if (pch != NULL)
  {
     reactionPin = atoi(pch); 
     // Serial.println(pch);
  }
  else
  {
   return; 
  }
  
  // reaction type
  pch = strtok(NULL, ",");
  if (pch != NULL)
  {
     reactionType = (ReactionType) atoi(pch); 
     // Serial.println(pch);
  }
  else
  {
    return;
  }

  // reaction condition
  pch = strtok(NULL, ",");
  if (pch != NULL)
  {
     reactionCond = (EventType) atoi(pch); 
     // Serial.println(pch);
  }
  else
  {
    return;
  }
  
  // reaction args
  pch = strtok(NULL, ",");
  if (pch != NULL)
  {
     reactionArgs = atoi(pch); 
     // Serial.println(pch);
  }
  else
  {
    return;
  }
  
  ///////// build the event
  /*
  Serial.println("Got valid config string");
  Serial.print("EventPin "); Serial.print(eventPin, DEC);
  Serial.print(" EventType "); Serial.print(eventType, DEC);
  Serial.print(" EventCond "); Serial.print(eventCond, DEC);
  Serial.print(" EventArgs "); Serial.print(eventArgs, DEC);
  Serial.print(" ReactPin "); Serial.print(reactionPin, DEC);
  Serial.print(" ReactType "); Serial.print(reactionType, DEC);
  Serial.print(" ReactCond "); Serial.print(reactionCond, DEC);
  Serial.print(" ReactArgs "); Serial.println(reactionArgs, DEC);*/
  
  // create a new connection
  Config config(eventType, eventCond, eventArgs, reactionType, reactionCond, reactionArgs);
  Connection::Connect(eventPin, reactionPin, config);
  PrintAck();
}


