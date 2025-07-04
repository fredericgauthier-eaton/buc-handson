using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using PeopleApi.Middlewares;

namespace PeopleApi.Tests.Middlewares;

[TestFixture]
public class RequestLoggingMiddlewareTests
{
    private DefaultHttpContext _context;
    private Mock<RequestDelegate> _nextMock;
    private RequestLoggingMiddleware _middleware;
    private StringWriter _consoleOutput;

    [SetUp]
    public void SetUp()
    {
        _context = new DefaultHttpContext();
        _context.Request.Method = "GET";
        _context.Request.Path = "/test";
        _context.Response.Body = new MemoryStream();
        _nextMock = new Mock<RequestDelegate>();
        _middleware = new RequestLoggingMiddleware(_nextMock.Object);
        _consoleOutput = new StringWriter();
        Console.SetOut(_consoleOutput);
    }

    [TearDown]
    public void TearDown()
    {
        _consoleOutput.Dispose();
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
    }

    [Test]
    public async Task InvokeAsync_LogsRequestAndResponse()
    {
        _context.Response.StatusCode = 204;
        _nextMock.Setup(n => n(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

        await _middleware.InvokeAsync(_context);

        var output = _consoleOutput.ToString();
        StringAssert.Contains("Request: GET /test", output);
        StringAssert.Contains("Response: 204", output);
        _nextMock.Verify(n => n(_context), Times.Once);
    }

    [Test]
    public async Task InvokeAsync_LogsCorrectStatusCodeAfterNext()
    {
        _nextMock.Setup(n => n(It.IsAny<HttpContext>())).Callback<HttpContext>(ctx => ctx.Response.StatusCode = 201).Returns(Task.CompletedTask);

        await _middleware.InvokeAsync(_context);

        var output = _consoleOutput.ToString();
        StringAssert.Contains("Request: GET /test", output);
        StringAssert.Contains("Response: 201", output);
    }
}
