namespace CreativeCoders.Git.Merges;

/// <summary>Flags controlling checkout notification behavior.</summary>
[Flags]
public enum CheckoutNotifyFlags
{
    /// <summary>No checkout notification.</summary>
    None = 0,
    /// <summary>Notify on conflicting paths.</summary>
    Conflict = 1,
    /// <summary>
    /// Notify about dirty files. These are files that do not need
    /// an update, but no longer match the baseline.
    /// </summary>
    Dirty = 2,
    /// <summary>Notify for files that will be updated.</summary>
    Updated = 4,
    /// <summary>Notify for untracked files.</summary>
    Untracked = 8,
    /// <summary>Notify about ignored file.</summary>
    Ignored = 16
}
