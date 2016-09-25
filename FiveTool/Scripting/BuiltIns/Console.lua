DefineHelp ("Core", "Clear", {
	shortDescription = "Clear the screen",
	examples = {
		"Clear()",
	},
});

DefineHelp ("Core", "Dump", {
	shortDescription = "Pretty-print a value's contents",
	args = {
		{ "value", "The value to dump" },
		{ "depth", "(Optional) Maximum recursion depth (-1 = infinite)" },
	},
	examples = {
		"Dump(42)",
		"Dump(\"Hello, world!\")",
		"Dump({ \"a\", \"b\", \"c\" })",
		"Dump({ a = \"b\", c = \"d\" })",
		"Dump(FindModuleEntry(\"storm_assault_rifle.weapon\"))",
	},
});

-- Disable buffering on stdout to make things easier
io.output():setvbuf("no")