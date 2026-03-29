namespace CreativeCoders.Git.Abstractions.Merges;

/// <summary>
/// Represents a handler for checkout notifications that can cancel the operation.
/// </summary>
/// <param name="path">The path of the file being checked out.</param>
/// <param name="notifyFlags">A bitwise combination of the enumeration values that specifies the type of notification.</param>
/// <returns><see langword="true"/> to continue the checkout; otherwise, <see langword="false"/> to cancel.</returns>
public delegate bool GitCheckoutNotifyHandler(string path, GitCheckoutNotifyFlags notifyFlags);

/// <summary>
/// Represents a handler for checkout notifications that does not cancel the operation.
/// </summary>
/// <param name="path">The path of the file being checked out.</param>
/// <param name="notifyFlags">A bitwise combination of the enumeration values that specifies the type of notification.</param>
public delegate void GitSimpleCheckoutNotifyHandler(string path, GitCheckoutNotifyFlags notifyFlags);
