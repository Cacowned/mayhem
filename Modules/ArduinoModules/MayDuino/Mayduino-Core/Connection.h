#ifndef CONNECTION_H
#define CONNECTION_H
#include "WProgram.h"  // Arduino Macros
#include "Pin.h"
#include "Config.h"    // conection config


#define MAX_CONNECTIONS 30


class Connection
{
  
  private:
  ////////////////
  EPIN eventPin;           // id of event pin
  TPIN triggerPin;         // id of trigger pin
  Config config;            // connection configuration 
  boolean enabled; 
  
  //////////////
  // static fields
  static byte numConnections;      // no more than 256, only about 80 will fit in the UNO EEPROM anyways
  static Connection connections[ MAX_CONNECTIONS ];  
  
  // private, instance-level connect
  void ConnectPins(EPIN ePin, TPIN tPin, Config conf);
  void Trigger();
  void PrintInfo();

  public:
      
    Connection();
    
    // inline this one
    void GetEEPROMImage(ConnectionEEPROMImage& image)
    {
      image.fields.ePin  = eventPin;
      image.fields.eType = config.eventType;
      image.fields.eCond = config.eventCondition;
      image.fields.eArgs = config.eventArgs;
      
      image.fields.tPin  = triggerPin;
      image.fields.tType = config.reactionType;
      image.fields.tCond = config.reactionCondition;
      image.fields.tArgs = config.reactionArgs;
      
    }
    
    // resets all connections
    static void Reset();               
    // connect two pins
    static boolean Connect(EPIN ePin, TPIN tPin, Config conf); 
    // test connection status
    static void Test(byte pinID);
    static void WriteAlltoEEPROM();
    static void ReadFromEEPROM();
};


#endif
