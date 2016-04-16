using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using WebDavSync.Core;
using WebDavSync.UnitTests.Infrastructure;

namespace WebDavSync.UnitTests.Core {

    [TestFixture]
    public class CryptoServiceTests : MockTestBase<CryptoService> {

        protected override CryptoService CreateSut() {
            return new CryptoService();
        }

        [Test]
        public void Encrypt_an_emtpy_string_should_return_an_empty_string_too() {
            Assert.IsNullOrEmpty(this.Sut.Encrypt(string.Empty));
        }

        [Test]
        public void Encrypt_null_should_return_an_empty_string() {
            Assert.IsNullOrEmpty(this.Sut.Encrypt(null));
        }

        [Test]
        public void Decrypt_an_emtpy_string_should_return_an_emtpy_string_too() {
            Assert.IsNullOrEmpty(this.Sut.Decrypt(string.Empty));
        }

        [Test]
        public void Decrypt_null_should_return_an_emtpy_string() {
            Assert.IsNullOrEmpty(this.Sut.Decrypt(string.Empty));
        }

        [Test]
        public void Decrypt_illegal_string_should_return_empty_string() {
            Assert.IsNullOrEmpty(this.Sut.Decrypt("$"));
        }

        [Test]
        public void Encrypt_and_decrypt_a_string_should_return_original_string() {
            var original = "Franz jagt im Taxi durch das komplett verwahrloste Paris oder so ähnlich";

            var encrypted = this.Sut.Encrypt(original);

            var decrypted = this.Sut.Decrypt(encrypted);

            Assert.AreEqual(original, decrypted);
        }
    }
}
