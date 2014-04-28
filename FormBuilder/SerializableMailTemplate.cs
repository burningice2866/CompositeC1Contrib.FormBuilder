using System;

using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.FormBuilder
{
    [Serializable]
    public class SerializableMailTemplate
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool EncryptMessage { get; set; }
        public string EncryptPassword { get; set; }

        public static SerializableMailTemplate FromData(IMailTemplate template)
        {
            return new SerializableMailTemplate
            {
                From =  template.From,
                To = template.To,
                Cc = template.Cc,
                Bcc = template.Bcc,
                Subject = template.Subject,
                Body = template.Body,
                EncryptMessage = template.EncryptMessage,
                EncryptPassword = template.EncryptPassword
            };
        }
    }
}
