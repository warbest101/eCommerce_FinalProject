using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace WebBanHang.Services
{
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _sMSAccountFrom;

        public AuthMessageSender(/*IOptions<SMSoptions> optionsAccessor,*/IConfiguration config)
        {
            //Options = optionsAccessor.Value;
            _accountSid = config["TwilioSettings:SMSAccountIdentification"];
            _authToken = config["TwilioSettings:SMSAccountPassword"];
            _sMSAccountFrom = config["TwilioSettings:SMSAccountFrom"];
        }

        public SMSoptions Options { get; }  // set only via Secret Manager

        public Task SendEmailAsync(string email, string subject, string message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            // Your Account SID from twilio.com/console
            //var accountSid = Options.SMSAccountIdentification;
            // Your Auth Token from twilio.com/console
            //var authToken = Options.SMSAccountPassword;

            //TwilioClient.SetUsername(accountSid);
            //TwilioClient.SetPassword(authToken);
            //TwilioClient.Init(accountSid, authToken);

            TwilioClient.Init(_accountSid, _authToken);

            return MessageResource.CreateAsync(
              to: new PhoneNumber("+84" + number),
              from: new PhoneNumber(_sMSAccountFrom),
              body: message);
            /*
            return MessageResource.CreateAsync(
              to: new PhoneNumber("+84" + number),
              from: new PhoneNumber(Options.SMSAccountFrom),
              body: message);
            */
        }
    }
}
