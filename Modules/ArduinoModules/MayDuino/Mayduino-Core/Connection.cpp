#include "Connection.h"
#include "Servo.h"
#include "EEPROM.h"



// initialize static fields
byte Connection::numConnections = 0;
Connection Connection::connections[ MAX_CONNECTIONS ]; 

Servo servo[8];
boolean servoConnected = false;

// servo pin definitions
byte gServoPins[8] = {3,5,6,9,10,11,12,13};
//

// default constructor
Connection::Connection(): 
  eventPin(0xff), 
  triggerPin(0xff),
  config(Config()),
  enabled(false)
{
  numConnections = 0;
}



// Triggers a connection according to the reaction type
void Connection::Trigger()
{
  ReactionType rType = config.reactionType; //GetReactionType(config.connectionConfig);
  //Serial.print("ReactionType : "); Serial.println(rType,DEC);
  switch (rType)
  {
    case DIGITALREACTION : 
          {
                byte output = config.reactionCondition;
                digitalWrite(triggerPin, output );      
          }
          break;
    case ANALOGREACTION : 
          {
              int value = config.reactionArgs;
              analogWrite(triggerPin, value);
          }   
          break;
          
    case SERVOREACTION :   
          {
              int servoValue = config.reactionArgs;
             // Serial.print("Servo value: ");Serial.print(servoValue, DEC);
             // Serial.print(" servo Pin "); Serial.println(gServoPins[triggerPin], DEC);
             servo[triggerPin].write(servoValue);
          }    
          break;
  } 
}

void Connection::ConnectPins(EPIN ePin, TPIN tPin, Config conf)
{
  eventPin = ePin;
  triggerPin = tPin;
  config = conf;
  
  if (config.eventType == DIGITALEVENT)
  {
     digitalWrite(eventPin, config.eventArgs);
     pinMode(eventPin, INPUT);
  }
  else if (config.eventType == ANALOGEVENT)
  {
    // 
  }
  
  
  // reaction
  if (config.reactionType == SERVOREACTION)
  {
    if (!servo[triggerPin].attached())
    {
      //Serial.print("Attaching Servo on Pin: "); Serial.println(gServoPins[triggerPin],DEC);
      servo[triggerPin].attach(gServoPins[triggerPin]);
      servoConnected = true;
    }
  }
  else if (config.reactionType == DIGITALREACTION)
  {
     pinMode(triggerPin, OUTPUT); 
  }
  enabled=true;
}

// assign a connection to the first free place (static)
boolean Connection::Connect(EPIN ePin, TPIN tPin, Config conf)
{
  if (Connection::numConnections < MAX_CONNECTIONS)
  {
    // don't connect disabled events or connections
     if (conf.eventType != EVENT_DISABLED && conf.reactionType != REACTION_DISABLED)
     {
       Connection::connections[Connection::numConnections++].ConnectPins(ePin, tPin, conf);
       return true;
     }
     else
     {
       return false; 
     }
  }
  else
  {
   return false; 
  }
  
}

// test if reaction should be fired
void Connection::Test(byte connId)
{
  if (connId <= MAX_CONNECTIONS)
  {
    Connection * c = &(connections[connId]);
    if (c->enabled)
    {
     // Serial.print((int)connId,DEC); Serial.print(" is Active! ");
      EventType eType = c->config.eventType;
     // Serial.print("Event Type: ");Serial.print(eType,DEC);Serial.print(" epin "); Serial.print(c->eventPin,DEC); Serial.print(" tpin "); Serial.println(c->triggerPin, DEC);
      
      if (eType == DIGITALEVENT)
       {
          boolean pinstate = digitalRead(c->eventPin); 
          boolean monitorstate = c->config.eventCondition;           
          boolean output =  c->config.reactionCondition;  
          //Serial.print(" ps ");Serial.print(pinstate, DEC); Serial.print(" ms "); Serial.print(monitorstate, DEC);  Serial.print(" os "); Serial.println(output,DEC);
          
          if (pinstate  ==  monitorstate) 
          {
           // do action 
           //Serial.println("Trigger"); 
           //digitalWrite(c->triggerPin, output);
           c->Trigger();
          }
       } 
       else if (eType == ANALOGEVENT)
       {
         // process events for analog pin 
         int pinValue = analogRead(c->eventPin); 
         boolean triggerMode =c->config.eventCondition;  
         
       //  Serial.print(" ps "); Serial.print(pinValue, DEC); Serial.print(" ms "); Serial.println(triggerMode, DEC);  //Serial.print(" os "); Serial.println(output,DEC);
         if (triggerMode == GREATER && pinValue > c->config.eventArgs)
         {
             // do rection
       //      Serial.print("Trigger (rection to analog event), triggerPin"); Serial.println(c->triggerPin, DEC);
             c->Trigger();
             
         }
         else if (triggerMode == SMALLER && pinValue < c->config.eventArgs)
         {
            // do  
       //     Serial.println("Trigger (rection to analog event)"); 
            c->Trigger();
            //digitalWrite(c->triggerPin, HIGH);
         }
       }
    }
  }
}

void Connection::Reset()
{
 for (int i = 0; i < MAX_CONNECTIONS; i++)
  {
    connections[i] = Connection(); 
  }   
  numConnections = 0; 
}


void Connection::WriteAlltoEEPROM()
{
    int  address = 0; 
    const byte write_size = sizeof(ConnectionEEPROMImage);
    
    EEPROM.write(address++, numConnections);
    
   // Serial.print("Writing...Active Connections: "); Serial.println(numConnections,DEC);
    
    for (int i =0; i < MAX_CONNECTIONS; i++)
    {
       digitalWrite(13,HIGH);
       // for now just write out the whole stack of connections
      Connection * c = &(connections[i]); 
      ConnectionEEPROMImage cImage;
      c->GetEEPROMImage(cImage);
      // write out to the eeprom
     
      for (byte k = 0; k < write_size; k++)
      {
         EEPROM.write(address++,cImage.bytes[k]); 
      }
      digitalWrite(13,LOW);
    }
   // Serial.print("Wrote "); Serial.print(address, DEC); Serial.print( "bytes");
}

void Connection::ReadFromEEPROM()
{
   Connection::Reset();
   const byte read_size = sizeof(ConnectionEEPROMImage); 
   int address = 0;
   
   numConnections = EEPROM.read(address++);
  // Serial.print("EEPROM -- Number of configured connections: "); Serial.println(numConnections,DEC);
   
   for (int i = 0; i < MAX_CONNECTIONS; i++)
   {
     ConnectionEEPROMImage cImage;
     for (int k = 0; k < read_size; k++)
     {
        cImage.bytes[k] = EEPROM.read(address++);
     }
    
     Config conf = Config((EventType) cImage.fields.eType, 
                  cImage.fields.eCond, 
                  cImage.fields.eArgs, 
                  (ReactionType) cImage.fields.tType,
                  cImage.fields.tCond,
                  cImage.fields.tArgs);
                  
     if (Connection::Connect(cImage.fields.ePin, cImage.fields.tPin, conf));
     //  connections[numConnections-1].PrintInfo();
   }
}

void Connection::PrintInfo()
{
   Serial.println("======= Connection =====");
   Serial.print("TriggerPin: ");Serial.println(triggerPin,DEC);
   Serial.print("EventPin: ");Serial.println(eventPin,DEC);
   
   Serial.print("EventType: ");Serial.println(config.eventType,DEC);
   Serial.print("EventCondition: ");Serial.println(config.eventCondition,DEC);
   Serial.print("EventArgs: ");Serial.println(config.eventArgs,DEC);
   
   Serial.print("ReactionType: ");Serial.println(config.reactionType,DEC);
   Serial.print("ReactionCondition: ");Serial.println(config.reactionCondition,DEC);
   Serial.print("ReactionArgs: ");Serial.println(config.reactionArgs,DEC);
   
   Serial.print("Enabled: "); Serial.println(enabled, DEC);
   
}






