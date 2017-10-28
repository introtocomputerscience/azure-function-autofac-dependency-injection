# Autofac Dependency Injection in Azure Functions
An Autofac based implementation of Dependency Injection based on Boris Wilhelm's [azure-function-dependency-injection](https://github.com/BorisWilhelms/azure-function-dependency-injection) and Scott Holden's [WebJobs.ContextResolver](https://github.com/ScottHolden/WebJobs.ContextResolver)

## Usage
In order to implement the dependency injection you have to create a resolver and add an attribute on your function.

### Resolver
The resolver has to implement the IInjectResolver class which just requires them to resolve an instance based on a Type. Theoretically you could use a different system other than autofac by implementing a different version of the resolver but this is currently an untested theory.
```c#
public class AutofacResolver : IInjectResolver
{
    private readonly IContainer container;

    public AutofacResolver()
    {
        ContainerBuilder builder = new ContainerBuilder();
        builder.RegisterType<Greeter>().As<IGreeter>();
        this.container = builder.Build();
    }

    public object Resolve(Type type)
    {
        return this.container.Resolve(type);
    }
}
```
### Function Attribute and Inject Attribute
Once you have created your resolver you need to annotate your function indicating which resolver to use and annotate any parameters that are being injected. Note: All injected parameters must be registered with the autofac container in your resolver in order for this to work.
```c#
public class GreeterFunction
{
    [InjectResolver(typeof(AutofacResolver))]
    [FunctionName("GreeterFunction")]
    public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage request, TraceWriter log, [Inject]IGreeter greeter)
    {
        log.Info("C# HTTP trigger function processed a request.");
        return request.CreateResponse(HttpStatusCode.OK, greeter.Greet());
    }
}
```
