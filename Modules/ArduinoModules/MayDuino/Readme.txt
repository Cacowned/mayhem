Mayduino is an experimental set of modules that allows events and reactions related to the Arduino to be hard coded in the Arduino.

This allows the Arduino to execute events and reaction in a standalone mode, not needing the PC app to be running. In effect, users can use Mayhem to program simple applications for the Arduino which will run on the controller board in a standalone basis.

However, there is no interconnectivity between Mayduino events and reactions and the Mayhem application's events and reactions. This issue will have to be addressed in the future. 

The directory Mayduino Core contains the source code for the Arduino sketch that must be running on the Arduino for the Mayduino events and reactions to work.

At this time only the Arduino UNO and Diecimilla (Atmega 16x, full size) are supported. 
