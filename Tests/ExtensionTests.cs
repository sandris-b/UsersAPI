using UsersAPI;

namespace Tests
{
    [TestClass]
    public class ExtensionTests
    {
        [TestMethod]
        public void GetAdditionalColumn_Test_Succeed()
        {
            string name = "Sandris Burnovskis";
            Assert.AreEqual(name.GetAdditionalColumn(), "SBurnovskis@ibsat.com");
        }

        [TestMethod]
        public void GetAdditionalColumn_Test_EmptyShouldBeReturned()
        {
            string name = "SandrisBurnovskis";
            Assert.AreEqual(name.GetAdditionalColumn(), string.Empty);

            string firstnameonly = "Sandris";
            Assert.AreEqual(firstnameonly.GetAdditionalColumn(), string.Empty);
        }
    }
}