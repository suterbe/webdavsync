using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using WebDavSync.Core;
using System.IO;
using WebDavSync.UnitTests.Infrastructure;
using Rhino.Mocks;

namespace WebDavSync.UnitTests.Core {
    [TestFixture]
    public class ProfileManagerTests : MockTestBase<ProfileManager> {

        private string ProfilePath {
            get { return Path.Combine(Environment.CurrentDirectory, "TestProfile.xml"); }
        }

        private ICryptoService CryptoServiceMock { get; set; }

        protected override ProfileManager CreateSut() {
            this.CryptoServiceMock = this.MockFactory.StrictMock<ICryptoService>();

            return new ProfileManager(this.ProfilePath, this.CryptoServiceMock);
        }

        [Test]
        public void Save_and_load_a_profile_roundtrip() {
            var profileName = @"John's Profile";
            var davServerPath = @"/Home";
            var davServer = @"http://localhost:2345";
            var davUser = @"john.doe@google.com";
            var davPass = @"p23on2vgöewn";
            var localPath = @"C:\Users\John\Downloads";
            var overrideItems = true;

            var profiles = new List<WebDAVClientProfile> {
                new WebDAVClientProfile {
                    ProfileName = profileName,
                    DavServer = davServer,
                    DavServerPath=davServerPath,
                    DavUser = davUser,
                    DavPass = davPass, 
                    LocalPath = localPath, 
                    OverrideItems = overrideItems
                }
            };

            var encryptedPassword = @"$wünü2n3fäpja_p3w$nvq3p4ofn32$2nsaföjwe";

            List<WebDAVClientProfile> loadedProfiles = null;

            With.Mocks(this.MockFactory).Expecting(delegate {
                Expect.Call(this.CryptoServiceMock.Encrypt(davPass))
                    .Return(encryptedPassword);
                Expect.Call(this.CryptoServiceMock.Decrypt(encryptedPassword))
                    .Return(davPass);
            }).Verify(delegate {

                // Save
                this.Sut.SaveProfiles(profiles);

                // Load
                loadedProfiles = this.Sut.GetProfiles();
            });

            Assert.IsTrue(File.Exists(this.ProfilePath));

            Assert.AreEqual(1, loadedProfiles.Count);

            var loadedProfile = loadedProfiles[0];
            Assert.AreEqual(profileName, loadedProfile.ProfileName);
            Assert.AreEqual(davServerPath, loadedProfile.DavServerPath);
            Assert.AreEqual(davServer, loadedProfile.DavServer);
            Assert.AreEqual(davUser, loadedProfile.DavUser);
            Assert.AreEqual(davPass, loadedProfile.DavPass);
            Assert.AreEqual(localPath, loadedProfile.LocalPath);
            Assert.AreEqual(overrideItems, loadedProfile.OverrideItems);
        }


    }
}
