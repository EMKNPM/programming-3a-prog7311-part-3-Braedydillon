using Xunit;
using Microsoft.AspNetCore.Http;
using Moq;
using System.IO;

namespace Prog7311_UnitTests
{
    public class FileValidationTests
    {
        [Theory]
        [InlineData("document.pdf", true)]
        [InlineData("virus.exe", false)]
        [InlineData("picture.png", false)]
        public void IsFileValid_ShouldOnlyAllowPdf(string fileName, bool expectedResult)
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns(fileName);

            var extension = Path.GetExtension(fileMock.Object.FileName).ToLower();

            bool isValid = extension == ".pdf";

            Assert.Equal(expectedResult, isValid);
        }
    }
}