namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class ValidationOptions
    {
        public bool ValidateFiles { get; set; }
        public bool ValidateCaptcha { get; set; }

        public ValidationOptions()
        {
            ValidateFiles = true;
            ValidateCaptcha = true;
        }
    }
}
