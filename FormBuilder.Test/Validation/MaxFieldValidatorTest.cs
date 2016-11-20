using CompositeC1Contrib.FormBuilder.Validation;

using NUnit.Framework;

namespace FormBuilder.Test.Validation
{
    public class MaxFieldValidatorTest : BaseValidatorTest<string>
    {
        public MaxFieldValidatorTest() : base(() => new MaxFieldLengthAttribute("test", 5)) { }

        [TestCase("1")]
        [TestCase("12")]
        [TestCase("1234")]
        public void LessSucceeds(string i)
        {
            var rule = CreateRule(i);

            Assert.That(rule.IsValid(), Is.True);
        }

        [TestCase("12345")]
        public void EqualsSucceeds(string i)
        {
            var rule = CreateRule(i);

            Assert.That(rule.IsValid(), Is.True);
        }

        [TestCase("123456")]
        [TestCase("1234567890")]
        public void MoreFails(string i)
        {
            var rule = CreateRule(i);

            Assert.That(rule.IsValid(), Is.False);
        }
    }
}
