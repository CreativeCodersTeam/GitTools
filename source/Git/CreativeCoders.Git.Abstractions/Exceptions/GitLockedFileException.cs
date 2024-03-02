using System;

namespace CreativeCoders.Git.Abstractions.Exceptions;

public class GitLockedFileException(Exception innerException) : GitException("A file is locked", innerException);