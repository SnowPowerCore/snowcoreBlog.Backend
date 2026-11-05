using AspectInjector.Broker;

namespace snowcoreBlog.Backend.Infrastructure.RequestPersistence;

/// <summary>
/// Aspect (defined in the business repo — MinimalStepifiedSystem stays unmodified) woven around
/// every public method of classes marked with <see cref="CheckpointedStepAttribute"/>. For
/// <c>InvokeAsync(context, next, token)</c> it consults <see cref="IStepCheckpointCoordinator"/>:
/// <list type="bullet">
/// <item>skip decision (resume) → invokes the remaining chain directly, bypassing the step body;</item>
/// <item>run decision → executes the body and schedules a checkpoint capture when the step's task completes.</item>
/// </list>
/// Cross-assembly weaving is the same mechanism the MinimalStepifiedSystem package itself uses
/// (its own <c>StepifiedProcessAttribute</c> aspect weaves from a referenced assembly).
/// </summary>
[Aspect(Scope.Global)]
public class StepCheckpointAspect
{
    private const string InvokeAsyncMethodName = "InvokeAsync";

    [Advice(Kind.Around, Targets = Target.Public | Target.Method)]
    public object WrapStep([Argument(Source.Instance)] object instance,
                           [Argument(Source.Name)] string name,
                           [Argument(Source.Arguments)] object[] args,
                           [Argument(Source.Target)] Func<object[], object> proceed)
    {
        var coordinator = StepCheckpointCoordinator.Current;
        if (coordinator is default(StepCheckpointCoordinator)
            || !string.Equals(name, InvokeAsyncMethodName, StringComparison.Ordinal)
            || args.Length != 2)
        {
            return proceed(args);
        }

        var context = args[0];
        var decision = coordinator.BeginStep(instance, context);
        if (decision.SkipStep)
        {
            // Continue with the remaining pipeline (next(context)) without the step body. The
            // generated chain delegates have the shape (context, CancellationToken token = default),
            // so Type.Missing binds the optional token parameter.
            return ((Delegate)args[1]).DynamicInvoke(context, Type.Missing)!;
        }

        var stepTask = (Task)proceed(args);
        coordinator.TrackCompletion(instance, stepTask, context);
        return stepTask;
    }
}
