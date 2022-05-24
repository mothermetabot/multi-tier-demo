using NUnit.Framework;
using System;
using Application2.Services;

namespace Tests.UnitTests.Application2
{
    public class MessageValidatorTests
    {
        private DefaultMessageValidator _validator = new DefaultMessageValidator();

        [OneTimeSetUp]
        public void Setup()
        {
            DefaultMessageValidator.MaxLength = 10;
        }


        [Test]
        public void Sanitize()
        {
            var longString = "12345678901234";
            var resultString = "5678901234";

            var sanitized = _validator.Sanitize(longString);

            Assert.IsTrue(resultString == sanitized);
            Assert.IsTrue(sanitized.Length <= DefaultMessageValidator.MaxLength);

            var shortString = "122";
            var shortSanitized = _validator.Sanitize(shortString);

            Assert.IsTrue(shortString == shortSanitized);


            var empty = string.Empty;
            var emptySanitized = _validator.Sanitize(empty);
            Assert.IsTrue(empty == emptySanitized);

            Assert.Throws<ArgumentNullException>(() => _validator.Sanitize(null));
        }

        [Test]
        public void Validate()
        {
            Assert.Throws<ArgumentNullException>(() => _validator.Validate(null));

            Assert.IsFalse(_validator.Validate("dfasdfasdfasdfasdfasdfasdf"));
            Assert.IsTrue(_validator.Validate("sasd)"));

            DefaultMessageValidator.MaxLength = 0;
            Assert.IsFalse(_validator.Validate("sasd)"));
            Assert.IsTrue(_validator.Validate(String.Empty));
        }
    }
}
