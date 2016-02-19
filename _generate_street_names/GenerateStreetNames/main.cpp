#include <bits/stdc++.h>
#include "tinyxml2.h"

using namespace std;
using namespace tinyxml2;

ofstream myfile;

#define SetRandomRange 2500
#define StreePath "./ReadFile/SFOStreet.xml"
#define NamePath "./ReadFile/americanNames.txt"

vector<char *> typeList;
vector<string> nameList;

void GenerateNumberGenerate(int number, bool isSuffix = true){
    int lastNum = number % 10;

    myfile << number;

    if (isSuffix){
        if (lastNum == 1){
            myfile << "st";
        } else if (lastNum == 2) {
            myfile << "nd";
        } else if (lastNum == 3) {
            myfile << "rd";
        } else {
            myfile << "th";
        }
    }

    myfile << ", ";
}

void FetchNames(){
    string line, cell;
    ifstream myfile(NamePath);

    try
    {
        myfile.is_open();
            while (getline(myfile, line))
            {
                nameList.push_back(line);
            }
    }

    // display the exception thrown
    catch (ifstream::failure e) {
        cout << "Exception opening/reading file: " << e.what();
    }
}

void ShowNames(){
    vector<char> LastNameList;
    char ch = 'A';

    for (int i = 0; i < 26; ++i, ++ch){
        LastNameList.push_back(ch);
    }

    printf("%s %c., ", nameList[rand() % nameList.size()].c_str(), LastNameList[rand() % LastNameList.size()]);

}

void RandomNumberGenerate(){
    int number = rand() % SetRandomRange;
    int lastNum = number % 10;

    myfile << number;

    if (lastNum == 1){
        myfile << "st";
    } else if (lastNum == 2) {
        myfile << "nd";
    } else if (lastNum == 3) {
        myfile << "rd";
    } else {
        myfile << "th";
    }

    myfile << ", ";
}

void ConditionalPushVector(char* content){
    int i = 0;
    for (; i < typeList.size(); ++i){
        if (strcmp(typeList[i], content) == 0){
            break;
        }
    }

    if (i == typeList.size()){
        typeList.push_back(content);
    }
}

void ShowAllRoadType(){
    for (int i = 0; i < typeList.size(); ++i){
        cout << typeList[i] << endl;
    }
}

void GenerateSpecificSequence(const char* content, int startNum, int endNum){
    for (int i = startNum; i < endNum; ++i){
        GenerateNumberGenerate(i, false);
        myfile << content << endl;
    }
}

char* ExpandRoadType(char* roadType){
    if (strcmp(roadType, "ST") == 0){
        return " ST,";
    } else if (strcmp(roadType, "AVE") == 0){
        return " AVE,";
    } else if (strcmp(roadType, "ALY") == 0){
        return " ALY,";
    } else if (strcmp(roadType, "CT") == 0){
        return " CT,";
    } else if (strcmp(roadType, "WAY") == 0){
        return " WAY,";
    } else if (strcmp(roadType, "TER") == 0){
        return " TER,";
    } else if (strcmp(roadType, "BLVD") == 0){
        return " BLVD,";
    } else if (strcmp(roadType, "RAMP") == 0){
        return " RAMP,";
    } else if (strcmp(roadType, "PL") == 0){
        return " PL,";
    } else if (strcmp(roadType, "LN") == 0){
        return " LN,";
    } else if (strcmp(roadType, "LOOP") == 0){
        return " LOOP,";
    } else if (strcmp(roadType, "DR") == 0){
        return " DR,";
    } else if (strcmp(roadType, "RD") == 0){
        return " RD,";
    } else if (strcmp(roadType, "CIR") == 0){
        return " CIR,";
    } else if (strcmp(roadType, "WALK") == 0){
        return " WALK,";
    } else if (strcmp(roadType, "PARK") == 0){
        return " PARK,";
    } else if (strcmp(roadType, "ROW") == 0){
        return " ROW,";
    } else if (strcmp(roadType, "PATH") == 0){
        return " PATH,";
    } else if (strcmp(roadType, "HWY") == 0){
        return " HWY,";
    } else if (strcmp(roadType, "EXPY") == 0){
        return " EXPY,";
    } else if (strcmp(roadType, "HL") == 0){
        return " HL,";
    } else if (strcmp(roadType, "PLZ") == 0){
        return " PLZ,";
    } else if (strcmp(roadType, "STPS") == 0){
        return " STPS,";
    } else if (strcmp(roadType, "BLVD NORTH") == 0){
        return " BLVD NORTH,";
    } else if (strcmp(roadType, "BLVD SOUTH") == 0){
        return " BLVD SOUTH,";
    } else if (strcmp(roadType, "STWY") == 0){
        return " STWY,";
    } else if (strcmp(roadType, "TUNL") == 0){
        return " TUNL,";
    } else {
        return roadType; // not match
    }
}

void GenerateStreetName()
{
    myfile.open ("input_test.txt");

	XMLDocument doc;
	doc.LoadFile(StreePath);
	XMLElement *row = doc.RootElement();
	XMLElement *surface = row->FirstChildElement("row");

	for (; surface; surface = surface->NextSiblingElement())
	{
        XMLElement *surfaceChild = surface->FirstChildElement();
        const char* content;
        const XMLAttribute *attributeOfSurface = surface->FirstAttribute();
        // printf("%s:%s:   ", attributeOfSurface->Name(), attributeOfSurface->Value());

        // ShowNames();

        for (; surfaceChild; surfaceChild = surfaceChild->NextSiblingElement()){
            const char* conditionName = surfaceChild->Name();

            if (strcmp(conditionName, "streetname") == 0){ // streetname type select
                content = surfaceChild->GetText();
                // printf("%s", content);

                GenerateSpecificSequence(content, 1, 100);

                GenerateSpecificSequence(content, 100, 150);

                GenerateSpecificSequence(content, 200, 250);

                GenerateSpecificSequence(content, 1000, 1100);

                GenerateSpecificSequence(content, 2000, 2100);


            } else if (strcmp(conditionName, "streettype") == 0){ // streettype type select
                content = surfaceChild->GetText();

                if (content == NULL) continue;

                char * str = const_cast<char *>(content);
                ConditionalPushVector(str);

                // printf("%s", ExpandRoadType(str));
                // printf(" SF, CA");
            }
        }

        // puts("");
	}

    // ShowAllRoadType();
}

int main()
{
    // initial random seed
    srand(time(NULL));

    FetchNames();
    GenerateStreetName();

    return 0;
}





