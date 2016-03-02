- GenerateStreetNames: this project is to generate street address by xml format road names. Put the file to read in /ReadFile and configure file name in `main.cpp`. for more details, refer to `main.cpp`. Generated data will be put in /Common folder

- ValidStreetNmes: this project is to valid generated street address via USPS API(I still don't know the limit, maybe there's no limit). provide the input file in /bin/release after you compile the project. input file should be named as "input_test.txt" which you can update in Program.cs. Result file will be names as "FinalResult.txt" with validated address and zip code.

- ImplementOrder: this project is to generate orders by validated address. Result will be put in /Common .

- CollectedData: there's city street names I dowanloaded form web.

- Common: common data between projects.

- CityGraph: Calculate distance between each cities.