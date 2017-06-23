# keepass2-passwordmodifiedcolumn

**KeePass 2.x plugin to show the time a password was last modified.**

**This functionality is now built into KeePass as of 2.36, however this is maintained for users that cannot upgrade.**

**Download plgx from [here](https://github.com/andrew-schofield/keepass2-passwordmodifiedcolumn/raw/master/PasswordModifiedColumn.plgx).**

**Mono users can download the dlls from [here](https://github.com/andrew-schofield/keepass2-passwordmodifiedcolumn/tree/master/mono).**

## Usage

* Enable the custom column. If history entries are available for an entry they will be used to calculated a password last modified date.

## How it works

Unfortunately KeePass doesn't keep track of when individual fields are modified in an entry, it just stores an overall `Last Modified` date for the whole entry. This poses a problem if you want to work out when a password was last changed.

Luckily KeePass stores history entries whenever a modification is made. These can be used to work out when a password was last changed by comparing the passwords in the history entries.

* If there are no history entries we have to assume that the `Last Modified ` date of the entry is the last time the password was changed.
* If the password is the same in all the history entries and the current entry then we assume the modification date of the earliest history entry is the last time the password was changed.
* Otherwise we start from the current entry and work backwards through the history looking for the first time the password changed and use the later of two entries which have different passwords.

This should give accurate results if you maintain the history for each entry, however if you prune the history it will only be accurate as far back as there is history data.

## Notes

* Sorting does not work currently due to this not being available to custom columns.


## Donate

keepass2-passwordmodifiedcolumn is developed entirely in my own time. If you wish to support development you can donate via paypal here.

[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=S2DVYTS47PX4S)

## Copyright

&copy; 2017 Andrew Schofield
