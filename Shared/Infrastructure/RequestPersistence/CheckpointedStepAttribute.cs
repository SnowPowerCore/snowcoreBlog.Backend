using AspectInjector.Broker;

namespace snowcoreBlog.Backend.Infrastructure.RequestPersistence;

/// <summary>
/// Marks a step class as checkpoint-aware for the durable request-persistence feature. When the
/// application registers an <see cref="IStepCheckpointCoordinator"/>, every checkpointed step
/// reports its entry and completion boundaries to it, enabling durable replay/resume of
/// interrupted pipelines.
/// <para>
/// Business-repo metadata only (the shared MinimalStepifiedSystem library stays unmodified):
/// AspectInjector weaves the advice from this referenced assembly into the step's
/// <c>InvokeAsync</c> at compile time. Pure marker — inert without a registered coordinator.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
[Injection(typeof(StepCheckpointAspect))]
public sealed class CheckpointedStepAttribute : Attribute
{
}
