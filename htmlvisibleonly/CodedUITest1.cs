using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using htmlvisibleonly.RecordedUIMapClasses;

namespace htmlvisibleonly
{
    [CodedUITest]
    public class CodedUITest1
    {
        [TestMethod]
        [DeploymentItem("example2.htm")]
        [ExpectedException(typeof(FailedToPerformActionOnHiddenControlException))]
        public void Mouse_click_will_fail_because_hidden_link_is_matched_instead_of_visible_link()
        {
            // Playback.PlaybackSettings.SmartMatchOptions = SmartMatchOptions.None; // tried Sudhish Mathuria's suggestion

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
            
            visibleLink.Find(); // THIS SHOULD BE THE VISIBLE ONE

            Debug.WriteLine("BoundingRectangle: " + visibleLink.BoundingRectangle);
            Assert.IsTrue(visibleLink.BoundingRectangle.Width > 0, "Width should positive.");
            Assert.IsTrue(visibleLink.BoundingRectangle.Height > 0, "Height should positive.");

            Mouse.Click(visibleLink);
        }

        [TestMethod]
        [DeploymentItem("example2.htm")]
        public void Mouse_click_will_succeed_because_recorded_filter_properties_includes_tag_instance_number()
        {
            var example2Path = Path.Combine(TestContext.TestDeploymentDir, "example2.htm");
            
            var window = BrowserWindow.Launch(example2Path);

            var map = new RecordedUIMap();
            map.UIItem2ad6de5575f1403fWindow.CopyFrom(window);

            var visibleLink = map.UIItem2ad6de5575f1403fWindow.UIItem2ad6de5575f1403fDocument.UIHelloHyperlink;
            
            visibleLink.Find();
            visibleLink.DrawHighlight();

            Mouse.Click(visibleLink);
        }

        [TestMethod]
        [DeploymentItem("example2.htm")]
        [ExpectedException(typeof(FailedToPerformActionOnHiddenControlException))]
        public void Mouse_click_will_fail_because_recorded_map_based_on_tag_instance_should_be_based_on_VisibleOnly_for_real_use()
        {
            var example2Path = Path.Combine(TestContext.TestDeploymentDir, "example2.htm");

            var window = BrowserWindow.Launch(example2Path);

            var map = new RecordedUIMap();
            map.UIItem2ad6de5575f1403fWindow.CopyFrom(window);

            var visibleLink = map.UIItem2ad6de5575f1403fWindow.UIItem2ad6de5575f1403fDocument.UIHelloHyperlink;

            // remove tag instance filter because relative tag order of visible control will change dynamically in actual system under test
            visibleLink.FilterProperties.Remove(HtmlControl.PropertyNames.TagInstance);

            // add VisibleOnly search configuration to match visible item regardless of relative tag position
            visibleLink.SearchConfigurations.Add(SearchConfiguration.VisibleOnly);

            visibleLink.Find();

            Mouse.Click(visibleLink);
        }

        public TestContext TestContext { get; set; }
    }
}
