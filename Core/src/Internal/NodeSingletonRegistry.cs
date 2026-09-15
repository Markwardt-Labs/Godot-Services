namespace Markwardt.GodotServices.Internal;

/// <summary>
/// Registers autoloaded <see cref="Node"/> singletons with the DI container, capturing each one
/// as it enters the scene tree so its registered service factory can resolve it without a static
/// reference on the node itself.
/// </summary>
/// <param name="services">The service collection to register captured nodes' singletons into.</param>
internal sealed class NodeSingletonRegistry(IServiceCollection services)
{
    private readonly MethodInfo registerMethod = typeof(NodeSingletonRegistry).GetMethod(
        nameof(Register),
        BindingFlags.NonPublic | BindingFlags.Instance)!;

    private readonly Dictionary<Type, Node?> nodesByType = new();

    /// <summary>
    /// Registers <typeparamref name="TNode"/> as the implementation of the
    /// <typeparamref name="TService"/> singleton, resolved from the captured node once
    /// <typeparamref name="TNode"/> has entered the scene tree. A <typeparamref name="TNode"/>
    /// annotated with <see cref="TransientAttribute"/> is registered as transient instead.
    /// </summary>
    /// <typeparam name="TService">The service type to register.</typeparam>
    /// <typeparam name="TNode">The node type implementing <typeparamref name="TService"/>.</typeparam>
    internal void Register<TService, TNode>()
        where TService : class
        where TNode : Node, TService
    {
        nodesByType[typeof(TNode)] = null;

        if (typeof(TNode).IsDefined(typeof(TransientAttribute)))
        {
            services.AddTransient<TService>(_ => Get<TNode>());
        }
        else
        {
            services.AddSingleton<TService>(_ => Get<TNode>());
        }
    }

    /// <summary>
    /// Registers every concrete <see cref="Node"/> subclass in <paramref name="assembly"/> that
    /// implements an interface named after itself (e.g. <c>Thing</c> implementing
    /// <c>IThing</c>) as a node singleton, without requiring explicit registration.
    /// </summary>
    /// <param name="assembly">The assembly to scan for conventionally-named node singletons.</param>
    internal void RegisterConventionNodes(Assembly assembly)
    {
        foreach (Type type in assembly.GetTypes())
        {
            if (!type.IsClass || type.IsAbstract || type.IsGenericTypeDefinition || !typeof(Node).IsAssignableFrom(type))
            {
                continue;
            }

            Type? contract = type.GetInterface($"I{type.Name}");

            if (contract is not null)
            {
                registerMethod.MakeGenericMethod(contract, type).Invoke(this, null);
            }
        }
    }

    /// <summary>
    /// Captures <paramref name="node"/> if its type was previously registered with
    /// <see cref="Register{TService, TNode}"/> or <see cref="RegisterConventionNodes"/>.
    /// </summary>
    /// <param name="node">The node that just entered the scene tree.</param>
    internal void Capture(Node node)
    {
        if (nodesByType.ContainsKey(node.GetType()))
        {
            nodesByType[node.GetType()] = node;
        }
    }

    private TNode Get<TNode>()
        where TNode : Node
    {
        if (!nodesByType.TryGetValue(typeof(TNode), out Node? node) || node is null)
        {
            throw new InvalidOperationException($"'{typeof(TNode).Name}' has not entered the scene tree yet.");
        }

        return (TNode)node;
    }
}
