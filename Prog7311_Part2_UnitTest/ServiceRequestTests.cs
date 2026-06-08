using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using APIConnectorCore.Controllers;
using APIConnectorCore.Models;
using APIConnectorCore.Repositories;

namespace Prog7311_UnitTests
{
    public class ServiceRequestTests
    {
        [Fact]
        public async Task GetServiceRequest_ReturnsNotFound_WhenRequestDoesNotExist()
        {
            var mockRepo = new Mock<IServiceRequestRepository>();

            mockRepo
                .Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((ServiceRequest?)null);

            var controller = new ServiceRequestsController(mockRepo.Object);

            var result = await controller.GetServiceRequest(99);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}