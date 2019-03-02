# v1.0
## Features
- Provides basic configuration functions, adding configuration items to the database, and applications can get configuration items
- Database configuration items should follow the key-value rule, where the Key must be selected to follow the ": rule ", that is, if the Key Value needs to be classified, colon: shall be used for segmentation.

# v1.1
## Updates
- Support for automatic retrieval of the latest configuration from the database. This is the default behavior;
- Through SqlServerConfigurationSource constructor, executable configuration access behavior; Get the latest configuration from the database every 5 seconds by default; The corresponding parameters are:
- ReloadOnChange: default is true, when set to False, no longer automatically get the latest configuration;
- RefreshInterval: defaults to 5 seconds.