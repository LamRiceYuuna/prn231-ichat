namespace WebAPI.Utilities
{
    public static class RandomNumberGenerator
    {
        public static int GenerateRandomSixDigitNumber()
        {
            Random random = new Random();
            return random.Next(100000, 1000000);
        }
    }
}
