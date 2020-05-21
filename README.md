# Autofac Dependency Injection in Azure Functions
An Autofac based implementation of Dependency Injection based on Boris Wilhelm's [azure-function-dependency-injection](https://github.com/BorisWilhelms/azure-function-dependency-injection) and Scott Holden's [WebJobs.ContextResolver](https://github.com/ScottHolden/WebJobs.ContextResolver) available on NuGet as [AzureFunctions.Autofac](https://www.nuget.org/packages/AzureFunctions.Autofac)

[![Build status](https://ci.appveyor.com/api/projects/status/d6k6g4gbhulqneef?svg=true)](https://ci.appveyor.com/project/vandersmissenc/azure-function-autofac-dependency-injection)


## Usage
In order to implement the dependency injection you have to create a class to configure DependencyInjection and add an attribute on your function class.

### Configuration

The configuration class is used to setup dependency injestion. Within the constructor of the class DependencyInjection.Initialize must be invoked. Registrations are then according to standard Autofac procedures.  

In both .NET Framework and .NET Core a required functionName parameter is automatically injected for you but you must specify it as a constructor parameter. You can also use the optional ILoggerFactory parameter, to register it into the container, and therefore allow Autofac to inject ILogger<> into your services.

In .NET Core you have another optional baseDirectory parameter that can be used for loading external app configs. If you wish to use this functionality then you must specify this as a constructor parameter and it will be injected for you.

#### Functions V1 Example (.NET Framework)

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

#### Functions V2 Example (.NET Core)

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
### Function Attribute and Inject Attribute
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
### Logging with ILoggerFactory
With Azure Functions v2 it is now possible to provide an optional ILoggerFactory when setting up the Dependency Injection Config. 

You will need to add a using statement in the Dependency Injection Config for `AzureFunctions.Autofac.Shared.Extensions` and add the following line in your Initialize function:
```
builder.RegisterLoggerFactory(factory);
```

You will also need to define an [ILogWriter](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-3.1#create-logs) implementation such as the example [LogWriter](https://github.com/introtocomputerscience/azure-function-autofac-dependency-injection/blob/master/NetCoreExample/Models/LogWriter.cs)

Once it has been created in the Initialize function you need to register it:
```
builder.RegisterType<LogWriter>().As<ILogWriter>();
```

It will now be possible for Autofac to inject into your classes an ILogWriter that can be used to output to the console or configured location.

Note that you must also update the *host.json* file to contain a Logging Configuration. See the [Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-3.1#configure-logging) for more details.
### Using Named Dependencies
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
### Multiple Dependency Injection Configurations
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

## Verifying dependency injection configuration
Dependency injection is a great tool for creating unit tests. But with manual configuration of the dependency injection, there is a risk of mis-configuration that will not show up in unit tests. For this purpose, there is the `DependencyInjection.VerifyConfiguration` method.

It is not recommended to call `VerifyConfiguration` unless done so in a test-scenario.

`VerifyConfiguration` verifies the following rules:
1. That an `InjectAttribute` is preceeded by a `DependencyInjectionConfigAttribute`.
2. That the configuration can be instantiated.
3. That all injected dependencies in the given type can be resolved with the defined configuration.
4. Optionally that no redundant configurations exist, i.e. a `DependencyInjectionConfigAttribute` with no corresponding `InjectAttribute`.
### Simple example of verification
Below is a very simple example of verifying the dependency injection configuration for a specific class:
```c#
    DependencyInjection.VerifyConfiguration(typeof(MyCustomClassThatUsesDependencyInjection));
```
### Ignoring redundant configurations
If you don't want to verify rule 4, pass in `false` as the second parameter to `VerifyConfiguration`:
```c#
    DependencyInjection.VerifyConfiguration(typeof(MyCustomClassThatUsesDependencyInjection), false);
```
### Example unit test to verify an entire project/assembly
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
