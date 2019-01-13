#include <Capstone.h>
#include <HexConversionUtils.h>
#include <MxChipUtility.h>
#include <SipHash_2_4.h>

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

void loop() {
  // put your main code here, to run repeatedly:
  if (hasIoTHub && hasWifi)
  {
    char buff[128];
    byte fred;
    MxChipUtility chipUtility;

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

    if(hasIoTHub && chipUtility.IsButtonClicked(USER_BUTTON_A))
    {
      Screen.print(2, "Rotor Alert");
      delay(50);
    }
    else if(hasIoTHub && chipUtility.IsButtonClicked(USER_BUTTON_B))
    {
      Screen.print(2, "Gen Alert");
      delay(50);
    }
    else
    {
      Screen.print(2, "Normal");
      delay(50);    
    }


    char hashBuff[128];
    char message[128] = "Let's has this string";
    int result;
    Capstone capstone("123456");

    capstone.GenerateUid(message, 22, hashBuff, 128);
    Screen.print(3, hashBuff);

    delay(1000);
  }
}
