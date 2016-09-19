function MakeStringId(val)
	if type(val) == "string" then
		return StringId.FromString(val)
	elseif type(val) == "number" then
		return StringId.FromNumber(val)
	else
		error("Expected a string or a number")
	end
end

DefineHelp("Data Type", "MakeStringId", {
	shortDescription = "Make a string_id value from a string or number",
	args = {
		{ "val", "The string or number value" },
	},
	examples = {
		"MakeStringId(\"__default__\")",
		"MakeStringId(0x9B555AD2)",
	},
})