using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using PeopleApi.Middlewares;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace PeopleApi.Tests.Middlewares;

[TestFixture]
public class AuthMiddlewareTests
{
    private DefaultHttpContext _context;
    private Mock<RequestDelegate> _nextMock;
    private AuthMiddleware _middleware;

    [SetUp]
    public void SetUp()
    {
        _context = new DefaultHttpContext();
        _context.Response.Body = new MemoryStream();
        _nextMock = new Mock<RequestDelegate>();
        _middleware = new AuthMiddleware(_nextMock.Object);
    }

    [Test]
    public async Task InvokeAsync_BlankUsernameOrPassword_Returns401()
    {
        // No headers set
        await _middleware.InvokeAsync(_context);
        Assert.That(_context.Response.StatusCode, Is.EqualTo(401));
        _context.Response.Body.Seek(0, SeekOrigin.Begin);
        var response = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(_context.Response.Body);
        Assert.That(response?["message"], Is.EqualTo("Username or password can't be blank!"));
        _nextMock.Verify(n => n(It.IsAny<HttpContext>()), Times.Never);
    }

    [Test]
    public async Task InvokeAsync_InvalidCredentials_Returns401()
    {
        _context.Request.Headers["username"] = "wrong";
        _context.Request.Headers["password"] = "wrong";
        await _middleware.InvokeAsync(_context);
        Assert.That(_context.Response.StatusCode, Is.EqualTo(401));
        _context.Response.Body.Seek(0, SeekOrigin.Begin);
        var response = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(_context.Response.Body);
        Assert.That(response?["message"], Is.EqualTo("Invalid credentials!"));
        _nextMock.Verify(n => n(It.IsAny<HttpContext>()), Times.Never);
    }

    [Test]
    public async Task InvokeAsync_ValidCredentials_CallsNext()
    {
        _context.Request.Headers["username"] = "calaca";
        _context.Request.Headers["password"] = "12345";
        await _middleware.InvokeAsync(_context);
        Assert.That(_context.Response.StatusCode, Is.EqualTo(200)); // Default if not set
        _nextMock.Verify(n => n(It.IsAny<HttpContext>()), Times.Once);
    }
}
