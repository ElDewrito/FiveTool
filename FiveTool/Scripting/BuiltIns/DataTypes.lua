function StringId(val)
	if type(val) == "string" then
		return StringIdFactory.FromString(val)
	elseif type(val) == "number" then
		return StringIdFactory.FromNumber(val)
	else
		error("Expected a string or a number")
	end
end

DefineHelp("Data", "StringId", {
	shortDescription = "Make a string_id value from a string or number",
	args = {
		{ "val", "The string or number value" },
	},
	examples = {
		"StringId(\"__default__\")",
		"StringId(0x9B555AD2)",
	},
	returns = "The string_id value",
})

function Hex(val)
	if type(val) == "userdata" then
		return val.ToHexString()
	else
		return string.format("%x", val)
	end
end

DefineHelp("Data", "Hex", {
	shortDescription = "Convert a number to a hexadecimal string",
	args = {
		{ "val", "The number value" },
	},
	examples = {
		"Hex(0xDEADBEEF)",
	},
	returns = "The hexadecimal string",
})

function LEHex(val)
	return Hex(BSwap32(val))
end

DefineHelp("Data", "LEHex", {
	shortDescription = "Convert a 32-bit number to a little-endian hexadecimal string",
	args = {
		{ "val", "The number value" },
	},
	examples = {
		"LEHex(0xDEADBEEF)",
	},
	returns = "The hexadecimal string",
})

function UInt64(val)
	if type(val) == "string" then
		return UInt64Factory.FromString(val)
	elseif type(val) == "number" then
		return UInt64Factory.FromNumber(val)
	else
		error("Expected a string or a number")
	end
end

DefineHelp("Data", "UInt64", {
	shortDescription = "Make a 64-bit unsigned integer value",
	longDescription = "Makes a 64-bit unsigned integer value.\n\nThis surpasses Lua's precision limit by returning a userdata object that acts\nlike an integer.",
	args = {
		{ "val", "The number or string value. Hex strings may start with \"0x\"." },
	},
	examples = {
		"UInt64(1337)",
		"UInt64(\"1311768467463790320\")",
		"UInt64(\"0x123456789abcdef0\")",
	},
	returns = "The UInt64 value",
})