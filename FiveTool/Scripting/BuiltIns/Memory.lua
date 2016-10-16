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
	returns = "A table mapping each loaded tag's global ID to its local handle",
	examples = {
		"MemGetLoadedTags()",
	},
})

DefineHelp("Memory", "MemGetTagAddressFromHandle", {
	shortDescription = "Get a tag's address from its handle",
	args = {
		{ "handle", "The tag's local handle" },
	},
	returns = "The address of the tag's main structure, or 0 if the handle is invalid",
	examples = {
		"Hex(MemGetTagAddressFromHandle(MemGetLocalTagHandle(0x8595)))",
	},
})

function MemGetTagAddress(globalId)
	local handle = MemGetLocalTagHandle(globalId)
	if handle != -1 then
		return MemGetTagAddressFromHandle(handle)
	else
		return 0
	end
end

DefineHelp("Memory", "MemGetTagAddress", {
	shortDescription = "Get a tag's address",
	args = {
		{ "globalId", "The tag's global ID" },
	},
	returns = "The address of the tag's main structure, or 0 if the handle is invalid",
	examples = {
		"Hex(MemGetTagAddress(0x8595))",
	},
})