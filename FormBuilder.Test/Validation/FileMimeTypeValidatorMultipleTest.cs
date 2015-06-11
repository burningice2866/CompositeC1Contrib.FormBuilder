using System.Collections.Generic;
using System.Linq;

using CompositeC1Contrib.FormBuilder;
using CompositeC1Contrib.FormBuilder.Validation;

using NUnit.Framework;

namespace FormBuilder.Test.Validation
{
    public class FileMimeTypeValidatorMultipleTest : BaseValidatorTest<IEnumerable<FormFile>>
    {
        public FileMimeTypeValidatorMultipleTest()
            : base(() => new FileMimeTypeValidatorAttribute("test", "application/pdf")) { }

        [TestCase("application/pdf")]
        [TestCase("application/pdf", "application/pdf")]
        [TestCase("application/pdf", "application/pdf", "application/pdf")]
        public void ValidFilesSucceeds(params string[] mimeTypes)
        {
            var files = mimeTypes.Select(m => new FormFile { ContentType = m });
            var rule = CreateRule(files);

            Assert.That(rule.Rule(), Is.True);
        }

        [TestCase("application/gif")]
        [TestCase("application/gif", "application/gif")]
        [TestCase("application/pdf", "application/gif")]
        public void InvalidFileFails(params string[] mimeTypes)
        {
            var files = mimeTypes.Select(m => new FormFile { ContentType = m });
            var rule = CreateRule(files);

            Assert.That(rule.Rule(), Is.False);
        }
    }
}