#include "AZ3166WiFi.h"
#include "DevKitMQTTClient.h"
#include "Sensor.h"
#include "SystemVersion.h"
#include "http_client.h"
#include "telemetry.h"

static bool hasWifi = false;
static bool hasIoTHub = false;

void setup() {
  // put your setup code here, to run once:
  if (WiFi.begin() == WL_CONNECTED)
  {
    hasWifi = true;
    Screen.print(1, "Running...");

    if (!DevKitMQTTClient_Init())
    {
      hasIoTHub = false;
      return;
    }
    hasIoTHub = true;
  }
  else
  {
    hasWifi = false;
    Screen.print(1, "No Wi-Fi");
  }
}

bool IsButtonClicked(unsigned char ulPin)
{
    pinMode(ulPin, INPUT);
    int buttonState = digitalRead(ulPin);
    if(buttonState == LOW)
    {
        return true;
    }
    return false;
}

void loop() {
  // put your main code here, to run repeatedly:
  if (hasIoTHub && hasWifi)
  {
    char buff[128];

    // replace the following line with your data sent to Azure IoTHub
    snprintf(buff, 128, "{\"topic\":\"iot\"}");
    
    if (DevKitMQTTClient_SendEvent(buff))
    {
      Screen.print(1, "Sending...");
    }
    else
    {
      Screen.print(1, "Failure...");
    }

    if(hasIoTHub && IsButtonClicked(USER_BUTTON_A))
    {
      Screen.print(2, "Rotor Alert");
      delay(50);
    }
    else if(hasIoTHub && IsButtonClicked(USER_BUTTON_B))
    {
      Screen.print(2, "Gen Alert");
      delay(50);
    }
    else
    {
      Screen.print(2, "Normal");
      delay(50);    
    }
    delay(1000);
  }
}
