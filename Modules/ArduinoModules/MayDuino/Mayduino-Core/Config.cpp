#include "Config.h"


Config::Config() : 
  eventType(EVENT_DISABLED),
  eventCondition(0xff),
  eventArgs(0xffff),
  reactionType(REACTION_DISABLED),
  reactionCondition(0xff),
  reactionArgs(0xffff)
{}

Config::Config(EventType eType, byte eCondition, int eArgs, ReactionType rType, byte rCondition,  int rArgs) :
  eventType(eType),
  eventCondition(eCondition),
  eventArgs(eArgs),
  reactionType(rType),
  reactionCondition(rCondition),
  reactionArgs(rArgs)
{
}



