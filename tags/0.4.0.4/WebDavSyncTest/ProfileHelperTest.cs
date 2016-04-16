using WebDavSyncGUI.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebDavSync;
using System.Collections.Generic;

namespace WebDavSyncTest
{
    
    
    /// <summary>
    ///This is a test class for ProfileHelperTest and is intended
    ///to contain all ProfileHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ProfileHelperTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for saveProfiles
        ///</summary>
        [TestMethod()]
        public void saveProfilesTest()
        {
            string strProfilePath = @"E:\WebDavClient\WebDavSync\WebDavSync\WebDavSyncTest\WebDavSync.xml"; 
            ProfileHelper target = new ProfileHelper(strProfilePath);
            List<WebDAVClientProfile> listProfiles = new List<WebDAVClientProfile>();
            WebDAVClientProfile p1 = new WebDAVClientProfile();
            p1.ProfileName = "PREN1";
            p1.DavServerPath = "/ilias/webdav.php/hslu/ref_696271/TA.PRG2.F0903/Dateiaustausch/Projekt/Gruppe13/";
            p1.DavServer = "https://elearning.hslu.ch";
            p1.DavUser = "tfsuter";
            p1.DavPass = "JS_+U4JK";
            p1.LocalPath = @"D:\Daten\Sync\Gruppe 13\";
            listProfiles.Add(p1);

            WebDAVClientProfile p2 = new WebDAVClientProfile();
            p2.ProfileName = "PREN2";
            p2.DavServerPath = "/ilias/webdav.php/hslu/ref_696271/TA.PRG2.F0903/Dateiaustausch/Projekt/Gruppe14/";
            p2.DavServer = "https://elearning.hslu.ch";
            p2.DavUser = "tfsuter";
            p2.DavPass = "JS_+U4JK";
            p2.LocalPath = @"D:\Daten\Sync\Gruppe 14\";
            listProfiles.Add(p2);
            target.saveProfiles(listProfiles);
            
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for getProfiles
        ///</summary>
        [TestMethod()]
        public void getProfilesTest()
        {
            string strProfilePath = @"E:\WebDavClient\WebDavSync\WebDavSync\WebDavSyncTest\WebDavSync.xml"; // TODO: Initialize to an appropriate value
            ProfileHelper target = new ProfileHelper(strProfilePath); // TODO: Initialize to an appropriate value
            List<WebDAVClientProfile> actual;
            actual = target.getProfiles();
            Assert.AreEqual(2, actual.Count);
        }
    }
}
