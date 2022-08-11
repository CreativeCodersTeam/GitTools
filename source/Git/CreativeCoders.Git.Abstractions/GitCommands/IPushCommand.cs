﻿using System;
using System.Collections.Generic;
using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.Git.Abstractions.Fetches;
using CreativeCoders.Git.Abstractions.Pushes;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.GitCommands;

[PublicAPI]
public interface IPushCommand
{
    IPushCommand CreateRemoteBranchIfNotExists();

    IPushCommand CreateRemoteBranchIfNotExists(bool createRemoteBranchIfNotExists);

    IPushCommand Branch(IGitBranch branch);

    IPushCommand OnPushStatusError(Action<GitPushStatusError> pushStatusError);

    IPushCommand OnPackBuilderProgress(Action<GitPackBuilderProgress> packBuilderProgress);

    IPushCommand OnPackBuilderProgress(Func<GitPackBuilderProgress, bool> packBuilderProgress);

    IPushCommand OnTransferProgress(Action<GitPushTransferProgress> transferProgress);

    IPushCommand OnTransferProgress(Func<GitPushTransferProgress, bool> transferProgress);

    IPushCommand OnNegotiationCompletedBeforePush(Action<IEnumerable<GitPushUpdate>> negotiationCompletedBeforePush);

    IPushCommand OnNegotiationCompletedBeforePush(Func<IEnumerable<GitPushUpdate>, bool> negotiationCompletedBeforePush);

    void Run();
}
