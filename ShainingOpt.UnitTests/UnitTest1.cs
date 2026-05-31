using FluentAssertions;
using ShainingOpt.Controllers;
using ShainingOpt.Helpers;

namespace ShainingOpt.UnitTests
{
    public class UnitTest1
    {
        [Theory]
        [InlineData(15, 15, 1)]
        [InlineData(16, 15, 2)]
        [InlineData(14, 15, 1)]
        public void CalculatePagination_ShouldCalculateTotalPages(int totalItems, int pageSize, int expected)
        {
            //Arrange

            //Act
            var result = PaginationHelpers.CalculatePagination(totalItems, pageSize, 1);

            // Assert
            result.TotalPages.Should().Be(expected);
        }

        [Fact]
        public void CalculatePagination_ShouldCalculateTotalPages2()
        {
            // Arrange
            int totalItems = 31;
            int pageSize = 15;
            int pageNumber = 1;

            // Act
            var result = PaginationHelpers.CalculatePagination(
                totalItems,
                pageSize,
                pageNumber);

            // Assert
            result.TotalPages.Should().Be(3);
        }
    }
}
