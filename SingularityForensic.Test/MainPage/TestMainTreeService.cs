﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SingularityForensic.Contracts.Helpers;
using SingularityForensic.Contracts.MainPage;
using SingularityForensic.Contracts.TreeView;
using SingularityForensic.Contracts.TreeView.Events;
using System.Linq;

namespace SingularityForensic.Test.MainPage {
    [TestClass]
    public class TestMainTreeService {
        [TestInitialize]
        public void Initialize() {
            TestCommon.InitializeTest();
            _treeService = MainTreeService.Current as SingularityForensic.MainPage.MainTreeServiceImpl;
            Assert.IsNotNull(_treeService);
        }

        private SingularityForensic.MainPage.MainTreeServiceImpl _treeService;

        [TestMethod]
        public void TestAddUnit() {
            var unit = TreeUnitFactory.CreateNew(string.Empty);
            var addedCatched = false;
            var addStoken = CommonEventHelper.GetEvent<TreeUnitAddedEvent>().Subscribe(tuple => {
                addedCatched = true;
                Assert.AreEqual(unit, tuple.unit);
            });

            _treeService.AddUnit(null, unit);

            Assert.IsTrue(addedCatched);

            CommonEventHelper.GetEvent<TreeUnitAddedEvent>().Unsubscribe(addStoken);
        }

        [TestMethod]
        public void TestAddChildToUnit() {
            TestAddUnit();
            var unit = _treeService.CurrentUnits.ElementAt(0);
            var newUnit = TreeUnitFactory.CreateNew(string.Empty);

            var addedCatched = false;
            var areEqual = false;

            CommonEventHelper.GetEvent<TreeUnitAddedEvent>().Subscribe(tuple => {
                addedCatched = true;
                areEqual = newUnit == tuple.unit;
            });

            _treeService.AddUnit(unit, newUnit);
            Assert.AreEqual(unit.Children.Count, 1);
            Assert.IsTrue(areEqual);
            Assert.IsTrue(addedCatched);
        }

        [TestMethod]
        public void TestRemoveUnit() {
            TestAddUnit();
            var unit = _treeService.CurrentUnits.ElementAt(0);

            var removedCatched = false;
            var areEqual = false;


            CommonEventHelper.GetEvent<TreeUnitRemovedEvent>().Subscribe(tuple => {
                removedCatched = true;
                areEqual = unit == tuple.unit;
            });

            _treeService.RemoveUnit(unit);

            Assert.IsTrue(removedCatched);
            Assert.IsTrue(areEqual);
        }

        [TestMethod]
        public void TestSelectedChangedEvent() {
            TestAddUnit();

            var slUnit = _treeService.VM.TreeUnits[0];

            var selectedCatched = false;
            var areEqual = false;

            CommonEventHelper.GetEvent<TreeUnitSelectedChangedEvent>().Subscribe(tuple => {
                selectedCatched = true;
                areEqual = tuple.unit == slUnit;
            });
            
            _treeService.VM.SelectedUnit = slUnit;
            Assert.IsTrue(selectedCatched);
            Assert.IsTrue(areEqual);
        }

    }
}
