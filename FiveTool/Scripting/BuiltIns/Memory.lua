DefineHelp("Memory", "MemGetLocalTagHandle", {
	shortDescription = "Get a tag's local handle from memory",
	args = {
		{ "globalId", "The tag's global ID" },
	},
	returns = "The tag's local handle, or -1 if not loaded",
	examples = {
		"MemGetLocalTagHandle(0x8595)",
	},
})

DefineHelp("Memory", "MemGetLoadedTags", {
	shortDescription = "Get a table of all loaded tags (slow!)",
	returns = "A table mapping each loaded tag's global ID to its local tag handle",
	examples = {
		"MemGetLoadedTags()",
	},
})