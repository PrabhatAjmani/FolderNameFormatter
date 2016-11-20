using System;

namespace FolderNameFormatter
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            PrintInputDetails();

            Tuple<int, int, string> inputResult = GetInputResult();

            if (inputResult.Item1 == 0)
                return;

            try
            {
                var modifyDirectoryName = new ModifyDirectoryName();
                modifyDirectoryName.Rename(inputResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
        }

        private static void PrintInputDetails()
        {
            Console.WriteLine("Select 1 : For Title Case e.g. (My Computer) \n");
            Console.WriteLine("Select 2 : FOR ALL UPPER CASE e.g. (MY COMPUTER) \n");
            Console.WriteLine("Select 3 : for lower case e.g. (my computer)  \n");
            Console.WriteLine("Select 4 : To add the text \"TextToAdd\" in the beginning of folder name.\n");
            Console.WriteLine("Select 5 : To add the text \"TextToAdd\" in the end of folder name.\n");
            Console.WriteLine("Select 6 : For removing first 'N' characters from folder name.\n");
            Console.WriteLine("Select 7 : For removing last 'N' characters from folder name.\n");
            Console.WriteLine("Press 0 : To EXIT \n-----------------------------\n");
        }

        private static Tuple<int, int, string> GetInputResult()
        {
            int result;
            int noOfCharacters = 0;
            string textToAdd = string.Empty;

        begin: Console.WriteLine("Enter the case: ");

            if (!int.TryParse(Console.ReadLine(), out result) || result > 7 || result < 0)
            {
                Console.WriteLine("Given input is incorrect.\n\n------------------------\n");
                goto begin;
            }

            if (result < 4)
                return new Tuple<int, int, string>(result, 0, textToAdd);
            else if (result == 4 || result == 5)
            {
                Console.WriteLine("Enter the text you want to Add : ");
                textToAdd = Console.ReadLine();
            }
            else if (result == 6 || result == 7)
            {
            takeIntInput: Console.WriteLine("Enter the number of characters you want to Delete : ");

                if (!int.TryParse(Console.ReadLine(), out noOfCharacters) || noOfCharacters < 1)
                {
                    Console.WriteLine("Given input is incorrect.\n\n------------------------\n");
                    goto takeIntInput;
                }
            }

            return new Tuple<int, int, string>(result, noOfCharacters, textToAdd);
        }
    }
}