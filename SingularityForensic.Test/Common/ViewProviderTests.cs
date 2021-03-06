﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SingularityForensic.Common;
using SingularityForensic.Contracts.Common;

namespace SingularityForensic.Test.Common {
    [TestClass()]
    public class ViewProviderTests {
        [TestInitialize]
        public void Initialize() {
            TestCommon.InitializeTest();
            ViewProvider.SetViewProvider(new ViewProviderImpl(ServiceProvider.Current));
            Assert.IsNotNull(_viewProvider = ViewProvider.Current);
        }
        IViewProvider _viewProvider;

        [TestMethod()]
        public void SetViewProviderTest() {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetViewTest() {
            var hexView = _viewProvider.GetView(SingularityForensic.Contracts.Hex.Constants.HexView);
        }
    }
}