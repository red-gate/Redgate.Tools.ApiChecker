# Redgate.Tools.ApiChecker
A simple library to validate whether a given assembly only exports types it owns.

This is important so you don't force your choices onto consumers. For example, if you use a particular JSON library and inadvertantly return a public type from that interface you force the choice onto consumers of your library.

This can also lead to diamond dependencies.
