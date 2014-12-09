using CompositeC1Contrib.FormBuilder;
using CompositeC1Contrib.FormBuilder.Validation;

using NUnit.Framework;

namespace FormBuilder.Test.Validation
{
    public class FileMimeTypeValidatorSingleTest : BaseValidatorTest<FormFile>
    {
        public FileMimeTypeValidatorSingleTest()
            : base(() => new FileMimeTypeValidatorAttribute("test", "application/pdf"))
        {
        }

        [TestCase("application/pdf")]
        public void ValidFileSucceeds(string mimeType)
        {
            var rule = CreateRule(new FormFile { ContentType = mimeType });

            Assert.That(rule.Rule(), Is.True);
        }

        [TestCase("application/gif")]
        [TestCase("application/jpeg")]
        public void InvalidFileFails(string mimeType)
        {
            var rule = CreateRule(new FormFile { ContentType = mimeType });

            Assert.That(rule.Rule(), Is.False);
        }
    }
}