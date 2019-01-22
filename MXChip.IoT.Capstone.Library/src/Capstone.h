/*
    Capstone.h - Library for generating unique identifiers for Azure IoT MPP Capstone solutions
*/
#ifndef Capstone_h
#define Capstone_h

class Capstone
{
    public:
        Capstone(const char* studentId);
        

        // assumes input is a zero terminated string
        void GenerateUid(char* input, unsigned int inputLength, char* output, unsigned int outputMaxLength);
    private:
        const char* _studentId;
};


#endif // Capstone_h