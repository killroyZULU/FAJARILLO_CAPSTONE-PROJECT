using System.Collections.Generic;

namespace FAJARILLO_FINAL_PROJECT.CLASSES
{
    internal class FileReadWrite
    {
        public static List<Users> ReadAllUsers()
        {
            return AppRepository.LoadUsers();
        }

        public static void AppendUser(Users user)
        {
            AppRepository.UpsertUser(user);
        }

        public static bool UserExists(string username, string email)
        {
            return AppRepository.UserExists(username, email);
        }

        public static Users ValidateLogin(string email, string password)
        {
            return AppRepository.Authenticate(email, password);
        }

        public static void UpdateUser(Users updatedUser)
        {
            AppRepository.UpsertUser(updatedUser);
        }
    }
}
