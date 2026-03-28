namespace CreativeCoders.Git.Abstractions.Merges;

/// <summary>
/// Represents a handler for checkout progress updates.
/// </summary>
/// <param name="path">The path of the file currently being processed.</param>
/// <param name="completedSteps">The number of completed checkout steps.</param>
/// <param name="totalSteps">The total number of checkout steps.</param>
public delegate void GitCheckoutProgressHandler(string path, int completedSteps, int totalSteps);
