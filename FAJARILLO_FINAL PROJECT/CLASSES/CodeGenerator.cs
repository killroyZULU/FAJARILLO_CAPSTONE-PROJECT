using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAJARILLO_FINAL_PROJECT.CLASSES
{
    internal class CodeGenerator
    {
        public static string GenerateCode()
        {
            string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string numbers = "0123456789";
            char[] code = new char[6];
            Random rand = new Random();

            // Add 3 letters
            for (int i = 0; i < 3; i++)
            {
                int index = rand.Next(letters.Length);
                code[i] = letters[index];
            }

            // Add 3 numbers
            for (int i = 3; i < 6; i++)
            {
                int index = rand.Next(numbers.Length);
                code[i] = numbers[index];
            }

            // Shuffle
            for (int i = 0; i < code.Length; i++)
            {
                int j = rand.Next(code.Length);
                char temp = code[i];
                code[i] = code[j];
                code[j] = temp;
            }

            return new string(code);
        }
    }
}
