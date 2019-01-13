#include "Capstone.h"
#include "HexConversionUtils.h"
#include "SipHash_2_4.h"
#include "AZ3166WiFi.h"
#include "DevKitMQTTClient.h"
#include "Sensor.h"
#include "SystemVersion.h"
#include "http_client.h"
#include "telemetry.h"

Capstone::Capstone(char* studentId)
{
    _studentId = studentId;
}

void Capstone::GenerateUid(char* input, unsigned int inputLength, char* output, unsigned int outputMaxLength)
{
      // Define your 'secret' 16 byte key in RAM
    uint8_t key[] = {0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
                        0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f};

    char prompt[] = "Another message to hash";
    int msgLen = sizeof prompt;

    // to start hashing initialize with your key
    sipHash.initFromRAM(key);
    // for each byte in the message call updateHash()
    for (int i=0; i<msgLen;i++) 
    {
    sipHash.updateHash((byte)prompt[i]); // update hash with each byte of msg
    }
    // at the end of the message call finalize to calculate the result
    sipHash.finish(); // finish
    // the uint8_t[8] variable, sipHash.result, then contains the 8 bytes of the hash in BigEndian format

    //snprintf(output, outputMaxLength, "Result!");

    hexToAscii(sipHash.result, 8, output, outputMaxLength);
}