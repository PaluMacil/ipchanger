# First Notes (TL):

1. the obvious way to do this is using the WMI library, however WMI has a limitation that it can only interact with network adapters that have a cable plugged in.
2. The second (dangerous) way is editing the registry settings themselves.  Simple enough to do, but editing the registry directly is rarely the greatest option.
3. We can start a process within our C# code to call netsh commands.  Again, calling processes within C# isn't always the best option, but it has the advantage of using easily testable and very good working netsh commands for the change.
