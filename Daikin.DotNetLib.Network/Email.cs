using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Daikin.DotNetLib.Network
{
    /// <summary>
    /// Email Class to send SMTP email
    /// </summary>
    public class Email
    {
        // TODO: Catch various exceptions on sending message
        //   Invalid Credentials
        //   Missing email addresses
        //   Invalid SMTP Server (network connectivity issue)

        #region Enumerators
        /// <summary>
        /// Message Types
        /// </summary>
        public enum MessageType
        {
            Html,
            Text
        };
        #endregion

        #region Constants
        /// <summary>
        /// Default SMTP Port
        /// </summary>
        private const int DefaultSmtpPort = 25;

        /// <summary>
        /// End of Line for Text Messages
        /// </summary>
        private const string EndOfLineText = "\r\n";

        /// <summary>
        /// Expression for an email address
        /// </summary>
        public const string EmailAddressRegex = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
        #endregion

        #region Fields
        private string _receiveDisplayName;
        private MailMessage _mailMessage;
        #endregion

        #region Constructors
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Email()
        {
            InitEmail(MessageType.Text);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="messageType">Message Type</param>
        public Email(MessageType messageType)
        {
            InitEmail(messageType);
        }
        #endregion

        #region Properties
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        [Category("Data"), Description("TO Email Address to Send Message")]
        public string SendToAddresses { get; set; }

        [Category("Data"), Description("CC Email Address to Send Message")]
        public string SendCcAddresses { get; set; }

        [Category("Data"), Description("BCC Email Address to Send Message")]
        public string SendBccAddresses { get; set; }

        [Category("Data"), Description("Email Address to indicate the message came from")]
        public string ReceiveFromAddress { get; set; }

        [Category("Data"), Description("Display Name for the From")]
        public string ReceiveDisplayName
        {
            get { return _receiveDisplayName.Length > 0 ? _receiveDisplayName : ReceiveFromAddress; }
            set { _receiveDisplayName = value; }
        }

        [Category("Data"), Description("Specify the SMTP Server"), DefaultValue("localhost")]
        public string SmtpServer { get; set; }

        [Category("Data"), Description("Specify the SMTP Port"), DefaultValue("25")]
        public int SmtpPort { get; set; }

        [Category("Data"), Description("The SMTP Credentials Username")]
        public string Username { get; set; }

        [Category("Data"), Description("The SMTP Credentials Password")]
        public string Password { get; set; }
        public MessageType MessageFormat { get; set; }

        [Category("Data"), Description("Subject line of the email message")]
        public string Subject { get; set; }

        [Category("Data"), Description("Email message main content")]
        public string Body { get; set; }

        [Category("Layout"), Description("Return the end of line characters appropriate to the mailformat")]
        public string BodyNewLine
        {
            get
            {
                if (MessageFormat == MessageType.Html)
                {
                    return "<br>";
                }
                return EndOfLineText;
            }
        }

        [Category("Layout"), Description("Return the start of a paragraph characters appropriate to the mailformat")]
        public string ParagraphStart
        {
            get
            {
                if (MessageFormat == MessageType.Html)
                {
                    return "<p>";
                }
                return "";
            }
        }

        [Category("Layout"), Description("Return the end of a paragraph characters appropriate to the mailformat")]
        public string ParagraphStop
        {
            get
            {
                if (MessageFormat == MessageType.Html)
                {
                    return "</p>";
                }
                return EndOfLineText + EndOfLineText;
            }
        }

        [Category("Layout"), Description("Return the start of bold characters appropriate to the mailformat")]
        public string BoldStart
        {
            get
            {
                if (MessageFormat == MessageType.Html)
                {
                    return "<b>";
                }
                return "";
            }
        }

        [Category("Layout"), Description("Return the stop of bold characters appropriate to the mailformat")]
        public string BoldStop
        {
            get
            {
                if (MessageFormat == MessageType.Html)
                {
                    return "</b>";
                }
                return "";
            }
        }
        #endregion

        #region Methods
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>
        /// Initialize email settings
        /// </summary>
        /// <param name="messageType">Type of message to setup</param>
        private void InitEmail(MessageType messageType)
        {
            _mailMessage = new MailMessage();

            MessageFormat = messageType;
            SendToAddresses = null;
            ReceiveFromAddress = string.Empty;
            ReceiveDisplayName = string.Empty;
            Subject = string.Empty;
            Body = string.Empty;
            SmtpServer = "localhost";
            SmtpPort = DefaultSmtpPort;
            Username = string.Empty;
            Password = string.Empty;
        }

        /// <summary>
        /// Set SMTP Login Credentials
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        public void SetCredentials(string username, string password)
        {
            Username = username;
            Password = password;
        }

        /// <summary>
        /// Determine whether the specified email address is valid
        /// </summary>
        /// <param name="emailAddr">Email address to check</param>
        /// <returns>Whether the email address is valid</returns>
        public static bool ValidEmailAddress(string emailAddr)
        {
            Regex r = new Regex(EmailAddressRegex);
            Match m = r.Match(emailAddr);
            var isValid = m.Success;

            if (isValid) // if valid, do some additional checking
            {
                if ((emailAddr.IndexOf(" ") >= 0) || (emailAddr.IndexOf("|") >= 0) || (emailAddr.IndexOf("/") >= 0) || (emailAddr.IndexOf("\\") >= 0))
                {
                    isValid = false;
                }
                else
                {
                    int index = emailAddr.IndexOf("@"); // find the first @ symbol (caught in regex)
                    if (index > 0)
                    {
                        index = emailAddr.IndexOf("@", index + 1); // make sure no duplicate @'s included
                        if (index > 0)
                        {
                            isValid = false;
                        }
                    }
                }
            }
            return isValid;
        }

        /// <summary>
        /// Add a file attachment to the email message
        /// </summary>
        /// <param name="filename">Filename, with path, of the file to attach</param>
        public void AddAttachment(string filename)
        {
            try
            {
                Attachment attachment = new Attachment(filename);
                _mailMessage.Attachments.Add(attachment);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Failed to attach file '" + filename + "'.  Reason: " + e.Message);
            }
        }

        /// <summary>
        /// Add text to be bolded
        /// </summary>
        /// <param name="data">Data to bold</param>
        public void AppendBold(string data)
        {
            Body += BoldStart + data + BoldStop;
        }

        /// <summary>
        /// Add text to be paragraphed
        /// </summary>
        /// <param name="data">Data to paragraph</param>
        public void AppendParagraph(string data)
        {
            Body += ParagraphStart + data + ParagraphStop;
        }

        /// <summary>
        /// Add data to the message body
        /// </summary>
        /// <param name="body">Text to add to the message body</param>
        public void AppendBody(string body)
        {
            Body += body;
        }

        /// <summary>
        /// Remove existing body content.
        /// </summary>
        public void RemoveBody()
        {
            Body = string.Empty;
        }

        /// <summary>
        /// Add line break to message
        /// </summary>
        /// <param name="data">Data to include and then line break</param>
        public void AppendLineBreak(string data)
        {
            Body += data + BodyNewLine;
        }

        /// <summary>
        /// Set the message style
        /// </summary>
        /// <param name="messageType">Message Type</param>
        /// <remarks>
        /// Note that changing the message type mid-stream can be a problem as existing data is not removed and may be encoded in another message type.
        /// </remarks>
        public void BodyStyle(MessageType messageType)
        {
            MessageFormat = messageType;
        }

        /// <summary>
        /// Send email message
        /// </summary>
        /// <returns>Success of sending</returns>
        public bool Send()
        {
            bool isSent = false;

            string sendToAddresses = SendToAddresses;
            sendToAddresses = sendToAddresses.Replace(';', ',');
            if (sendToAddresses.Length > 0)
            {
                _mailMessage.To.Add(sendToAddresses); // Comma separated list
            }

            string sendCcAddresses = SendCcAddresses;
            sendCcAddresses = sendCcAddresses.Replace(';', ',');
            if (sendCcAddresses.Length > 0)
            {
                _mailMessage.CC.Add(sendCcAddresses);
            }

            string sendBccAddresses = SendBccAddresses;
            sendBccAddresses = sendBccAddresses.Replace(';', ',');
            if (sendBccAddresses.Length > 0)
            {
                _mailMessage.Bcc.Add(sendBccAddresses);
            }

            MailAddress mailFromAddress = new MailAddress(ReceiveFromAddress, ReceiveDisplayName);
            _mailMessage.From = mailFromAddress;

            // Replace obsoleted method with new list method
            //_mailMessage.ReplyTo = mailFromAddress;
            _mailMessage.ReplyToList.Add(mailFromAddress);

            _mailMessage.IsBodyHtml = (MessageFormat == MessageType.Html);
            _mailMessage.Subject = Subject;
            _mailMessage.Body = Body;

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = SmtpServer;
            smtpClient.Port = SmtpPort;
            if (Username.Length > 0)
            {
                NetworkCredential networkCredential = new NetworkCredential(Username, Password);
                smtpClient.Credentials = networkCredential;
            }

            try
            {
                smtpClient.Send(_mailMessage);
                isSent = true;
            }
            catch
            {
                // do nothing
            }

            return isSent;
        }

        /// <summary>
        /// Send mail message
        /// </summary>
        /// <param name="sendToAddresses">TO email addresses (comma separated list)</param>
        /// <param name="receiveFromAddresses">FROM Email Address</param>
        /// <param name="smtpServer">SMTP Mail Server</param>
        /// <param name="subject">Message Subject</param>
        /// <param name="body">Message Body</param>
        /// <returns>Success of sending message</returns>
        public bool Send(string sendToAddresses, string receiveFromAddresses, string smtpServer, string subject, string body)
        {
            return Send(sendToAddresses, String.Empty, string.Empty, receiveFromAddresses, ReceiveDisplayName, smtpServer, DefaultSmtpPort, subject, body);
        }

        /// <summary>
        /// Send mail message
        /// </summary>
        /// <param name="sendToAddresses">TO email addresses (comma separated list)</param>
        /// <param name="receiveFromAddresses">FROM Email Address</param>
        /// <param name="displayFrom">Display name of FROM Email Address</param>
        /// <param name="smtpServer">SMTP Mail Server</param>
        /// <param name="subject">Message Subject</param>
        /// <param name="body">Message Body</param>
        /// <returns>Success of sending message</returns>
        public bool Send(string sendToAddresses, string receiveFromAddresses, string displayFrom, string smtpServer, string subject, string body)
        {
            return Send(sendToAddresses, String.Empty, string.Empty, receiveFromAddresses, displayFrom, smtpServer, DefaultSmtpPort, subject, body);
        }

        /// <summary>
        /// Send mail message
        /// </summary>
        /// <param name="sendToAddresses">TO email addresses (comma separated list)</param>
        /// <param name="sendCcAddresses">CC email addresses (comma separated list)</param>
        /// <param name="sendBccAddresses">BCC email addresses (comma separated list)</param>
        /// <param name="receiveFromAddress">FROM Email Address</param>
        /// <param name="displayFrom">Display name of FROM Email Address</param>
        /// <param name="smtpServer">SMTP Mail Server</param>
        /// <param name="subject">Message Subject</param>
        /// <param name="body">Message Body</param>
        /// <returns>Success of sending message</returns>
        public bool Send(string sendToAddresses, string sendCcAddresses, string sendBccAddresses, string receiveFromAddress, string displayFrom, string smtpServer, string subject, string body)
        {
            return Send(sendToAddresses, sendCcAddresses, sendBccAddresses, receiveFromAddress, displayFrom, smtpServer, DefaultSmtpPort, subject, body);
        }

        /// <summary>
        /// Send mail message
        /// </summary>
        /// <param name="sendToAddresses">TO email addresses (comma separated list)</param>
        /// <param name="sendCcAddresses">CC email addresses (comma separated list)</param>
        /// <param name="sendBccAddresses">BCC email addresses (comma separated list)</param>
        /// <param name="receiveFromAddress">FROM Email Address</param>
        /// <param name="displayFrom">Display name of FROM Email Address</param>
        /// <param name="smtpServer">SMTP Mail Server</param>
        /// <param name="smtpPort">SMTP Server Port</param>
        /// <param name="subject">Message Subject</param>
        /// <param name="body">Message Body</param>
        /// <returns>Success of sending message</returns>
        public bool Send(string sendToAddresses, string sendCcAddresses, string sendBccAddresses, string receiveFromAddress, string displayFrom, string smtpServer, int smtpPort, string subject, string body)
        {
            SendToAddresses = sendToAddresses;
            SendCcAddresses = sendCcAddresses;
            SendBccAddresses = sendBccAddresses;
            ReceiveFromAddress = receiveFromAddress;
            ReceiveDisplayName = displayFrom;
            SmtpServer = smtpServer;
            SmtpPort = smtpPort;
            Subject = subject;
            Body = body;
            return Send();
        }
        #endregion

    }
}
