/*
*    Connection Configuration
*
*
*/

#ifndef CONFIG_H
#define CONFIG_H
#include "WProgram.h"  // Arduino Macros
#include "Pin.h"



class Config
{
  public:
  
  EventType eventType;               // what kind of event?
  byte eventCondition;            // what is the event condition?
  int eventArgs;                  // extended configuration argument for triggers
  
  ReactionType reactionType;            // what kind of reaction? 
  byte reactionCondition;            // what is the reaction condition 
  int reactionArgs;                 // configuration argument for reaction
  
  Config();                        // 
  Config(EventType eType, byte eCondition, int eArgs, ReactionType rType, byte rCondition,  int rArgs);
  
};

#endif

