# MonitorFileSystemWatcher
C# Linq Script to Monitor a File System or File Share for Changes
It shows Filenames for modified/created Files also for Sub-Directory where the User Account has no Access Rights to.
Works on all Windows Versions.

The ReadDirectoryChangesW API function on Microsoft Windows 2000, XP, Server 2003, and Vista, Windows 8, Windows 10 does not check permissions for child objects, which allows local users to bypass permissions by opening a directory with LIST (READ) access and using ReadDirectoryChangesW to monitor changes of files that do not have LIST permissions, which can be leveraged to determine filenames, access times, and other sensitive information.
Background CVE-2007-0843 https://nvd.nist.gov/vuln/detail/CVE-2007-0843

Workaround (breaks a lot of other Stuff):
Disabling Bypass Traverse Checking for the User Account.
https://docs.microsoft.com/en-us/windows/security/threat-protection/security-policy-settings/bypass-traverse-checking
