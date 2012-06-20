/*
 * Mayhem firmware for the MSP430 TI Launchpad
 * with the MSP430G2553 chip.
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
 * Version 1.0.0
 */

// Includes
#include <msp430.h>
#include <time.h>

// Function definition
void configClocks(void);
void configPins(void);
void configUart(void);
char pollInputs(void);
void TransmitString(char *string);
void Transmit(int);
void setDigitalOutputs(char);
void readDigitalInputs(void);
void delay(unsigned int);

// Variable definition
unsigned int txByte;
unsigned char inputPin1;			// P1.3
unsigned char inputPin1State;
unsigned char inputPin1StateOld;
unsigned char inputPin2;			// P2.1
unsigned char inputPin2State;
unsigned char inputPin2StateOld;
unsigned char inputPin3;			// P2.2
unsigned char inputPin3State;
unsigned char inputPin3StateOld;
unsigned char inputPin4;			// P2.3
unsigned char inputPin4State;
unsigned char inputPin4StateOld;
unsigned char inputPin5;			// P2.4
unsigned char inputPin5State;
unsigned char inputPin5StateOld;
unsigned char inputPin6;			// P2.5
unsigned char inputPin6State;
unsigned char inputPin6StateOld;

// Main function
void main(void)
{
	// Configuration and initialization
	configClocks();
	configPins();
	configUart();

	// Enable interrupts
	__enable_interrupt();

	// Clear input state variables
	inputPin1State = inputPin2State = inputPin3State = inputPin4State = inputPin5State = inputPin6State = 0x00;
	inputPin1StateOld = inputPin2StateOld = inputPin3StateOld = inputPin4StateOld = inputPin5StateOld = inputPin6StateOld = 0x00;

	while (1)
	{
		// Initialize and save the input pin states
		inputPin1StateOld = inputPin1State;
		inputPin2StateOld = inputPin2State;
		inputPin3StateOld = inputPin3State;
		inputPin4StateOld = inputPin4State;
		inputPin5StateOld = inputPin5State;
		inputPin6StateOld = inputPin6State;

		// Poll the input pins; Goes High = 0x01; Goes Low = 0x00
		if ((BIT3 & P1IN)) inputPin1State = 0x01; else inputPin1State = 0x00;
		if ((BIT1 & P2IN)) inputPin2State = 0x01; else inputPin2State = 0x00;
		if ((BIT2 & P2IN)) inputPin3State = 0x01; else inputPin3State = 0x00;
		if ((BIT3 & P2IN)) inputPin4State = 0x01; else inputPin4State = 0x00;
		if ((BIT4 & P2IN)) inputPin5State = 0x01; else inputPin5State = 0x00;
		if ((BIT5 & P2IN)) inputPin6State = 0x01; else inputPin6State = 0x00;

		// Change in input pin? Is High = 0x01; Is Low = 0x00; Transmit Notification to Mayhem
		if((inputPin1StateOld != inputPin1State))
		{
			if(inputPin1State == 0x01) Transmit(0x50);
			if(inputPin1State == 0x00) Transmit(0x60);
		}

		// Change in input pin? Is High = 0x01; Is Low = 0x00; Transmit Notification to Mayhem
		if((inputPin2StateOld != inputPin2State))
		{
			if(inputPin2State == 0x01) Transmit(0x51);
			if(inputPin2State == 0x00) Transmit(0x61);
		}

		// Change in input pin? Is High = 0x01; Is Low = 0x00; Transmit Notification to Mayhem
		if((inputPin3StateOld != inputPin3State))
		{
			if(inputPin3State == 0x01) Transmit(0x52);
			if(inputPin3State == 0x00) Transmit(0x62);
		}

		// Change in input pin? Is High = 0x01; Is Low = 0x00; Transmit Notification to Mayhem
		if((inputPin4StateOld != inputPin4State))
		{
			if(inputPin4State == 0x01) Transmit(0x53);
			if(inputPin4State == 0x00) Transmit(0x63);
		}

		// Change in input pin? Is High = 0x01; Is Low = 0x00; Transmit Notification to Mayhem
		if((inputPin5StateOld != inputPin5State))
		{
			if(inputPin5State == 0x01) Transmit(0x54);
			if(inputPin5State == 0x00) Transmit(0x64);
		}

		// Change in input pin? Is High = 0x01; Is Low = 0x00; Transmit Notification to Mayhem
		if((inputPin6StateOld != inputPin6State))
		{
			if(inputPin6State == 0x01) Transmit(0x55);
			if(inputPin6State == 0x00) Transmit(0x65);
		}
	}
}

// Clock configuration
void configClocks(void)
{
	// Stop watchdog timer
	WDTCTL = WDTPW + WDTHOLD;

	// Set DCO to run at 1MHz
	DCOCTL = CALDCO_1MHZ;

	// Set basic clock registers to 1MHz
	BCSCTL1 = CALBC1_1MHZ;
	BCSCTL2 = SELM_0 + DIVM_0;
}

// Pin configuration
void configPins(void)
{
	// &= BITWISE AND assignment
	// |= BITWISE OR assignment
	// ^= BITWISE XOR assignment
	// ~  BITWISE NOT

	// Set port directions for Port 1
	// P1.0 = BIT0 = output
	// P1.1 = BIT1 = UART TX (set by USCI)
	// P1.2 = BIT2 = UART RX (set by USCI)
	// P1.3 = BIT3 = input
	// P1.4 = BIT4 = output
	// P1.5 = BIT5 = output
	// P1.6 = BIT6 = output
	// P1.7 = BIT7 = output
	P1DIR |= BIT0+BIT4+BIT5+BIT6+BIT7;
	P1DIR &= ~BIT3;

	// Set port directions for Port 2
	// P2.0 = BIT0 = output
	// P2.1 = BIT1 = input
	// P2.2 = BIT2 = input
	// P2.3 = BIT3 = input
	// P2.4 = BIT4 = input
	// P2.5 = BIT5 = input
	P2DIR |= BIT0;
	P2DIR &= ~(BIT1+BIT2+BIT3+BIT4+BIT5);

	// Enable internal resistor for inputs and set to pull-down
	P1REN |= BIT3;
	P1OUT &= ~BIT3;
	P2REN |= BIT1+BIT2+BIT3+BIT4+BIT5;
	P2OUT &= ~(BIT1+BIT2+BIT3+BIT4+BIT5);
}

// Configuration for the UART
void configUart(void)
{
	// Port select for TX/RX pins on P1.1 and P1.2
	P1SEL = BIT1 + BIT2;
	P1SEL2 = BIT1 + BIT2;

	// Configure the UART at 9600baud
	UCA0CTL1 = UCSWRST;  	//reset
	UCA0CTL0 = 0;		 	//set UART mode
	UCA0CTL1 |= UCSSEL_2;	//set SMCLK
	UCA0MCTL = UCBRS0;		//set modulation
	UCA0BR0 = 104;   		//set baud rate register 0
	UCA0BR1 = 0;     		//set baud rate register 1
	UCA0CTL1 &= ~UCSWRST;	//reset

	// Enable UART RX interrupt
	IE2 = UCA0RXIE;
}

// Transmit a string on the serial port
void TransmitString(char *string)
{
	while(*string != 0)
	{
		txByte = *string;
		Transmit(txByte);
		string++;
	}
}

// Transmit a byte on the serial port
void Transmit(int transmitByte)
{
	UCA0TXBUF = transmitByte;
}

// Delay in milliseconds
void delay(unsigned int ms)
{
	unsigned int j;

	for (j=ms; j>0; j--)
	{
		__delay_cycles(1000);
	}
}

// Set digital outputs based on the byte received on the serial port
void setDigitalOutputs(char byteReceived)
{
	//Toggle pins (0+index in hex)
	if (byteReceived == 0x00) P1OUT ^= BIT0; 						// P1.0
	if (byteReceived == 0x01) P1OUT ^= BIT4; 						// P1.4
	if (byteReceived == 0x02) P1OUT ^= BIT5; 						// P1.5
	if (byteReceived == 0x03) P1OUT ^= BIT6; 						// P1.6
	if (byteReceived == 0x04) P1OUT ^= BIT7; 						// P1.7
	if (byteReceived == 0x05) P2OUT ^= BIT0; 						// P2.0

	//Turn pins on (1+index in hex)
	if (byteReceived == 0x10) P1OUT |= BIT0; 						// P1.0
	if (byteReceived == 0x11) P1OUT |= BIT4; 						// P1.4
	if (byteReceived == 0x12) P1OUT |= BIT5; 						// P1.5
	if (byteReceived == 0x13) P1OUT |= BIT6; 						// P1.6
	if (byteReceived == 0x14) P1OUT |= BIT7; 						// P1.7
	if (byteReceived == 0x15) P2OUT |= BIT0; 						// P2.0

	//Turn pins off (2+index in hex)
	if (byteReceived == 0x20) P1OUT &= ~BIT0; 						// P1.0
	if (byteReceived == 0x21) P1OUT &= ~BIT4; 						// P1.4
	if (byteReceived == 0x22) P1OUT &= ~BIT5; 						// P1.5
	if (byteReceived == 0x23) P1OUT &= ~BIT6; 						// P1.6
	if (byteReceived == 0x24) P1OUT &= ~BIT7; 						// P1.7
	if (byteReceived == 0x25) P2OUT &= ~BIT0; 						// P2.0
}

// Interrupt vector for the UART receive functionality
#pragma vector=USCIAB0RX_VECTOR
__interrupt void USCIAB0RX_ISR()
{
	unsigned char rxBuffer;

	if(IFG2 & UCA0RXIFG)
	{
	    rxBuffer = UCA0RXBUF;                  // Store RX'ed UART data
	}

	if (rxBuffer == 0x76)
	{
		Transmit(0x79);
	}

	setDigitalOutputs(rxBuffer);
}
