#include <Arduino.h>

// Pin number for the LED
const int ledPin = 13;

// Method to turn the LED on
void turnLedOn() {
  Serial.println("POWER ON");
  digitalWrite(ledPin, HIGH);
}

// Method to turn the LED off
void turnLedOff() {
  Serial.println("POWER OFF");
  digitalWrite(ledPin, LOW);
}

// Method to restart (power off, wait for 10 seconds, and power on)
void restart() {
  Serial.println("HARD REBOOT");
  // Turn the LED off
  turnLedOff();
  
  // Wait for 10 seconds
  Serial.println("wait 10s");
  delay(10000);
  
  // Turn the LED on
  turnLedOn();
}


void setup() {
  // Initialize the digital pin as an output.
  pinMode(ledPin, OUTPUT);
  
  // Initialize serial communication at 9600 bits per second
  Serial.begin(9600);

  // Turn on the LED at the start of the day
  turnLedOn();
  Serial.println("o will turn on power");
  Serial.println("f will turn off power");
  Serial.println("r will restart power with 10s delai");
}

void loop() {
  // Check if data is available to read
  if (Serial.available() > 0) {
    // Read the incoming string
    String command = Serial.readStringUntil('\n');

    // Echo the received command back to the serial monitor
    Serial.println("Received: " + command);

    // Remove any trailing newline characters from the command
    command.trim();

    // Execute commands
    if (command == "o") {
      turnLedOn();
    } else if (command == "f") {
      turnLedOff();
    } else if (command == "r") {
      restart();
    } else {
      // If an unknown command is received, notify the user
      Serial.println("Unknown command");
    }
  }
}

