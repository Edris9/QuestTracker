using System;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using QuestTracker.Models;

namespace QuestTracker.Services
{
    public class NotificationService
    {
        private string _accountSid;
        private string _authToken;
        private string _twilioPhoneNumber;

        public NotificationService()
        {
            _accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            _authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
            _twilioPhoneNumber = Environment.GetEnvironmentVariable("TWILIO_PHONE_NUMBER");

            if (string.IsNullOrWhiteSpace(_accountSid) || string.IsNullOrWhiteSpace(_authToken))
            {
                Console.Error.WriteLine("❌ Missing TWILIO_ACCOUNT_SID or TWILIO_AUTH_TOKEN.");
                return;
            }

            if (string.IsNullOrWhiteSpace(_twilioPhoneNumber))
            {
                Console.Error.WriteLine("❌ Missing TWILIO_PHONE_NUMBER.");
                return;
            }

            try
            {
                TwilioClient.Init(_accountSid, _authToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Twilio init error: {ex.Message}");
            }
        }

        // Skicka SMS med valfritt telefonnummer
        public bool SendSMS(string phoneNumber, string message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phoneNumber))
                {
                    Console.WriteLine("❌ Error: Phone number is empty!");
                    return false;
                }

                // Validera telefonnummer format
                if (!phoneNumber.StartsWith("+"))
                {
                    phoneNumber = "+" + phoneNumber;
                }

                var sms = MessageResource.Create(
                    body: message,
                    from: new PhoneNumber(_twilioPhoneNumber),
                    to: new PhoneNumber(phoneNumber)
                );

                Console.WriteLine($"✅ SMS skickat till {phoneNumber}");
                return !string.IsNullOrEmpty(sms.Sid);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ SMS Error: {ex.Message}");
                return false;
            }
        }

        // Skicka deadline-varning
        public bool SendDeadlineAlert(string phoneNumber, string userName, string questTitle)
        {
            string message = $"⚔️ Hjälte {userName}, ditt uppdrag '{questTitle}' måste slutföras idag!";
            return SendSMS(phoneNumber, message);
        }

        // Skicka completion-meddelande
        public bool SendCompletionNotification(string phoneNumber, string userName, string questTitle)
        {
            string message = $"✅ Bra gjort, {userName}! Du har slutfört '{questTitle}'!";
            return SendSMS(phoneNumber, message);
        }

        // Skicka quest-påminnelse
        public bool SendQuestReminder(string phoneNumber, string questTitle, int daysLeft)
        {
            string message = $"⏰ Påminnelse: '{questTitle}' - {daysLeft} dagar kvar!";
            return SendSMS(phoneNumber, message);
        }
    }
}