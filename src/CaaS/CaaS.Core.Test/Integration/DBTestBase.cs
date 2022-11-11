    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CaaS.Core.Test.Integration
{
    public abstract class DBTestBase
    {
        protected TestcontainersContainer? testcontainer;

        [OneTimeSetUp]
        public async Task Setup()
        {
            // if ryuk image can't be loaded:
            // docker pull testcontainers/ryuk:0.3.4

            var imageName = await new ImageFromDockerfileBuilder()
              .WithName("caas-dev-db-integration")
              .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), "Docker\\db_dev")
              .WithDockerfile("Dockerfile")
              .WithDeleteIfExists(true)
              .WithCleanUp(true)
              .Build();

            // wait strategy not working
            using (var consumer = Consume.RedirectStdoutAndStderrToStream(new MemoryStream(), new MemoryStream()))
            {
                var nginxContainer = new TestcontainersBuilder<TestcontainersContainer>()
                .WithCleanUp(true)
                .WithAutoRemove(true)
              .WithName(Guid.NewGuid().ToString("D"))
              .WithImage("caas-dev-db")
              .WithExposedPort(3306)
              .WithPortBinding(3306)
              .WithOutputConsumer(consumer)
              .WithWaitStrategy(Wait.ForWindowsContainer().UntilMessageIsLogged(consumer.Stdout,"ready for connections"))
              .Build();
               await nginxContainer.StartAsync().ConfigureAwait(false);
            }
            

        }

        [TearDown]
        public void Teardown()
        {

        }
    }
}
