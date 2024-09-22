using FluentAssertions;
using PoC.VirtualManager.Playground.Apollo;

namespace PoC.VirtualManager.Playground.Tests
{
    [TestClass]
    public class ApolloTeamClientTests
    {
        [TestMethod]
        public async Task GetApolloTeamTest()
        {
            //Arrange
            var apolloClient = new ApolloTeamClient();

            //Act
            var team = await apolloClient.GetTeamAsync("apolloTeamId");

            //Assert
            team.Should().NotBeNull();
            team.Name.Should().Contain("Apollo");
        }

        [TestMethod]
        public async Task GetApolloTeamMemberTest()
        {
            //Arrange
            var apolloClient = new ApolloTeamClient();

            //Act
            var teamMember = await apolloClient.GetTeamMemberByEmailAsync("marco.agostinho@frotcom.com");

            //Assert
            teamMember.Should().NotBeNull();
            teamMember.Email.Should().Be("marco.agostinho@frotcom.com");
        }
    }
}