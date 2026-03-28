namespace CreativeCoders.Git.Abstractions.Diffs;

/// <summary>
/// Specifies the UNIX file mode of a Git tree entry.
/// </summary>
public enum GitEntryMode
{
    /// <summary>The entry does not exist.</summary>
    Nonexistent = 0,
    /// <summary>The entry is a directory (tree).</summary>
    Directory = 16384, // 0x00004000
    /// <summary>The entry is a non-executable file (blob).</summary>
    NonExecutableFile = 33188, // 0x000081A4
    /// <summary>The entry is a non-executable, group-writable file (blob).</summary>
    NonExecutableGroupWritableFile = 33204, // 0x000081B4
    /// <summary>The entry is an executable file (blob).</summary>
    ExecutableFile = 33261, // 0x000081ED
    /// <summary>The entry is a symbolic link.</summary>
    SymbolicLink = 40960, // 0x0000A000
    /// <summary>The entry is a Git submodule link.</summary>
    GitLink = 57344 // 0x0000E000
}
