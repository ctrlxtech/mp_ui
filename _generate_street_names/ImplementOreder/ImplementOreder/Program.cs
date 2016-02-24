namespace ImplementOreder
{
    using System;
    using System.IO;
    using System.Linq;

    class Program
    {
        public const string OutputFileName = "finalOrders.txt";
        public const string InputFileName = "StreetsAfterValidation.txt";
        public const string PeopleNameFile = "AmericanNames.txt";

        static void Main(string[] args)
        {
            // create new file
            File.WriteAllText(OutputFileName, "");

            var streetNames = File.ReadAllLines(InputFileName);

            foreach (var streetName in streetNames)
            {
                using (var streamWriter = File.AppendText(OutputFileName))
                {
                    var randomPeopleName = GenerateRandomFullName();
                    var randomMassageInfo = GenerateMassageInfo();
                    var randomAppiontmentTime = GenerateAppointmentTime();

                    streamWriter.WriteLine(randomPeopleName + streetName + randomMassageInfo + randomAppiontmentTime);
                }
            }
        }

        public static string GenerateRandomFullName()
        {
            var firstName = GenerateRandomFirstName();

            var lastName = RandomString();

            return firstName + ' ' + lastName + "., ";
        }

        public static string RandomString()
        {
            const string chars = "ABCDFGHJKLMNPRSTVWYZ";
            return new string(Enumerable.Repeat(chars, 1)
                .Select(s => s[GetRandomNumber(s.Length)]).ToArray());
        }

        private static readonly Random Getrandom = new Random();
        private static readonly object SyncLock = new object();

        public static int GetRandomNumber(int min, int max)
        {
            lock (SyncLock)
            { // synchronize
                return Getrandom.Next(min, max);
            }
        }
        public static int GetRandomNumber(int max)
        {
            lock (SyncLock)
            { // synchronize
                return Getrandom.Next(max);
            }
        }

        public static string GenerateRandomFirstName()
        {
            var peopleNames = File.ReadAllLines(PeopleNameFile);
            var name = peopleNames[GetRandomNumber(0, peopleNames.Length)];

            return name;
        }

        public static string GenerateMassageInfo()
        {
            return ", " + GenerateMassageType();
        }

        public static string GenerateMassageType()
        {
            var generateNum = GetRandomNumber(50);

            if (generateNum < 18)
            {
                return "Deep Tissue Massage" + GenerateMassageDuration();
            }
            else if (generateNum < 40)
            {
                return "Sports Massage" + GenerateMassageDuration();
            }
            else if (generateNum < 47)
            {
                return "Couple Massage" + GenerateMassageDuration();
            }
            else if(generateNum < 49)
            {
                return "Shiatsu Massage" + " for 1 hour,";
            }

            return "Deep Tissue Massage" + GenerateMassageDuration();
        }

        public static string GenerateMassageDuration()
        {
            var generateNum = GetRandomNumber(10);
            return generateNum % 2 == 0 ? " for 1.5 hours," : " for 1 hour,";
        }

        public static string GenerateAppointmentTime()
        {
            var generateNum = GetRandomNumber(10);
            switch (generateNum)
            {
                case 0:
                    return " today after 6pm";
                case 1:
                    return " today after 7pm";
                case 2:
                    return " today after 8pm";
                case 3:
                    return " tomorrow after 6pm";
                case 4:
                    return " tomorrow after 7pm";
                case 5:
                    return " tomorrow after 8pm";
                case 6:
                    return " tonight";
                case 7:
                    return " tomorrow night";
                case 8:
                    return " tomorrow afternoon";
                case 9:
                    return " tomorrow morning";
                default:
                    return " tonight 8:00";
            }
        }
    }
}
