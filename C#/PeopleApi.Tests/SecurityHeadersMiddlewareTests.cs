using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using PeopleApi.Middlewares;

namespace PeopleApi.Tests.Middlewares;

[TestFixture]
public class SecurityHeadersMiddlewareTests
{
    private DefaultHttpContext _context;
    private Mock<RequestDelegate> _nextMock;
    private SecurityHeadersMiddleware _middleware;

    [SetUp]
    public void SetUp()
    {
        _context = new DefaultHttpContext();
        _context.Response.Body = new MemoryStream();
        _nextMock = new Mock<RequestDelegate>();
        _middleware = new SecurityHeadersMiddleware(_nextMock.Object);
    }

    [Test]
    public async Task InvokeAsync_AddsSecurityHeaders_AndCallsNext()
    {
        _nextMock.Setup(n => n(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

        await _middleware.InvokeAsync(_context);

        Assert.That(_context.Response.Headers["X-Content-Type-Options"], Is.EqualTo("nosniff"));
        Assert.That(_context.Response.Headers["X-Frame-Options"], Is.EqualTo("DENY"));
        Assert.That(_context.Response.Headers["X-XSS-Protection"], Is.EqualTo("1; mode=block"));
        _nextMock.Verify(n => n(It.IsAny<HttpContext>()), Times.Once);
    }
}
