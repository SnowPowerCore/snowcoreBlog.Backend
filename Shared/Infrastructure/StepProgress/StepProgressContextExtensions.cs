using MinimalStepifiedSystem.Base;

namespace snowcoreBlog.Backend.Infrastructure.StepProgress;

/// <summary>
/// Generic extensions that let any MinimalStepifiedSystem step report progress
/// through a <see cref="StepProgressReporter"/> carried by the process context.
/// <para>
/// The reporter is optional by design: when a process runs without an attached
/// reporter (e.g. from a non-streaming endpoint), reporting is a silent no-op.
/// </para>
/// </summary>
public static class StepProgressContextExtensions
{
    private const string ReporterKey = "snowcoreBlog.Backend.Infrastructure.StepProgressReporter";

    /// <summary>
    /// Attaches a <see cref="StepProgressReporter"/> to the process context.
    /// </summary>
    public static void AttachStepProgressReporter(this BaseGenericContext context, StepProgressReporter reporter) =>
        context.SetDataWith(ReporterKey, reporter);

    /// <summary>
    /// Gets the attached <see cref="StepProgressReporter"/> or <see langword="null"/> when none is attached.
    /// </summary>
    public static StepProgressReporter? GetStepProgressReporter(this BaseGenericContext context) =>
        context.GetFromData<StepProgressReporter>(ReporterKey);

    /// <summary>
    /// Reports that the process entered the step identified by <paramref name="stepCode"/>.
    /// A no-op when no reporter is attached to the context.
    /// </summary>
    /// <param name="stepCode">A stable, machine-readable step code without personal data.</param>
    public static void ReportStepProgress(this BaseGenericContext context, string stepCode) =>
        context.GetFromData<StepProgressReporter>(ReporterKey)?.Report(stepCode);
}
