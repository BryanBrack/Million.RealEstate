using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Million.Application.DTOs;
using Million.Application.Interfaces;
using Million.Infrastructure.Services;
using Moq;
using NUnit.Framework;
using System.Data;

namespace Million.Tests
{
    [TestFixture]
    public class PropertyServiceTests
    {
        private Mock<IFileStorage> _mockFileStorage;
        private Mock<IConfiguration> _mockConfiguration;
        private PropertyService _propertyService;

        [SetUp]
        public void SetUp()
        {
            _mockFileStorage = new Mock<IFileStorage>();

            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(c => c.GetConnectionString("RealEstateDb")).Returns("FakeConnectionString");

            _propertyService = new PropertyService(
                _mockFileStorage.Object,
                _mockConfiguration.Object
            );

        }

        [Test]
        public async Task CreateOwnerAsync_CreatesOwner_WhenOwnerDoesNotExist()
        {
            using (var connection = new SqlConnection(_mockConfiguration.Name))
            {
                var ownerRequest = new CreateOwnerRequest
                {
                    Name = "John Doe",
                    Address = "123 Main St",
                    Photo = "photo_url",
                    Birthday = new System.DateTime(1980, 5, 10)
                };

                _mockConfiguration.Setup(conn => connection.QueryFirstOrDefaultAsync<int>(
                    "SELECT COUNT(1) FROM Owner WHERE Name = @Name", It.IsAny<object>(), null, null, null))
                    .ReturnsAsync(0); 

                _mockConfiguration.Setup(conn => connection.QuerySingleAsync<int>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                    .ReturnsAsync(1); 

                var result = await _propertyService.CreateOwnerAsync(ownerRequest);
            }
        }

        [Test]
        public async Task CreateOwnerAsync_ThrowsException_WhenOwnerExists()
        {
            using (var connection = new SqlConnection(_mockConfiguration.Name))
            {
                var ownerRequest = new CreateOwnerRequest
                {
                    Name = "John Doe",
                    Address = "123 Main St",
                    Photo = "photo_url",
                    Birthday = new System.DateTime(1980, 5, 10)
                };

                _mockConfiguration.Setup(conn => connection.QueryFirstOrDefaultAsync<int>(
                    "SELECT COUNT(1) FROM Owner WHERE Name = @Name", It.IsAny<object>(), null, null, null))
                    .ReturnsAsync(1);  // Simulamos que ya existe

                var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                    await _propertyService.CreateOwnerAsync(ownerRequest));
            }
        }
    }
}
