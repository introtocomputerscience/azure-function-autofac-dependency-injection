# Autofac Dependency Injection in Azure Functions

An Autofac based implementation of Dependency Injection based on Boris Wilhelm's [azure-function-dependency-injection](https://github.com/BorisWilhelms/azure-function-dependency-injection) and Scott Holden's [WebJobs.ContextResolver](https://github.com/ScottHolden/WebJobs.ContextResolver) available on NuGet as [AzureFunctions.Autofac](https://www.nuget.org/packages/AzureFunctions.Autofac)

[![Build status](https://ci.appveyor.com/api/projects/status/d6k6g4gbhulqneef?svg=true)](https://ci.appveyor.com/project/vandersmissenc/azure-function-autofac-dependency-injection)

# Features

* Supports utilizing Autofac within Azure Functions
* Supports lifetime scoping
* Supports named dependency resolution
* Supports post container build actions for additonal configuration
* Supports injection of `ILogger<>` derived from provided `ILoggerFactory` for clean logging
* Supports container caching for speed and memory efficiency with the ability to disable if required
* Supports verification of dependency injection configs through unit testing extesions

# Usage

In order to implement the dependency injection you have to create a class to configure DependencyInjection and add an attribute on your function class.

## Configuration

The configuration class is used to setup dependency injestion. Within the constructor of the class DependencyInjection.Initialize must be invoked. Registrations are then according to standard Autofac procedures.  

In both .NET Framework and .NET Core a required functionName parameter is automatically injected for you but you must specify it as a constructor parameter. You can also use the optional ILoggerFactory parameter, to register it into the container, and therefore allow Autofac to inject ILogger<> into your services.

In .NET Core you have another optional baseDirectory parameter that can be used for loading external app configs. If you wish to use this functionality then you must specify this as a constructor parameter and it will be injected for you.

### Functions V1 Example (.NET Framework)

```c#
    public class DIConfig
    {
        public DIConfig(string functionName)
        {
            DependencyInjection.Initialize(builder =>
            {
                //Implicity registration
                builder.RegisterType<Sample>().As<ISample>();
                //Explicit registration
                builder.Register<Example>(c => new Example(c.Resolve<ISample>())).As<IExample>();
                //Registration by autofac module
                builder.RegisterModule(new TestModule());
                //Named Instances are supported
                builder.RegisterType<Thing1>().Named<IThing>("OptionA");
                builder.RegisterType<Thing2>().Named<IThing>("OptionB");
            }, functionName);
        }
    }
```

### Functions V2/V3 Example (.NET Core)

```c#
    public class DIConfig
    {
        public DIConfig(string functionName, string baseDirectory, ILoggerFactory factory)
        {
            DependencyInjection.Initialize(builder =>
            {
                //Implicity registration
                builder.RegisterType<Sample>().As<ISample>();
                //Explicit registration
                builder.Register<Example>(c => new Example(c.Resolve<ISample>())).As<IExample>();
                //Registration by autofac module
                builder.RegisterModule(new TestModule());
                //Named Instances are supported
                builder.RegisterType<Thing1>().Named<IThing>("OptionA");
                builder.RegisterType<Thing2>().Named<IThing>("OptionB");
                // Configure Autofac to provide ILogger<> into constructors
                builder.RegisterLoggerFactory(factory);
            }, functionName);
        }
    }
```

## Function Attribute and Inject Attribute

Once you have created your config class you need to annotate your function class indicating which config to use and annotate any parameters that are being injected. Note: All injected parameters must be registered with the autofac container in your resolver in order for this to work.

```c#
    [DependencyInjectionConfig(typeof(DIConfig))]
    public class GreeterFunction
    {
        [FunctionName("GreeterFunction")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage request, 
                                              ILogger log, 
                                              [Inject]IGreeter greeter, 
                                              [Inject]IGoodbyer goodbye)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            return request.CreateResponse(HttpStatusCode.OK, $"{greeter.Greet()} {goodbye.Goodbye()}");
        }
    }
```

### Extra attribute Functions V1 (.NET Framework)

An extra attribute, ScopeFilterAttribute, is necessary for Functions V1 to properly release the objects created by the autofac container.

```c#
    [DependencyInjectionConfig(typeof(DIConfig))]
    [ScopeFilter]
    public class GreeterFunction
    {
        [FunctionName("GreeterFunction")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage request, 
                                              ILogger log, 
                                              [Inject]IGreeter greeter, 
                                              [Inject]IGoodbyer goodbye)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            return request.CreateResponse(HttpStatusCode.OK, $"{greeter.Greet()} {goodbye.Goodbye()}");
        }
    }
```

## Logging with ILoggerFactory

With Azure Functions v2 it is now possible to provide an optional ILoggerFactory when setting up the Dependency Injection Config.

You will need to add a using statement in the Dependency Injection Config for `AzureFunctions.Autofac.Shared.Extensions` and add the following line in your Initialize function:

```c#
builder.RegisterLoggerFactory(factory);
```

It will now be possible for Autofac to inject into your classes an ILogger<> that can be used to output to the console or configured location.

An example of this is in the [Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-3.1#create-logs) as well as in this repo in the [LogWriter](https://github.com/introtocomputerscience/azure-function-autofac-dependency-injection/blob/master/NetCoreExample/Models/LogWriter.cs)

Note that you must also update the *host.json* file to contain a Logging Configuration. See the [Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-3.1#configure-logging) for more details.

## Using Named Dependencies

Support has been added to use named dependencies. Simple add a name parameter to the Inject attribute to specify which instance to use.

```c#
    [DependencyInjectionConfig(typeof(DIConfig))]
    public class GreeterFunction
    {
        [FunctionName("GreeterFunction")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage request, 
                                              ILogger log, 
                                              [Inject]IGreeter greeter, 
                                              [Inject("Main")]IGoodbyer goodbye, 
                                              [Inject("Secondary")]IGoodbyer alternateGoodbye)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            return request.CreateResponse(HttpStatusCode.OK, $"{greeter.Greet()} {goodbye.Goodbye()} or {alternateGoodbye.Goodbye()}");
        }
    }
```

## Multiple Dependency Injection Configurations

In some cases you may wish to have different dependency injection configs for different classes. This is supported by simply annotating the other class with a different dependency injection config.

```c#
    [DependencyInjectionConfig(typeof(DIConfig))]
    public class GreeterFunction
    {
        [FunctionName("GreeterFunction")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage request, 
                                              ILogger log, 
                                              [Inject]IGreeter greeter, 
                                              [Inject]IGoodbyer goodbye)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            return request.CreateResponse(HttpStatusCode.OK, $"{greeter.Greet()} {goodbye.Goodbye()}");
        }
    }

    [DependencyInjectionConfig(typeof(SecondaryConfig))]
    public class SecondaryGreeterFunction
    {
        [FunctionName("SecondaryGreeterFunction")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage request, 
                                              ILogger log, 
                                              [Inject]IGreeter greeter, 
                                              [Inject]IGoodbyer goodbye)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            return request.CreateResponse(HttpStatusCode.OK, $"{greeter.Greet()} {goodbye.Goodbye()}");
        }
    }
```

## Post-Build Container Setup

If you wish to perform actions on the DI container after it has been built you can pass an `Action<IContainer>` as the last parameter of the DependencyInjection.Initialize function.

```c#
public class DIConfig
    {
        public DIConfig(string functionName)
        {
            var tracer = new DotDiagnosticTracer();
            tracer.OperationCompleted += (sender, args) =>
            {
                // Writing the DOT trace to a file will let you render
                // it to a graph with Graphviz later, but this is
                // NOT A GOOD COPY/PASTE EXAMPLE. You'll want to do
                // things in an async fashion with good error handling.
                var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.dot");
                using var file = new StreamWriter(path);
                file.WriteLine(args.TraceContent);
            };
            DependencyInjection.Initialize(builder =>
            {
                builder.RegisterType<Greeter>().As<IGreeter>();
            }, functionName, c => c.SubscribeToDiagnostics(tracer));
        }
    }
```

[See Example](DOTGraphTracerExample/Configs/DIConfig.cs)

## Container Caching

By default containers are cached using the function name and a new lifetime scope of the container is created for each function invocation. This means that if you register a type as single instance then it will be provided to each function with the same name even though they are in different lifetime scopes. In some cases this behavior is not desired and as such you can disable caching during dependency injection initialization by passing `enableCaching` as `false`

```c#
public class DIConfig
    {
        public DIConfig(string functionName, string baseDirectory, ILoggerFactory factory)
        {
            DependencyInjection.Initialize(builder =>
            {
                builder.RegisterType<Sample>().As<ISample>().SingleInstance();
            }, functionName, enableCaching: false);
        }
    }
```

[See Enabled Caching Example](caching-example/Config/CachingConfig.cs)

[See Disabled Caching Example](caching-example/Config/NonCachingConfig.cs)

# Verifying dependency injection configuration

Dependency injection is a great tool for creating unit tests. But with manual configuration of the dependency injection, there is a risk of mis-configuration that will not show up in unit tests. For this purpose, there is the `DependencyInjection.VerifyConfiguration` method.

It is not recommended to call `VerifyConfiguration` unless done so in a test-scenario.

`VerifyConfiguration` verifies the following rules:

1. That an `InjectAttribute` is preceeded by a `DependencyInjectionConfigAttribute`.
2. That the configuration can be instantiated.
3. That all injected dependencies in the given type can be resolved with the defined configuration.
4. Optionally that no redundant configurations exist, i.e. a `DependencyInjectionConfigAttribute` with no corresponding `InjectAttribute`.

## Simple example of verification

Below is a very simple example of verifying the dependency injection configuration for a specific class:

```c#
    DependencyInjection.VerifyConfiguration(typeof(MyCustomClassThatUsesDependencyInjection));
```

## Ignoring redundant configurations

If you don't want to verify rule 4, pass in `false` as the second parameter to `VerifyConfiguration`:

```c#
    DependencyInjection.VerifyConfiguration(typeof(MyCustomClassThatUsesDependencyInjection), false);
```

## Example unit test to verify an entire project/assembly
For instance, you can use it in a unit test to verify that all classes in your project has dependency injection set up correctly:

```c#
    [TestMethod]
    public void TestDependencyInjectionConfigurationInAssembly() {
        var assembly = typeof(SomeClassInYouProject).Assembly;
        var types = assembly.GetTypes();
        foreach (var type in types) {
            DependencyInjection.VerifyConfiguration(type);
        }
    }
```

[See Example](AzureFunctions.Autofac.NetStandard.Tests/DependencyInjectionVerificationTests.cs)

# Buy me a beer?

I love programming and all but as both a day job and a hobby it can get tiring. I mean I could have just been drinking a beer or playing some games. If this project helped you advance your own work then why not consider buying me a beer...

* [Buy me a nice beer](https://www.paypal.me/cjvds/5.00?locale.x=en_US)
* [Buy me a good beer](https://www.paypal.me/cjvds/2.50?locale.x=en_US)
* [Buy me a decent beer](https://www.paypal.me/cjvds/1.50?locale.x=en_US)

and I will raise one to you! Much appreciated!
