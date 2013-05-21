using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace htmlvisibleonly
{
    [CodedUITest]
    public class CodedUITest1
    {
        [TestMethod]
        [DeploymentItem("example2.htm")]
        [ExpectedException(typeof(FailedToPerformActionOnHiddenControlException))]
        public void CodedUITestMethod1()
        {
            var example2Path = Path.Combine(TestContext.TestDeploymentDir, "example2.htm");
            
            var window = BrowserWindow.Launch(example2Path);

            var document = new HtmlDocument(window);
            document.FilterProperties[HtmlDocument.PropertyNames.Title] = "2ad6de55-75f1-403f-8fcb-1d5defac9786";
            
            var visibleLink = new HtmlHyperlink(document);
            visibleLink.SearchProperties[HtmlHyperlink.PropertyNames.InnerText] = "Hello";
            
            // ONLY FIND VISIBLE CONTROLS !
            visibleLink.SearchConfigurations.Add(SearchConfiguration.VisibleOnly);

            var allMatches = visibleLink.FindMatchingControls();
            Assert.AreEqual(2, allMatches.Count, "Should be two matching.");
            
            visibleLink.Find(); // THIS SHOULD BE THE VISIBLE ONE CARE

            Debug.WriteLine("BoundingRectangle: " + visibleLink.BoundingRectangle.ToString());
            Assert.IsTrue(visibleLink.BoundingRectangle.Width > 0, "Width should positive.");
            Assert.IsTrue(visibleLink.BoundingRectangle.Height > 0, "Height should positive.");

            Mouse.Click(visibleLink);
        }

        public TestContext TestContext { get; set; }
    }
}
