namespace CreativeCoders.Git.Abstractions.Diffs;

public enum GitEntryMode
{
    Nonexistent = 0,
    Directory = 16384, // 0x00004000
    NonExecutableFile = 33188, // 0x000081A4
    NonExecutableGroupWritableFile = 33204, // 0x000081B4
    ExecutableFile = 33261, // 0x000081ED
    SymbolicLink = 40960, // 0x0000A000
    GitLink = 57344, // 0x0000E000
}