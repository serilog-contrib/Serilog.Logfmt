# Serilog.Logfmt - A small and simple logfmt formatter for Serilog

This project contains a simple formatter to format logs in logfmt format using Serilog.

## Why logfmt?

Logmft is a compact log format which is is growing in popularity. Using this formatter your programs will generate logs using logfmt allowing easy integrations with products like [Loki](https://grafana.com/oss/loki/) or [hutils](https://github.com/brandur/hutils) among others.

## Adding to your project

Install the nuget package `Serilog.Logfmt`. This package targets `netstandard2.0`:

```
dotnet add package Serilog.Logfmt
```

## How to enable it

Just pass a instance of `LogfmtFormatter` to the Serilog Sink:

```csharp
.UseSerilog((hostBuilderContext, config) =>
{
    config.MinimumLevel.Verbose()
    .Enrich.FromLogContext()
    .WriteTo.Console(new LogfmtFormatter());
})
```

## Configuring the formatter

Formatter allows for several options to customize the output.

### Including all LogEvent properties

By default, the formatter only serializes the timestamp, level and message properties of the `LogEvent` object. However you can choose to serialize all scalar properties.

> Currently only scalar properties are serialized. Complex (structure, dictionary or array) properties are not serialized.

```csharp
new LogfmtFormatter(opt.IncludeAllProperties())
```

### Preserving case of property names

By default, the formatter converts all property names to snake_case. But you can choose to preserve property names as they are in the `LogLevel` object:

```csharp
new LogfmtFormatter(opt => opt.PreserveCase())
```

> Properties `ts` (timestamp), `level` (Log level) and `msg` (message) have only these names regardless this option. Also, timestamps are always in UTC.

### Log levels formats

By default, the formatter use values for the log levels that are recognized by [Grafana](https://grafana.com/). For example if log level is "Verbose" an entry with `level=trace` will be generated. Buy you can use the same values defined in the `LogEventLevel` enum:

```csharp
new LogfmtFormatter(opt => opt.PreserveSerilogLogLevels())
```

### Exception logging

If an exception is logged, the default behavior is:

1. Log the exeception type and message with level=err
2. Do **not** log Stack trace

```
ts=2021-02-19T18:23:15.1491195Z level=err exception=IOException err="Authentication failed because the remote party has closed the transport stream."
```

You can configure this behavior:

```csharp
new LogfmtFormatter(opt =>
    opt.OnException(e => e
        // log Only message and level (err) but not exception type
        .LogExceptionData(LogfmtExceptionDataFormat.Message | LogfmtExceptionDataFormat.Level)
        // Log full stack trace
        .LogStackTrace(LogfmtStackTraceFormat.All)
    ))
```

Generates following level:

```
ts=2021-02-19T18:28:20.9512682Z level=err err="Authentication failed because the remote party has closed the transport stream."    at System.Net.Security.SslStream.StartReadFrame(Byte[] buffer, Int32 readBytes, AsyncProtocolRequest asyncRequest)
   at System.Net.Security.SslStream.PartialFrameCallback(AsyncProtocolRequest asyncRequest)
--- End of stack trace from previous location where exception was thrown ---
   at System.Net.Security.SslStream.ThrowIfExceptional()
   at System.Net.Security.SslStream.InternalEndProcessAuthentication(LazyAsyncResult lazyResult)
   at System.Net.Security.SslStream.EndProcessAuthentication(IAsyncResult result)
   at System.Net.Security.SslStream.EndAuthenticateAsServer(IAsyncResult asyncResult)
   at System.Net.Security.SslStream.<>c.<AuthenticateAsServerAsync>b__69_1(IAsyncResult iar)
   at System.Threading.Tasks.TaskFactory`1.FromAsyncCoreLogic(IAsyncResult iar, Func`2 endFunction, Action`1 endAction, Task`1 promise, Boolean requiresSynchronization)
--- End of stack trace from previous location where exception was thrown ---
   at Microsoft.AspNetCore.Server.Kestrel.Https.Internal.HttpsConnectionMiddleware.InnerOnConnectionAsync(ConnectionContext context)
ts=2021-02-19T18:28:20.9512779Z level=debug msg="Failed to authenticate HTTPS connection."
ts=2021-02-19T18:28:20.9512779Z level=err err="Authentication failed because the remote party has closed the transport stream."    at System.Net.Security.SslStream.StartReadFrame(Byte[] buffer, Int32 readBytes, AsyncProtocolRequest asyncRequest)
   at System.Net.Security.SslStream.PartialFrameCallback(AsyncProtocolRequest asyncRequest)
--- End of stack trace from previous location where exception was thrown ---
   at System.Net.Security.SslStream.ThrowIfExceptional()
   at System.Net.Security.SslStream.InternalEndProcessAuthentication(LazyAsyncResult lazyResult)
   at System.Net.Security.SslStream.EndProcessAuthentication(IAsyncResult result)
   at System.Net.Security.SslStream.EndAuthenticateAsServer(IAsyncResult asyncResult)
   at System.Net.Security.SslStream.<>c.<AuthenticateAsServerAsync>b__69_1(IAsyncResult iar)
   at System.Threading.Tasks.TaskFactory`1.FromAsyncCoreLogic(IAsyncResult iar, Func`2 endFunction, Action`1 endAction, Task`1 promise, Boolean requiresSynchronization)
--- End of stack trace from previous location where exception was thrown ---
   at Microsoft.AspNetCore.Server.Kestrel.Https.Internal.HttpsConnectionMiddleware.InnerOnConnectionAsync(ConnectionContext context)
```

> Keep in mind that some log processors have problems dealing with this multiline stack traces, because each line is treated like a separated log entry.

If you use `LogfmtStackTraceFormat.Single` then the stack trace is generated in a single (long) line:

```
ts=2021-02-19T18:35:26.8931544Z level=err err="Authentication failed because the remote party has closed the transport stream."    at System.Net.Security.SslStream.StartReadFrame(Byte[] buffer, Int32 readBytes, AsyncProtocolRequest asyncRequest)   at System.Net.Security.SslStream.PartialFrameCallback(AsyncProtocolRequest asyncRequest)--- End of stack trace from previous location where exception was thrown ---   at System.Net.Security.SslStream.ThrowIfExceptional()   at System.Net.Security.SslStream.InternalEndProcessAuthentication(LazyAsyncResult lazyResult)   at System.Net.Security.SslStream.EndProcessAuthentication(IAsyncResult result)   at System.Net.Security.SslStream.EndAuthenticateAsServer(IAsyncResult asyncResult)   at System.Net.Security.SslStream.<>c.<AuthenticateAsServerAsync>b__69_1(IAsyncResult iar)   at System.Threading.Tasks.TaskFactory`1.FromAsyncCoreLogic(IAsyncResult iar, Func`2 endFunction, Action`1 endAction, Task`1 promise, Boolean requiresSynchronization)--- End of stack trace from previous location where exception was thrown ---   at Microsoft.AspNetCore.Server.Kestrel.Https.Internal.HttpsConnectionMiddleware.InnerOnConnectionAsync(ConnectionContext context)
```

## How to contribute

If you use the formatter feel free to post any issue or a PR. :)

## What next?

I have some improvements in mind, but nothing defined yet.