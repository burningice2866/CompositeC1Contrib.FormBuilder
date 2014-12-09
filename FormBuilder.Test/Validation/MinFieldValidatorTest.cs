using CompositeC1Contrib.FormBuilder.Validation;

using NUnit.Framework;

namespace FormBuilder.Test.Validation
{
    public class MinFieldValidatorTest : BaseValidatorTest<string>
    {
        public MinFieldValidatorTest() : base(() => new MinFieldLengthAttribute("test", 5)) { }

        [TestCase("1")]
        [TestCase("12")]
        [TestCase("1234")]
        public void LessFails(string i)
        {
            var rule = CreateRule(i);

            Assert.That(rule.Rule(), Is.False);
        }

        [TestCase("12345")]
        public void EqualsSucceeds(string i)
        {
            var rule = CreateRule(i);

            Assert.That(rule.Rule(), Is.True);
        }

        [TestCase("123456")]
        [TestCase("1234567890")]
        public void MoreSucceeds(string i)
        {
            var rule = CreateRule(i);

            Assert.That(rule.Rule(), Is.True);
        }
    }
}
