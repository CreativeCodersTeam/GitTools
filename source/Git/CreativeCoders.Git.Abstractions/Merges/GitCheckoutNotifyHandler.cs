namespace CreativeCoders.Git.Abstractions.Merges;

public delegate bool GitCheckoutNotifyHandler(string path, GitCheckoutNotifyFlags notifyFlags);

public delegate void GitSimpleCheckoutNotifyHandler(string path, GitCheckoutNotifyFlags notifyFlags);
