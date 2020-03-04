using System;
using System.Collections.Generic;
using Daikin.DotNetLib.MsTeams;
using Daikin.DotNetLib.MsTeams.Models;
using Xunit;

namespace Daikin.DotNetLib.Core.Tests
{
    public class Teams
    {
        #region Fields
        private readonly Config _configuration;
        #endregion

        #region Constructors
        public Teams()
        {
            _configuration = Config.GetConfiguration();
        }
        #endregion

        #region Methods
        [Fact]
        public void Send()
        {
            var request = new WebHookRequest
            {
                Summary = "Daikin.DotNetLib.MsTeams Test",
                Title = "Send Unit Test",
                ThemeColor = "#00a0e3",
                Text = "General message of item being communicated."
            };
            var section = new Section
            {
                Text = "Section of information, allowing multiple sections within a single message.", 
                Facts = new List<Fact>
                {
                    new Fact {Name = "Start Date: ", Value = DateTime.Now.ToLongDateString()},
                    new Fact {Name = "Start Time: ", Value = DateTime.Now.ToLongTimeString()},
                    new Fact {Name = "User: ", Value = Environment.UserDomainName + "\\" + Environment.UserName},
                    new Fact {Name = "Computer: ", Value = Environment.MachineName}
                }
            };
            request.Sections.Add(section);

            var (isSuccessful, response) = Notification.Send(
                webHookUrl: _configuration.TeamsWebHookUrl,
                request: request
            );

            Assert.True(isSuccessful);
            request = new WebHookRequest
            {
                Summary = "Daikin.DotNetLib.MsTeams Test",
                Title = "Send Unit Test Response",
                ThemeColor = "#00a0e3",
                Text = response
            };

            (isSuccessful, _) = Notification.Send(
                webHookUrl: _configuration.TeamsWebHookUrl,
                request: request
            );

            Assert.True(isSuccessful);
        }
        #endregion
    }
}
