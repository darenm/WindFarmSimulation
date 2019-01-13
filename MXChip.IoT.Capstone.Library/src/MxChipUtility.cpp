#include "AZ3166WiFi.h"
#include "DevKitMQTTClient.h"
#include "Sensor.h"
#include "SystemVersion.h"
#include "http_client.h"
#include "telemetry.h"
#include "MxChipUtility.h"

MxChipUtility::MxChipUtility()
{}

bool MxChipUtility::IsButtonClicked(unsigned char ulPin)
{
    pinMode(ulPin, INPUT);
    int buttonState = digitalRead(ulPin);
    if(buttonState == LOW)
    {
        return true;
    }
    return false;
}