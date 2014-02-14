using System;

using Composite.Core.WebClient.Captcha;

namespace CompositeC1Contrib.FormBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RequiresCaptchaAttribute : Attribute
    {
        private readonly Lazy<string> _encryptedValue = new Lazy<string>(Captcha.CreateEncryptedValue);

        public const string HiddenFieldName = "__captchaEncrypted";
        public const string InputName = "__captchaValue";

        public string EncryptedValue
        {
            get { return _encryptedValue.Value; }
        }

        public static bool IsValid(string encryptedValue, string postedValue)
        {
            string value;
            DateTime dt;
            
            return Captcha.Decrypt(encryptedValue, out dt, out value) && value == postedValue;
        }
    }
}
