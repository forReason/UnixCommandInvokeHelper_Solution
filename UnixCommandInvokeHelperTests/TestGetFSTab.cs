using Renci.SshNet;
using UnixCommandInvokeHelper;

namespace UnixCommandInvokeHelperTests
{
    public class TestGetFSTab
    {
        [Fact]
        public void Test1()
        {
            SshCommandHelper helper = new SshCommandHelper("testuser", "foobar", null, "testrig");
            CommandResult result = helper.RunCommandSudo("cat /etc/fstab", "foobar");
            Assert.Null(result.Exception);
            Assert.True(string.IsNullOrEmpty(result.Errors));
        }
    }
}