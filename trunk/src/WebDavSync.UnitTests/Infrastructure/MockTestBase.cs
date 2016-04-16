using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;

namespace WebDavSync.UnitTests.Infrastructure {

    public abstract class MockTestBase<TSut> {

        protected TSut Sut { get; set; }

        protected MockRepository MockFactory { get; private set; }

        [SetUp]
        public void SetUpMockTest() {
            this.MockFactory = new MockRepository();

            this.Sut = this.CreateSut();
        }

        protected abstract TSut CreateSut();
    }
}
