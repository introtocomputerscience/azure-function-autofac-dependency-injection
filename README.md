# Autofac Dependency Injection in Azure Functions
An Autofac based implementation of Dependency Injection based on Boris Wilhelm's [azure-function-dependency-injection](https://github.com/BorisWilhelms/azure-function-dependency-injection) and Scott Holden's [WebJobs.ContextResolver](https://github.com/ScottHolden/WebJobs.ContextResolver) available on NuGet as [AzureFunctions.Autofac](https://www.nuget.org/packages/AzureFunctions.Autofac)

## Usage
In order to implement the dependency injection you have to create a class to configure DependencyInjection and add an attribute on your function class.

### Configuration
Create a class and add a constructor that takes 1 string argument. The string argument will automatically be passed and runtime and simply needs to be passed through to the initialize method. In the constructor call the DependencyInjection.Initialize method. Perform the registrations as you normally would with Autofac.
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
### Function Attribute and Inject Attribute
Once you have created your config class you need to annotate your function class indicating which config to use and annotate any parameters that are being injected. Note: All injected parameters must be registered with the autofac container in your resolver in order for this to work.
```c#
    [DependencyInjectionConfig(typeof(DIConfig))]
    public class GreeterFunction
    {
        [FunctionName("GreeterFunction")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage request, 
                                              TraceWriter log, 
                                              [Inject]IGreeter greeter, 
                                              [Inject]IGoodbyer goodbye)
        {
            log.Info("C# HTTP trigger function processed a request.");
            return request.CreateResponse(HttpStatusCode.OK, $"{greeter.Greet()} {goodbye.Goodbye()}");
        }
    }
```
### Using Named Dependencies
Support has been added to use named dependencies. Simple add a name parameter to the Inject attribute to specify which instance to use.
```c#
    [DependencyInjectionConfig(typeof(DIConfig))]
    public class GreeterFunction
    {
        [FunctionName("GreeterFunction")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage request, 
                                              TraceWriter log, 
                                              [Inject]IGreeter greeter, 
                                              [Inject("Main")]IGoodbyer goodbye, 
                                              [Inject("Secondary")]IGoodbyer alternateGoodbye)
        {
            log.Info("C# HTTP trigger function processed a request.");
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
                                              TraceWriter log, 
                                              [Inject]IGreeter greeter, 
                                              [Inject]IGoodbyer goodbye)
        {
            log.Info("C# HTTP trigger function processed a request.");
            return request.CreateResponse(HttpStatusCode.OK, $"{greeter.Greet()} {goodbye.Goodbye()}");
        }
    }

    [DependencyInjectionConfig(typeof(SecondaryConfig))]
    public class SecondaryGreeterFunction
    {
        [FunctionName("SecondaryGreeterFunction")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage request, 
                                              TraceWriter log, 
                                              [Inject]IGreeter greeter, 
                                              [Inject]IGoodbyer goodbye)
        {
            log.Info("C# HTTP trigger function processed a request.");
            return request.CreateResponse(HttpStatusCode.OK, $"{greeter.Greet()} {goodbye.Goodbye()}");
        }
    }
```