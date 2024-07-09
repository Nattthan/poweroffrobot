#include <Arduino.h>

// Pin number for the LED
const int ledPin = 2;
// Pin number for the restart button
const int buttonPin = 3;

// Method to turn the LED on
void turnLedOn()
{
  digitalWrite(ledPin, LOW);
}

// Method to turn the LED off
void turnLedOff()
{
  digitalWrite(ledPin, HIGH);
}

// Method to restart (power off, wait for 10 seconds, and power on)
void restart()
{
  turnLedOff();
  delay(10000);
  turnLedOn();
}

void externalRestart()
{
  Serial.println("EXTERNAL_HARD_REBOOT \n");
  delay(5000);
  restart();
}

void setup()
{
  // Initialize the digital pin as an input_pullup for the button.
  pinMode(buttonPin, INPUT_PULLUP);
  // Initialize the digital pin as an output.
  pinMode(ledPin, OUTPUT);
  // Initialize serial communication at 9600 bits per second
  Serial.begin(9600);

  // Turn on the LED at the start of the day
  turnLedOn();
  delay(1000);
}

void loop()
{
  // Check if data is available to read
  if (Serial.available() > 0)
  {
    // Read the incoming string
    String command = Serial.readStringUntil('\n');

    // Remove any trailing newline characters from the command
    command.trim();

    // Execute commands
    if (command == "o")
    {
      turnLedOn();
    }
    else if (command == "f")
    {
      turnLedOff();
    }
    else if (command == "r")
    {
      restart();
    }
    else if (command == "RESTART")
    {
      externalRestart();
    }
    else
    {
      // If an unknown command is received, notify the user
      Serial.println("Unknown command");
    }
  }
}
