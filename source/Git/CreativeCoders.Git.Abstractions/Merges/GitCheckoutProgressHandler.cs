namespace CreativeCoders.Git.Abstractions.Merges;

public delegate void GitCheckoutProgressHandler(string path, int completedSteps, int totalSteps);
