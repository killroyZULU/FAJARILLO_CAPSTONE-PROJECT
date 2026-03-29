using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FAJARILLO_FINAL_PROJECT.CLASSES
{
    internal class EmailControl
    {
        public static class EmailSender
        {

            /// <summary>
            /// Sends a password change notification email to the specified user. 
            /// The email includes the date and time of the change and a security warning if the action was unauthorized.
            /// </summary>
            /// <param name="toEmail"> The recipient's email address. </param>
            /// <param name="name"> The recipient's full name used in the greeting. </param>
            /// <param name="username"> The username associated with the account that had its password changed. </param>
            public static void SendEmail(string toEmail, string name, string username)
            {
                try
                {
                    string fromEmail = "fajarillolee@gmail.com"; // use a valid email
                    string password = "crsk szdt ioqw fcol";     // use an app-specific password

                    MailMessage message = new MailMessage();

                    message.From = new MailAddress(fromEmail, "ACCOUNT NOTIFICATION");
                    message.To.Add(toEmail);
                    message.Subject = "Password Change Notification";

                    string body = $"Hi {name},\n\n" +
                                  $"Your account '{username}' has had its password changed.\n\n" +
                                  $"Change Date & Time: {DateTime.Now:MMMM dd, yyyy hh:mm tt}\n\n" +
                                  $"If you did not perform this change, please contact support immediately.";

                    message.Body = body;

                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                    smtp.Credentials = new NetworkCredential(fromEmail, password);
                    smtp.EnableSsl = true;


                    smtp.Send(message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to send email: " + ex.Message);
                }
            }

            /// <summary>
            /// /// Sends a registration confirmation email to the specified user. 
            /// If the user's birthdate matches today's date, a birthday greeting is included in the message.
            /// </summary>
            /// <param name="user"> object containing recipient details such as name, email, username, and birthdate. </param>
            public static void SendRegistrationEmail(Users user)
            {
                string now = DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt");
                string birthdayNote = "";

                // Parse the user's birthdate string into a DateTime object
                if (DateTime.TryParse(user.Birthdate, out DateTime birthDate))
                {
                    // Compare only the Month and Day
                    if (birthDate.Month == DateTime.Now.Month && birthDate.Day == DateTime.Now.Day)
                    {
                        birthdayNote = "\n🎉 Happy Birthday! 🎉";
                    }
                }

                string body = $"Hi {user.Name},\n\n" +
                              $"Your account '{user.UserName}' has been successfully created on {now}.{birthdayNote}\n\n" +
                              "Thank you for registering!";

                try
                {
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress("fajarillolee@gmail.com");
                    mail.To.Add(user.Email);
                    mail.Subject = "Account Created";
                    mail.Body = body;

                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
                    {
                        Credentials = new NetworkCredential("fajarillolee@gmail.com", "crsk szdt ioqw fcol"),
                        EnableSsl = true
                    };

                    smtp.Send(mail);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Email failed to send: {ex.Message}");
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="email"></param>
            /// <param name="code"></param>
            public static void SendCodeToEmail(string email, string code)
            {
                try
                {
                    string fromEmail = "fajarillolee@gmail.com";
                    string password = "crsk szdt ioqw fcol"; // App password

                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(fromEmail);
                    mail.To.Add(email);
                    mail.Subject = "Verification Code";
                    mail.Body = "Your verification code is: " + code;

                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                    smtp.Credentials = new NetworkCredential(fromEmail, password);
                    smtp.EnableSsl = true;

                    smtp.Send(mail);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Email failed: " + ex.Message);
                }
            }

        }
    }
}
