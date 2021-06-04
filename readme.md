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

### Double quotes

If the logged property contains an space, it is surrounded between double quotes. To avoid invalid log messages if the property itself contains double quotes, by default the formatter converts the double quotes property to single quotes. However this is configurable using the `OnDoubleQuotes` configuration method:

```csharp
.UseSerilog((hostBuilderContext, config) =>
{
   .WriteTo.Console(formatter: new LogfmtFormatter(opt => opt.OnDoubleQuotes(q => q.ConvertToSingle())));
})
```

Following options are available:

* Convert double quotes to single (`q.ConvertToSingle()`). This is the default.
* Remove the double quotes of the property (`q.Remove()`).
* Escape the double quotes of the property using `\"` sequence (`q.Escape()`).
* Do nothing (this can lead to invalid log messages) (`q.Preserve()`).

For example, using `q.ConvertToSingle()` and logging the following:

```csharp
var value = @"This value also have ""double quotes"" on it";
logger.LogInformation(@"Message with ""double quotes"" and a str value: {value} :) ", value);
```

Will generate following message:

```
ts=2021-06-04T07:48:03.8528712Z level=info msg="Message with 'double quotes' and a str value: This value also have 'double quotes' on it :) "
```  

### Including all LogEvent properties

By default, the formatter only serializes the timestamp, level and message properties of the `LogEvent` object. However you can choose to serialize all LogEvent properties

```csharp
new LogfmtFormatter(opt.IncludeAllProperties())
```

Using `opt.IncludeAllProperties()` and following code:

```csharp
LogContext.PushProperty("complex", new { Name = @"Property ""DOUBLE QUOTES"" on it", When = DateTime.UtcNow, Value = 42, Sub = new { Name = "Test", Iv = 32 } }, true);
LogContext.PushProperty("str", "Simple string property");
LogContext.PushProperty("int", 42);
var value = @"This value also have ""double quotes"" on it";
logger.LogInformation(@"Message with ""double quotes"" and a str value: {value} :) ", value);
```

Generates the following log message:

```
ts=2021-06-04T11:24:59.9399900Z level=info value="This value also have 'double quotes' on it" int=42 str="Simple string property" complex.name="Property 'DOUBLE QUOTES' on it" complex.when="6/4/2021 11:24:59 AM" complex.value=42 complex.sub.name=Test complex.sub.iv=32   msg="Message with 'double quotes' and a str value: This value also have 'double quotes' on it :) "
```

If serializing a complex property, each field name is concatenated to the property name. A complex property like:

```csharp
LogContext.PushProperty("complex", new { Name = @"Property ""DOUBLE QUOTES"" on it", When = DateTime.UtcNow, Value = 42, Sub = new { Name = "Test", Iv = 32 } }, true);
```

Generates following entries in the log line:

```
complex.name="Property 'DOUBLE QUOTES' on it" complex.when="6/4/2021 11:24:59 AM" complex.value=42 complex.sub.name=Test complex.sub.iv=32
```

By default a dot (`.`) is used a separator but you can use the `UseComplexPropertySeparator` with the separator you want to use:

```csharp
WriteTo.Console(formatter: new LogfmtFormatter(opt => opt.IncludeAllProperties().UseComplexPropertySeparator("->")));
``` 

Following entries are generated in the log line:

```
complex->name="Property 'DOUBLE QUOTES' on it" complex->when="6/4/2021 2:38:59 PM" complex->value=42 complex->sub->name=Test complex->sub->iv=32
```

For sequence properties entries generated in the log line use array sintax:

```csharp
LogContext.PushProperty("test", new[] {10, 100, 1000});
```

This property generates following entries in log line:

```
test[0]=10 test[1]=100 test[2]=1000
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