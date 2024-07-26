#include <Arduino.h>

// Pin number for the SSR
const int ledPin = 2;

// Method to turn the LED on (enabling the SSR)
void turnLedOn()
{
  digitalWrite(ledPin, LOW);
}

// Method to turn the LED off (disabling the SSR)
void turnLedOff()
{
  digitalWrite(ledPin, HIGH);
}

// Method to restart (power off, wait for 10 seconds, and power on)
void restart()
{
  Serial.println("Restarting...");
  delay(1000);
  turnLedOff();
  delay(10000);
  turnLedOn();
}

void setup()
{
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
    else if (command == "RESTART")
    {
      restart();
    }
    else
    {
      // If an unknown command is received, notify the user
      Serial.println("Unknown command");
    }
  }
}