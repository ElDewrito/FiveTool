function StringId(val)
	if type(val) == "string" then
		return StringIdFactory.FromString(val)
	elseif type(val) == "number" then
		return StringIdFactory.FromNumber(val)
	else
		error("Expected a string or a number")
	end
end

DefineHelp("Data Type", "StringId", {
	shortDescription = "Make a string_id value from a string or number",
	args = {
		{ "val", "The string or number value" },
	},
	examples = {
		"StringId(\"__default__\")",
		"StringId(0x9B555AD2)",
	},
})

function Hex(val)
	return string.format("%x", val)
end

DefineHelp("Data Type", "Hex", {
	shortDescription = "Convert a number to a hexadecimal string",
	args = {
		{ "val", "The number value" },
	},
	examples = {
		"Hex(0xDEADBEEF)",
	},
})

function LEHex(val)
	return Hex(BSwap32(val))
end

DefineHelp("Data Type", "LEHex", {
	shortDescription = "Convert a 32-bit number to a little-endian hexadecimal string",
	args = {
		{ "val", "The number value" },
	},
	examples = {
		"LEHex(0xDEADBEEF)",
	},
})