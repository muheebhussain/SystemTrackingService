using Microsoft.VisualStudio.TestTools.UnitTesting;
using ActivityTracker.Tracker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityTracker.Tracker.Tests
{
    [TestClass()]
    public class ActivityTrackerTests
    {
        [TestMethod()]
        public void WatchActivityTest()
        {
            ActivityTracker.WatchActivity();
            //Assert.Fail();
        }
    }
}