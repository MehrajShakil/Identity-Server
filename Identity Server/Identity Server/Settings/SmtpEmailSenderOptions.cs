namespace Identity_Server.Settings
{
    public class SmtpEmailSenderOptions
    {
        public const string Root = "SMTPEmailSender:";
        public const string Local = "Local";
        public const string SendGrid = "SendGrid";
        public const string Mailtrap = "Mailtrap";

        public string Server { get; set; } = string.Empty;
        public string Port { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;


        public static string GetLocalServerPath() => string.Concat(Root + Local);
        public static string GetSendGridServerPath() => string.Concat(Root + SendGrid);
        public static string GetMailtrapServerPath() => string.Concat(Root + Mailtrap);

    }
}
