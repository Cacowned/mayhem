#ifndef PIN_H
#define PIN_H

#include "WProgram.h"  // Arduino Macros
#include "Servo.h"

// Event Types
typedef enum Event_t {
 EVENT_DISABLED,
 DIGITALEVENT,
 ANALOGEVENT
} EventType; 

// reaction types
typedef enum Reaction_t
{
 REACTION_DISABLED,
 DIGITALREACTION,
 ANALOGREACTION,
 SERVOREACTION
} ReactionType ;


#define EVENTARGS_NONE 0

typedef byte EPIN;
typedef byte TPIN;


// analog event tye
enum
{
 SMALLER,
 GREATER 
};

// 6 Servos
enum
{
   SERVO_A,
   SERVO_B,
   SERVO_C,
   SERVO_D,
   SERVO_E,
   SERVO_F 
} ;



// EPIN eventPin;
// TPIN reactionPin;
// EventType eventType;
// ReactionType reactionType;
// int eventArgs, reactionArgs;
// byte eventCond, reactionCond;

 typedef struct
  {
    // event
    byte ePin;
    int eType;
    byte eCond;
    int eArgs;
    
    // reaction
    byte tPin;
    int tType;
    byte tCond;
    int tArgs; 
    
  } EEPROMImgFields; 

// union of the connection info to write to the EEPROM
typedef union
{
  EEPROMImgFields fields; 
  // bytes for writing to the EEPROM
  byte bytes [sizeof(EEPROMImgFields)];   
} ConnectionEEPROMImage;




#endif

